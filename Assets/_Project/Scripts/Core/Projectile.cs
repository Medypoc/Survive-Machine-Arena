using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 startPosition;
    
    // Эти переменные хранят данные внутри пули после вылета
    private float damage; 
    private float maxRange;

    /// <summary>
    /// Метод инициализации пули при выстреле
    /// </summary>
    public void Launch(float dmg, float spd, float range)
    {
        // 1. Находим Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        
        // 2. Сохраняем данные из WeaponDataSO в память пули
        damage = dmg; 
        maxRange = range;
        startPosition = transform.position;

        // 3. Даем пуле физический импульс
        if (rb != null)
        {
            // transform.up — это направление ствола (ось Y)
            rb.linearVelocity = transform.up * spd;
        }
        else
        {
            Debug.LogError($"На префабе {gameObject.name} отсутствует компонент Rigidbody2D!");
        }
        
        // Резервный таймер жизни (на случай если пуля улетит в бесконечность и не встретит преград)
        Destroy(gameObject, 10f); 
    }

    void Update()
    {
        // Логика ограничения дальности стрельбы
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Самая важная проверка: если это игрок — ВООБЩЕ ничего не делаем
        if (other.CompareTag("Player")) return;

        // 2. Если попали во что-то другое (врага, стену)
        Health targetHealth = other.GetComponentInParent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        // 3. Уничтожаем пулю, если это не другая пуля
        if (other.GetComponent<Projectile>() == null)
        {
            Destroy(gameObject);
        }
    }
}