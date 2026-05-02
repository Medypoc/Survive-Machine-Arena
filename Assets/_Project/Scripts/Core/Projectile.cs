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

        // Лог запуска: проверяем, кто стреляет и с какими параметрами
        Debug.Log($"[Projectile] Выстрел! Владелец: {(_owner != null ? _owner.name : "NULL")}, Урон: {_damage}, Крит: {_isCritical}");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
        
        Destroy(gameObject, range / speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Фиксируем сам факт любого физического контакта
        Debug.Log($"[Projectile] Контакт с объектом: {collision.name}, Слой: {LayerMask.LayerToName(collision.gameObject.layer)}");

        // 2. Проверка иерархии
        VehicleStats hitVehicle = collision.GetComponentInParent<VehicleStats>();
        GameObject hitRoot = collision.transform.root.gameObject;

        // Логируем проверку владельца
        if (hitVehicle != null)
        {
            Debug.Log($"[Projectile] Найдена машина: {hitVehicle.name}. Сравнение с владельцем: {(hitVehicle.gameObject == _owner ? "СВОЙ (Игнор)" : "ЧУЖОЙ (Атака)")}");
            
            if (hitVehicle.gameObject == _owner) return;
        }
        else
        {
            Debug.Log($"[Projectile] В объекте {collision.name} НЕ найден VehicleStats в родителях. Проверка по Root: {(hitRoot == _owner ? "СВОЙ" : "ЧУЖОЙ")}");
            if (hitRoot == _owner) return;
        }

        // 3. Поиск здоровья
        Health health = collision.GetComponentInParent<Health>();
        
        if (health != null)
        {
            Debug.Log($"[Projectile] Здоровье найдено на {health.gameObject.name}. Наносим урон!");
            health.TakeDamage(_damage, _isCritical, _owner);
            Destroy(gameObject); 
        }
        else
        {
            Debug.LogWarning($"[Projectile] Попал в {collision.name}, но компонент Health не найден ни в объекте, ни в родителях!");
            
            if (!collision.isTrigger)
            {
                Debug.Log("[Projectile] Попадание в стену или статический объект. Уничтожение.");
                Destroy(gameObject);
            }
        }
    }
}