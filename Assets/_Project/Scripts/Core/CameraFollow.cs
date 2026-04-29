using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;        // Твоя машина (Player)
    public float smoothTime = 0.2f; // Время сглаживания (чем меньше, тем быстрее камера)
    
    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0, 0, -10f); // Дистанция. Z обязательно -10

    private Vector3 currentVelocity = Vector3.zero;

    // Используем LateUpdate, чтобы камера двигалась ПОСЛЕ того, как машина переместилась в FixedUpdate
    void LateUpdate()
    {
        if (target == null) return;

        // Определяем, где камера ДОЛЖНА быть
        Vector3 targetPosition = target.position + offset;

        // Плавно перемещаем камеру к цели
        // SmoothDamp сам рассчитывает ускорение и замедление для идеальной плавности
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // ПРИНУДИТЕЛЬНО фиксируем поворот камеры, чтобы она никогда не вращалась
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}