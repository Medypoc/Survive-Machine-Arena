using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemySpawnZone : MonoBehaviour
{
    private BoxCollider2D _zoneCollider;

    private void Awake()
    {
        _zoneCollider = GetComponent<BoxCollider2D>();
    }

    // Метод возвращает случайную точку внутри границ коллайдера
    public Vector2 GetRandomPointInZone()
    {
        Bounds bounds = _zoneCollider.bounds;
        
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        
        return new Vector2(randomX, randomY);
    }

    // Отрисовка зоны в редакторе для удобства (не видна в игре)
    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}