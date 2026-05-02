using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _damage;
    private bool _isCritical;
    private GameObject _owner;

    public void Launch(float damage, float speed, float range, GameObject owner, bool isCritical)
    {
        _damage = damage;
        _isCritical = isCritical;
        _owner = owner;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
        
        Destroy(gameObject, range / speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Ищем хитбокс конкретной части
        VehiclePartHitbox hitbox = collision.GetComponentInParent<VehiclePartHitbox>();
        
        // Определяем машину (либо через хитбокс, либо через поиск в родителях)[cite: 7]
        VehicleStats hitVehicle = hitbox != null ? hitbox.ownerStats : collision.GetComponentInParent<VehicleStats>();

        // 2. Проверка: не попали ли мы в себя[cite: 7]
        if (hitVehicle != null && hitVehicle.gameObject == _owner) return;

        float damageToDeal = _damage;

        // 3. Если попали в конкретный хитбокс — применяем броню этой части
        if (hitbox != null && hitVehicle != null)
        {
            float partArmor = 0f;

            if (hitbox.partType == VehiclePartType.Cab && hitVehicle.Cab != null)
            {
                partArmor = hitVehicle.Cab.armor; // Берем броню из CabDataSO[cite: 6, 8]
            }
            else if (hitbox.partType == VehiclePartType.Body && hitVehicle.Body != null)
            {
                partArmor = hitVehicle.Body.armor; // Берем броню из BodyDataSO[cite: 6, 8]
            }

            // Вычитаем процент урона, равный проценту брони (броня 20 = -20% урона)
            float armorModifier = Mathf.Clamp(partArmor / 100f, 0f, 1f);
            damageToDeal *= (1f - armorModifier);
        }

        // 4. Нанесение урона компоненту Health[cite: 6, 7]
        Health health = collision.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damageToDeal, _isCritical, _owner);
            Destroy(gameObject); 
        }
        else if (!collision.isTrigger)
        {
            // Попадание в стену или препятствие[cite: 7]
            Destroy(gameObject);
        }
    }
}