using UnityEngine;
using System.Collections; // Необходимо для корутин

public class WeaponFire : MonoBehaviour
{
    private VehicleStats _stats;
    private AudioSource _audioSource;

    [Header("References")]
    public Transform firePoint;

    private float _nextFireTime;
    private bool _isPlayer;

    // --- Новые переменные для системы патронов ---
    private int _currentAmmo;
    private bool _isReloading = false;
    private bool _isInitialized = false;
    private float _reloadTimer; // Текущее прошедшее время перезарядки
    public float ReloadProgress => _reloadTimer / _stats.Weapon.reloadTime; // Прогресс от 0 до 1
    public float ReloadTimeRemaining => Mathf.Max(0, _stats.Weapon.reloadTime - _reloadTimer);

    private void Awake()
    {
        // Находим статы на корне машины[cite: 1]
        _stats = GetComponentInParent<VehicleStats>();
        
        // Кэшируем AudioSource для звуков выстрелов[cite: 1]
        _audioSource = GetComponent<AudioSource>();

        if (_stats == null)
        {
            Debug.LogError($"WeaponFire на {gameObject.name} не нашел VehicleStats!");
            return;
        }

        // Если компонента звука нет, добавим его автоматически[cite: 1]
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.5f; 
        }

        _isPlayer = _stats.CompareTag("Player");
    }

    private void Update()
    {
        if (_stats == null || _stats.Weapon == null) return;

        // Инициализация патронов при первом появлении оружия
        if (!_isInitialized)
        {
            _currentAmmo = _stats.Weapon.magazineSize;
            _isInitialized = true;
        }

        // Проверки только для Игрока
        if (_isPlayer)
        {
            // Стрельба на ЛКМ[cite: 1]
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }

            // РУЧНАЯ ПЕРЕЗАЯДКА НА 'R'
            if (Input.GetKeyDown(KeyCode.R))
            {
                ManualReload();
            }
        }
    }
    public void Shoot()
    {
        // Проверка кулдауна, наличия оружия и состояния перезарядки
        // Если перезаряжаемся или нет патронов - стрелять нельзя
        if (Time.time < _nextFireTime || _stats == null || _stats.Weapon == null || _isReloading || _currentAmmo <= 0) return;

        var weapon = _stats.Weapon;

        // 1. АУДИО: Берем fireSound и volume из WeaponDataSO[cite: 1]
        if (_audioSource != null && weapon.fireSound != null)
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(weapon.fireSound, weapon.volume);
        }

        // 2. УРОН: Расчет на основе данных оружия[cite: 1]
        bool isCritical = Random.value < weapon.criticalHitChance;
        float baseDamage = isCritical 
            ? weapon.maxDamage * weapon.criticalDamageMultiplier 
            : Random.Range(weapon.minDamage, weapon.maxDamage);

        float finalDamage = baseDamage * _stats.DamageMultiplier;

        // 3. СПАВН И НАСТРОЙКА СНАРЯДА[cite: 1]
        if (weapon.bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);

            int playerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
            int enemyProjectileLayer = LayerMask.NameToLayer("EnemyProjectile");

            bullet.layer = _isPlayer ? playerProjectileLayer : enemyProjectileLayer;

            Projectile projectileScript = bullet.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Launch(
                    finalDamage, 
                    weapon.bulletSpeed, 
                    weapon.range, 
                    _stats.gameObject, 
                    isCritical
                );
            }
        }

        // Тратим 1 патрон
        _currentAmmo--;

        // 4. КУЛДАУН: Используем FireCooldown из WeaponDataSO[cite: 1]
        _nextFireTime = Time.time + weapon.FireCooldown;

        // 5. ПРОВЕРКА НА ПЕРЕЗАРЯДКУ
        // Если патроны закончились сразу после этого выстрела, автоматически запускаем перезарядку
        if (_currentAmmo <= 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    // --- Корутина перезарядки ---
    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        _reloadTimer = 0f;

        float duration = _stats.Weapon.reloadTime;

        while (_reloadTimer < duration)
        {
            _reloadTimer += Time.deltaTime;
            yield return null; // Ждем следующий кадр для плавности UI
        }

        _currentAmmo = _stats.Weapon.magazineSize;
        _isReloading = false;
        _reloadTimer = 0f;
    }

    // Публичные геттеры для UI (например, чтобы выводить "30/30" или "Перезарядка..." на экран)
    public int GetCurrentAmmo() => _currentAmmo;
    public bool IsReloading() => _isReloading;
    
    // Метод для принудительной перезарядки (например, на клавишу 'R')
    public void ManualReload()
    {
        // Не начинаем, если уже в процессе, если нет данных или если магазин уже полный
        if (_isReloading || _stats == null || _stats.Weapon == null) return;
        
        if (_currentAmmo < _stats.Weapon.magazineSize)
        {
            StartCoroutine(ReloadRoutine());
        }
    }
}