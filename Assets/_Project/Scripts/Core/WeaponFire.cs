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
        // Ищем VehicleStats в родителе (на самой машине)
        _stats = GetComponentInParent<VehicleStats>();
        
        // Проверяем, является ли эта машина игроком
        _isPlayer = transform.root.CompareTag("Player");
    }

    private void Update()
    {
        // Если это игрок — стреляем при нажатии ЛКМ
        if (_isPlayer && Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // Проверки на наличие всех необходимых данных
        if (_stats == null || _stats.Weapon == null || firePoint == null) return;

        // Проверка кулдауна
        if (Time.time < _nextFireTime) return;

        if (_stats.Weapon.bulletPrefab == null) return;

        // 1. Получаем готовый урон из VehicleStats (уже с учетом разброса, крита и множителей)
        float finalDamage = _stats.CalculateAttackDamage(out bool isCritical);

        // 2. Создаем снаряд
        GameObject bullet = Instantiate(_stats.Weapon.bulletPrefab, firePoint.position, firePoint.rotation);

        // 3. Устанавливаем слой пули, чтобы она не попадала в того, кто выстрелил
        int bulletLayer = _isPlayer
            ? LayerMask.NameToLayer("PlayerProjectile")
            : LayerMask.NameToLayer("EnemyProjectile");

        if (bulletLayer >= 0)
        {
            bullet.layer = bulletLayer;
        }

        // 4. Инициализируем полет снаряда
        Projectile projectileScript = bullet.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Launch(
                finalDamage, 
                _stats.Weapon.bulletSpeed, 
                _stats.Weapon.range, 
                transform.root.gameObject, 
                isCritical // Передаем флаг крита для красивых цифр урона
            );
        }

        // 5. Рассчитываем время следующего выстрела на основе RPM
        // Используем Mathf.Max, чтобы не получить бесконечную паузу, если в данных стоит 0
        float rpm = Mathf.Max(1f, _stats.Weapon.fireRateRPM);
        _nextFireTime = Time.time + (60f / rpm);
    }
}