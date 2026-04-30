using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private VehicleStats _stats;

    [Header("Grip Settings")]
    [Range(0, 1)] public float driftFactor = 0.95f; // Чем ниже, тем меньше заносит

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _stats = GetComponent<VehicleStats>();
    }

    void FixedUpdate()
    {
        ApplyLateralFriction();
    }

    public void Move(float gasInput, float steerInput)
    {
        if (_stats == null) return;

        // Движение вперед
        _rb.AddForce(transform.up * gasInput * _stats.Acceleration);

        // Поворот (зависит от текущей скорости, чтобы не крутиться на месте)
        float speedFactor = Mathf.Clamp01(_rb.linearVelocity.magnitude / 5f);
        _rb.AddTorque(steerInput * _stats.SteeringSpeed * speedFactor * -1f);
    }

    private void ApplyLateralFriction()
    {
        // Вычисляем "боковую" скорость (насколько нас несет вправо или влево)
        Vector2 forwardVelocity = transform.up * Vector2.Dot(_rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(_rb.linearVelocity, transform.right);

        // Умножаем боковую скорость на фактор дрифта. 
        // Если driftFactor = 0, боковая скорость полностью гасится каждый кадр.
        _rb.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }
}