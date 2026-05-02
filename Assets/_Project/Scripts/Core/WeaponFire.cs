using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    private VehicleStats _stats;

    [Header("References")]
    public Transform firePoint;

    private float _nextFireTime;
    private bool _isPlayer;

    private void Awake()
    {
        // Находим компонент статов. Он гарантированно висит на корне каждой машины[cite: 8].
        _stats = GetComponentInParent<VehicleStats>();
        
        if (_stats == null)
        {
            Debug.LogError($"WeaponFire на {gameObject.name} не нашел VehicleStats в родителях!");
            return;
        }

        // Проверяем тег именно того объекта, на котором висят статы[cite: 8].
        _isPlayer = _stats.CompareTag("Player");
    }

    private void Update()
    {
        // Логика стрельбы для игрока[cite: 8]
        if (_isPlayer && Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // Проверка кулдауна и наличия данных[cite: 8]
        if (Time.time < _nextFireTime || _stats == null || _stats.Weapon == null) return;

        var weapon = _stats.Weapon;

        // 1. Расчет критического урона по твоей новой формуле[cite: 8]
        bool isCritical = Random.value < weapon.criticalHitChance;
        float baseDamage;

        if (isCritical)
        {
            // Крит = Максимальный урон * Множитель из WeaponDataSO[cite: 8]
            baseDamage = weapon.maxDamage * weapon.criticalDamageMultiplier;
        }
        else
        {
            // Обычный выстрел = Рандом в диапазоне[cite: 8]
            baseDamage = Random.Range(weapon.minDamage, weapon.maxDamage);
        }

        // Учитываем общие усиления урона машины[cite: 8]
        float finalDamage = baseDamage * _stats.DamageMultiplier;

        // 2. Спавн снаряда[cite: 8]
        if (weapon.bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);

            // --- НОВАЯ ЛОГИКА: УСТАНОВКА СЛОЯ СНАРЯДА ---
            // Присваиваем слой в зависимости от того, кто стреляет.
            // Убедитесь, что слои "PlayerProjectile" и "EnemyProjectile" созданы в Unity Editor.
            int playerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
            int enemyProjectileLayer = LayerMask.NameToLayer("EnemyProjectile");

            if (_isPlayer)
            {
                bullet.layer = playerProjectileLayer;
            }
            else
            {
                bullet.layer = enemyProjectileLayer;
            }
            // --------------------------------------------

            Projectile projectileScript = bullet.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                // ПЕРЕДАЕМ ВЛАДЕЛЬЦА: используем _stats.gameObject вместо transform.root[cite: 8]
                projectileScript.Launch(
                    finalDamage, 
                    weapon.bulletSpeed, 
                    weapon.range, 
                    _stats.gameObject, 
                    isCritical
                );
            }
        }

        // 3. Установка кулдауна на основе RPM из WeaponDataSO[cite: 8]
        _nextFireTime = Time.time + weapon.FireCooldown;
    }
}