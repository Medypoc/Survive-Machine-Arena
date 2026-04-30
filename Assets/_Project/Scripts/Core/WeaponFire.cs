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
        _stats = GetComponentInParent<VehicleStats>();
        _isPlayer = transform.root.CompareTag("Player");
    }

    private void Update()
    {
        // Player controls shooting with LMB. AI calls Shoot() from controller.
        if (_isPlayer && Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (_stats == null || _stats.Weapon == null || firePoint == null)
        {
            return;
        }

        if (Time.time < _nextFireTime)
        {
            return;
        }

        if (_stats.Weapon.bulletPrefab == null)
        {
            return;
        }

        GameObject bullet = Instantiate(_stats.Weapon.bulletPrefab, firePoint.position, firePoint.rotation);

        int bulletLayer = _isPlayer
            ? LayerMask.NameToLayer("PlayerProjectile")
            : LayerMask.NameToLayer("EnemyProjectile");

        if (bulletLayer >= 0)
        {
            bullet.layer = bulletLayer;
        }

        Projectile projectileScript = bullet.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            float finalDamage = _stats.Weapon.damage * _stats.DamageMultiplier;
            projectileScript.Launch(finalDamage, _stats.Weapon.bulletSpeed, _stats.Weapon.range);
        }

        // fireRate is rounds per minute; protect from zero or negative values.
        float fireRate = Mathf.Max(0.01f, _stats.Weapon.fireRate);
        _nextFireTime = Time.time + (60f / fireRate);
    }
}