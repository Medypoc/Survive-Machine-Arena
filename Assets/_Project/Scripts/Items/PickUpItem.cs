using UnityEngine;

public abstract class PickUpItem : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private string targetTag = "Player"; // Кто может подбирать

    // Метод OnTriggerEnter2D — общий для всех предметов
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем тег у корня объекта (так как коллайдер на дочернем объекте)
        if (collision.transform.root.CompareTag(targetTag))
        {
            // Пытаемся применить уникальный эффект предмета
            // Передаем корень игрока, где лежат все компоненты (Health, Fuel)
            if (OnPickedUp(collision.transform.root.gameObject))
            {
                FinalizePickup();
            }
        }
    }

    // Этот метод каждый предмет (Топливо, Аптечка) реализует по-своему
    // Возвращает true, если предмет был успешно использован
    protected abstract bool OnPickedUp(GameObject recipient);

    private void FinalizePickup()
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }
        
        // Предмет гарантированно исчезает после использования
        Destroy(gameObject);
    }
}