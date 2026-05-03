using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    private VehicleStats _stats;
    private AudioSource _audioSource;

    [Header("References")]
    public Transform firePoint;

    private float _nextFireTime;
    private bool _isPlayer;

    private void Awake()
    {
        // Находим статы на корне машины
        _stats = GetComponentInParent<VehicleStats>();
        
        // Кэшируем AudioSource для звуков выстрелов
        _audioSource = GetComponent<AudioSource>();

        if (_stats == null)
        {
            Debug.LogError($"WeaponFire на {gameObject.name} не нашел VehicleStats!");
            return;
        }

        // Если компонента звука нет, добавим его автоматически
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.5f; // Делаем звук частично объемным
        }

        _isPlayer = _stats.CompareTag("Player");
    }

    private void Update()
    {
        // Игрок стреляет на ЛКМ
        if (_isPlayer && Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // Проверка кулдауна и наличия данных в VehicleStats_7.cs[cite: 2, 3]
        if (Time.time < _nextFireTime || _stats == null || _stats.Weapon == null) return;

        var weapon = _stats.Weapon;

        // 1. АУДИО: Берем fireSound и volume из WeaponDataSO
        if (_audioSource != null && weapon.fireSound != null)
        {
            // Небольшой разброс высоты звука, чтобы стрельба звучала живее
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(weapon.fireSound, weapon.volume);
        }

        // 2. УРОН: Расчет на основе данных оружия[cite: 2, 3]
        bool isCritical = Random.value < weapon.criticalHitChance;
        float baseDamage = isCritical 
            ? weapon.maxDamage * weapon.criticalDamageMultiplier 
            : Random.Range(weapon.minDamage, weapon.maxDamage);

        float finalDamage = baseDamage * _stats.DamageMultiplier;

        // 3. СПАВН И НАСТРОЙКА СНАРЯДА
        if (weapon.bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);

            // Установка слоев для фильтрации коллизий
            int playerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
            int enemyProjectileLayer = LayerMask.NameToLayer("EnemyProjectile");

            bullet.layer = _isPlayer ? playerProjectileLayer : enemyProjectileLayer;

            Projectile projectileScript = bullet.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                // Передаем корень машины как владельца, чтобы не попадать в себя[cite: 3, 4]
                projectileScript.Launch(
                    finalDamage, 
                    weapon.bulletSpeed, 
                    weapon.range, 
                    _stats.gameObject, 
                    isCritical
                );
            }
        }

        // 4. КУЛДАУН: Используем FireCooldown из WeaponDataSO[cite: 2, 3]
        _nextFireTime = Time.time + weapon.FireCooldown;
    }
}