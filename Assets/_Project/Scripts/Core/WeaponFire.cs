using UnityEngine;
using System.Collections; 

public class WeaponFire : MonoBehaviour
{
    private VehicleStats _stats;
    private AudioSource _audioSource;

    [Header("References")]
    public Transform firePoint;

    private float _nextFireTime;
    private bool _isPlayer; // Флаг, принадлежит ли пушка игроку

    private int _currentAmmo;
    private bool _isReloading = false;
    private float _reloadTimer; 
    
    // Свойства для UI
    public float ReloadProgress => _stats != null && _stats.Weapon != null ? _reloadTimer / _stats.Weapon.shootingStats.reloadTime : 0f; 
    public float ReloadTimeRemaining => _stats != null && _stats.Weapon != null ? Mathf.Max(0, _stats.Weapon.shootingStats.reloadTime - _reloadTimer) : 0f;

    private void Start()
    {
        // Ищем VehicleStats на корневом объекте (Игроке или Враге)
        _stats = GetComponentInParent<VehicleStats>();
        _audioSource = GetComponent<AudioSource>();

        if (_stats == null)
        {
            Debug.LogError($"WeaponFire на {gameObject.name} не нашел VehicleStats!");
            return;
        }

        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.5f; 
        }

        // Определяем, кто хозяин этой пушки
        _isPlayer = transform.root.CompareTag("Player");

        // Инициализируем патроны, если пушка загрузилась успешно
        if (_stats.Weapon != null)
        {
            _currentAmmo = _stats.Weapon.shootingStats.magazineSize;
        }
    }

    private void Update()
    {
        if (_stats == null || _stats.Weapon == null) return;

        // --- ИСПРАВЛЕНИЕ ЛОГИКИ УПРАВЛЕНИЯ ---
        // Если это пушка ИГРОКА, она слушает мышь и клавиатуру
        if (_isPlayer)
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ManualReload();
            }
        }
        // Вражеская пушка здесь ничего не делает. 
        // Её метод Shoot() будет вызываться скриптом AIController!
    }

    public void Shoot()
    {
        if (_isReloading || Time.time < _nextFireTime || _stats == null || _stats.Weapon == null) return;

        var weapon = _stats.Weapon;

        if (_currentAmmo > 0)
        {
            // ИСПРАВЛЕНИЕ 1: Обращаемся к звукам через структуру visuals
            if (weapon.visuals.fireSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(weapon.visuals.fireSound);
            }

            // ИСПРАВЛЕНИЕ 2: Обращаемся к эффектам через структуру visuals
            if (weapon.visuals.muzzleFlashPrefab != null && firePoint != null)
            {
                GameObject flash = Instantiate(weapon.visuals.muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
                Destroy(flash, 0.1f);
            }

            

            // ... начало метода Shoot()

            if (weapon.bulletPrefab != null && firePoint != null)
            {
                GameObject proj = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);
                Projectile projectileScript = proj.GetComponent<Projectile>();
                
                // --- ИСПРАВЛЕННАЯ ЛОГИКА КРИТА ---
                bool isCritical = Random.value < weapon.damageStats.criticalHitChance;
                float finalDamage;

                if (isCritical)
                {
                    // Крит всегда считается от МАКСИМАЛЬНОГО урона
                    finalDamage = weapon.damageStats.maxDamage * weapon.damageStats.criticalDamageMultiplier;
                }
                else
                {
                    // Обычный выстрел берет случайное значение в диапазоне
                    finalDamage = Random.Range(weapon.damageStats.minDamage, weapon.damageStats.maxDamage);
                }

                // Применяем глобальный модификатор (волны / баффы)
                if (_stats != null)
                {
                    finalDamage *= _stats.DamageMultiplier; // (Убедитесь, что переменная называется именно так)
                }
                // ---------------------------------

                if (projectileScript != null)
                {
                    projectileScript.Launch(
                        finalDamage, 
                        weapon.shootingStats.bulletSpeed, 
                        weapon.shootingStats.range, 
                        _stats.gameObject, // Передаем владельца
                        isCritical
                    );
                }
            }    


            _currentAmmo--;
            
            // Расчет скорострельности (перевод RPM в секунды между выстрелами)
            float fireRateSeconds = 60f / weapon.shootingStats.fireRateRPM;
            _nextFireTime = Time.time + fireRateSeconds;

            if (_currentAmmo <= 0) 
            {
                StartCoroutine(ReloadRoutine());
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        _reloadTimer = 0f;

        float duration = _stats.Weapon.shootingStats.reloadTime;

        while (_reloadTimer < duration)
        {
            _reloadTimer += Time.deltaTime;
            yield return null; 
        }

        _currentAmmo = _stats.Weapon.shootingStats.magazineSize;
        _isReloading = false;
        _reloadTimer = 0f;
    }

    public int GetCurrentAmmo() => _currentAmmo;
    public bool IsReloading() => _isReloading;
    
    public void ManualReload()
    {
        if (_isReloading || _stats == null || _stats.Weapon == null) return;
        if (_currentAmmo < _stats.Weapon.shootingStats.magazineSize)
        {
            StartCoroutine(ReloadRoutine());
        }
    }
}