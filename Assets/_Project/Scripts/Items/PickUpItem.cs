using UnityEngine;

public abstract class PickUpItem : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private string targetTag = "Player"; // Кто может подбирать

    // Метод OnTriggerEnter2D — общий для всех предметов
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[PickUp] В предмет въехал коллайдер: {collision.gameObject.name}");

        Rigidbody2D rb = collision.attachedRigidbody;
        
        if (rb == null)
        {
            Debug.LogWarning("[PickUp] ОШИБКА 1: У въехавшего объекта нет Rigidbody2D на корне!");
            return;
        }

        Debug.Log($"[PickUp] Нашел корень: {rb.gameObject.name}, его тег: {rb.tag}");

        if (rb.CompareTag(targetTag))
        {
            if (OnPickedUp(rb.gameObject))
            {
                Debug.Log("[PickUp] УСПЕХ: Предмет подобран и применен!");
                FinalizePickup();
            }
            else
            {
                Debug.LogWarning($"[PickUp] ОШИБКА 3: OnPickedUp вернул false! Скорее всего на {rb.gameObject.name} нет скрипта Fuel или Health.");
            }
        }
        else
        {
            Debug.LogWarning($"[PickUp] ОШИБКА 2: Тег не совпал! Ждали '{targetTag}', а приехал '{rb.tag}'");
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