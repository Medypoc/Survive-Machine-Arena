using UnityEngine;

public class CarController : MonoBehaviour
{
    private VehicleStats stats;
    private Rigidbody2D rb;

    private float moveInput;
    private float steerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<VehicleStats>();
    }

    void FixedUpdate()
    {
        if (stats == null || stats.currentFuel <= 0) return;

        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        ApplyEngineForce();
        ApplySteering();
        ApplyDriftAndSpeedLimit();
    }

    // 1. Двигатель (Только толкает вперед/назад)
    void ApplyEngineForce()
    {
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            float currentAccel = (moveInput > 0) ? stats.acceleration : stats.acceleration / 2f;
            
            // Прикладываем постоянную силу. Множитель 50f - базовый, при массе 1.
            rb.AddForce(transform.up * moveInput * currentAccel * 50f);
        }
        else
        {
            // Торможение двигателем, когда газ отпущен
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * 2f);
        }
    }

    // 2. Руль (Четкий аркадный поворот без "волчка")
    void ApplySteering()
    {
        // Узнаем текущую скорость по направлению кабины
        float forwardVelocity = Vector2.Dot(rb.linearVelocity, transform.up);
        
        // Машина не должна поворачивать, если стоит на месте
        if (Mathf.Abs(forwardVelocity) > 0.5f)
        {
            // Едем назад = руль инвертируется (чтобы задница ехала в сторону поворота)
            float direction = forwardVelocity > 0 ? 1f : -1f;

            // Высчитываем градус поворота в этом кадре
            float turnAmount = steerInput * stats.cabData.steeringSpeed * direction * 0.5f;

            // Вращаем сам объект жестко, игнорируя инерцию
            rb.MoveRotation(rb.rotation - turnAmount);
        }
    }

    // 3. Сцепление с дорогой и ограничение скорости (Убирает рывки)
    void ApplyDriftAndSpeedLimit()
    {
        float currentMaxSpeed = (moveInput > 0) ? stats.maxSpeed : stats.maxSpeed / 2f;

        // Разделяем скорость на ту, что направлена ВПЕРЕД, и ту, что ВБОК
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);

        // Жестко обрезаем скорость вперед, если она превысила максимум (никаких рывков)
        if (forwardVelocity.magnitude > currentMaxSpeed)
        {
            forwardVelocity = forwardVelocity.normalized * currentMaxSpeed;
        }

        // Складываем векторы обратно. Множитель 0.9f гасит боковой занос.
        // Если поставить 0.0f - машина будет ехать как по рельсам.
        rb.linearVelocity = forwardVelocity + rightVelocity * 0.9f;
    }
}