using UnityEngine;

public class WeaponFire: MonoBehaviour
{
    private VehicleStats stats;
    
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint; // Пустой объект на кончике ствола

    private float nextFireTime;

    void Start()
    {
        stats = GetComponentInParent<VehicleStats>();
    }

    void Update()
    {
        if (stats == null || stats.weaponData == null) return;

        // Проверка нажатия кнопки и кулдауна (Rate of Fire)
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            // Рассчитываем время до следующего выстрела
            // Если fireRate = 600, то это 10 выстрелов в сек (интервал 0.1с)
            nextFireTime = Time.time + 60f / stats.weaponData.fireRate;
        }
    }

    void Shoot()
{
    if (bulletPrefab == null || firePoint == null) return;

    GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    Projectile proj = bulletObj.GetComponent<Projectile>();

    if (proj != null)
    {
        // Передаем ТРИ параметра: урон, скорость и ДАЛЬНОСТЬ
        proj.Launch(
            stats.weaponData.damage, 
            stats.weaponData.bulletSpeed, 
            stats.weaponData.range
        );
    }
}
}