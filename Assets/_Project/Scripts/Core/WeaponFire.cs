using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    private VehicleStats _stats;
    
    [Header("References")]
    public Transform firePoint;
    
    private float _nextFireTime;

    void Start()
    {
        // Ищем Stats в родительском объекте машины
        _stats = GetComponentInParent<VehicleStats>();
    }

    void Update()
    {
        // Если это игрок, он управляет стрельбой сам (ЛКМ)
        // Если это ИИ, метод Shoot() будет вызываться из AIController
        if (transform.root.CompareTag("Player"))
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
    }

    public void Shoot()
    {
        if (_stats == null || _stats.Weapon == null) return;

        if (Time.time >= _nextFireTime)
        {
            // Создаем снаряд
            GameObject bullet = Instantiate(_stats.Weapon.bulletPrefab, firePoint.position, firePoint.rotation);
            
            Projectile projectileScript = bullet.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Launch(_stats.Weapon.damage, _stats.Weapon.bulletSpeed, _stats.Weapon.range);
            }

            // ИСПРАВЛЕННАЯ ЛОГИКА:
            // Если FireRate — это выстрелы в минуту (например, 300)
            float delayBetweenShots = 60f / _stats.Weapon.fireRate; 
            
            // Если же ты хочешь выстрелы в секунду (например, 5 выстрелов в сек), используй:
            // float delayBetweenShots = 1f / _stats.Weapon.fireRate;

            _nextFireTime = Time.time + delayBetweenShots;
        }
    }
}