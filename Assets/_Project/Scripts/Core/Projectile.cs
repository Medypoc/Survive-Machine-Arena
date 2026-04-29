using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 startPosition;
    
    // ДОБАВЛЕНО: Эти переменные хранят данные внутри пули после вылета
    private float damage; 
    private float maxRange;

    public void Launch(float dmg, float spd, float range)
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Сохраняем полученные данные в переменные класса
        damage = dmg; 
        maxRange = range;
        startPosition = transform.position;

        if (rb != null)
        {
            rb.linearVelocity = transform.up * spd;
        }
        
        Destroy(gameObject, 10f); // Резервный таймер
    }

    void Update()
    {
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Теперь переменная 'damage' существует в текущем контексте
        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        if (!other.CompareTag("Player") && !other.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}