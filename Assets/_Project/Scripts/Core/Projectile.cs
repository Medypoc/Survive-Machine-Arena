using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private float damage;
    private float maxRange;
    
    // Новая переменная для хранения ссылки на того, кто выстрелил
    private GameObject _owner;

    public void Launch(float dmg, float spd, float range, GameObject owner)
    {
        rb = GetComponent<Rigidbody2D>();
        damage = dmg;
        maxRange = range;
        _owner = owner; // Запоминаем хозяина
        startPosition = transform.position;

        if (rb != null)
        {
            rb.linearVelocity = transform.up * spd;
        }
        
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ПРОВЕРКА 1: Игнорируем того, кто выпустил пулю (проверяем корень объекта)
        if (_owner != null && other.transform.root == _owner.transform) return;

        // ПРОВЕРКА 2: Игнорируем другие пули
        if (other.GetComponent<Projectile>() != null) return;

        // Наносим урон
        Health targetHealth = other.GetComponentInParent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);

            // РЕАЛИЗАЦИЯ ПРИОРИТЕТА 1: Если попали во врага, он должен захотеть отомстить
            AIController ai = other.GetComponentInParent<AIController>();
            if (ai != null && _owner != null)
            {
                ai.SetAggressor(_owner.transform);
            }
        }

        Destroy(gameObject);
    }
}