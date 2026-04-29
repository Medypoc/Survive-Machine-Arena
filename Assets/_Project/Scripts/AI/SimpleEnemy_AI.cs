using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Поиск игрока по ТЕГУ (убедись, что у игрока стоит тег Player)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
            player = playerObj.transform;
        else
            Debug.LogError("Враг не нашел объект с тегом Player!");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 1. Считаем направление
        Vector2 direction = (player.position - transform.position).normalized;

        // 2. Двигаем врага напрямую в сторону игрока
        // Это исключает проблему "неправильного переда" спрайта
        rb.linearVelocity = direction * speed;

        // 3. Поворачиваем спрайт (визуал) лицом к игроку
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(angle);
    }
}