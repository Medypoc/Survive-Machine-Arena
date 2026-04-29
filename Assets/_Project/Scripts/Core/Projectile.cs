using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private float maxRange;

    public void Launch(float dmg, float spd, float range)
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // Запоминаем точку вылета
        maxRange = range;

        if (rb != null)
        {
            // Убеждаемся, что пуля летит вперед (вверх по локальной оси Y)
            rb.linearVelocity = transform.up * spd;
        }
        else
        {
            Debug.LogError("Projectile: На префабе пули нет Rigidbody2D!");
        }
    }

    void Update()
    {
        // Проверяем пройденное расстояние
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxRange)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Игнорируем игрока и другие пули (если нужно)
        if (other.CompareTag("Player") || other.CompareTag("Projectile")) return;

        Debug.Log("Hit: " + other.name);
        DestroyProjectile();
    }

    void DestroyProjectile()
    {
        // Здесь в будущем можно создать эффект вспышки/взрыва
        Destroy(gameObject);
    }
}