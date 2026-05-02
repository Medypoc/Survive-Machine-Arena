using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private VehicleStats _stats;
    private Fuel _fuel; 

    [Header("Grip & Handbrake")]
    [Range(0, 1)] public float defaultDriftFactor = 0.95f; 
    [Range(0, 1)] public float handbrakeDriftFactor = 0.99f; // При ручнике боковая скорость почти не гасится (скольжение)
    public float handbrakeSteerMultiplier = 1.5f; // Руль становится резче при заносе
    
    [Header("Dash Settings")]
    public float dashForceMultiplier = 3f; // Сила рывка (умножается на базовое ускорение)
    public float dashCooldown = 3f;
    private float _lastDashTime = -10f; // Чтобы можно было сделать рывок сразу со старта

    private float _currentDriftFactor;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _stats = GetComponent<VehicleStats>();
        _fuel = GetComponent<Fuel>(); 
        _currentDriftFactor = defaultDriftFactor;
    }

    void FixedUpdate()
    {
        ApplyLateralFriction();
    }

    // Добавили параметр handbrake
    public void Move(float gasInput, float steerInput, bool isHandbrake)
    {
        if (_stats == null) return;

        if (_fuel != null && _fuel.currentFuel <= 0)
        {
            gasInput = 0f; 
            steerInput = 0f; 
        }

        // 1. Логика ручника
        _currentDriftFactor = isHandbrake ? handbrakeDriftFactor : defaultDriftFactor;
        
        // Если зажат ручник: режем газ в 2 раза (машина не может полноценно разгоняться) и делаем руль острее
        float appliedGas = isHandbrake ? gasInput * 0.5f : gasInput;
        float appliedSteer = isHandbrake ? steerInput * handbrakeSteerMultiplier : steerInput;

        // 2. Движение вперед[cite: 9]
        _rb.AddForce(transform.up * appliedGas * _stats.Acceleration);

        // 3. Поворот[cite: 9]
        float speedFactor = Mathf.Clamp01(_rb.linearVelocity.magnitude / 5f);
        _rb.AddTorque(appliedSteer * _stats.SteeringSpeed * speedFactor * -1f);

        // 4. Трата топлива[cite: 9]
        if (_fuel != null)
        {
            // Передаем реальный газ (даже если мы на ручнике, бензин всё равно горит)
            _fuel.ConsumeFuel(gasInput);
        }
    }

    public void Dash()
    {
        // Не делаем рывок, если нет статов или бак пуст
        if (_stats == null || (_fuel != null && _fuel.currentFuel <= 0)) return;

        // Проверка кулдауна
        if (Time.time >= _lastDashTime + dashCooldown)
        {
            // Используем ForceMode2D.Impulse для моментального, резкого толчка
            _rb.AddForce(transform.up * _stats.Acceleration * dashForceMultiplier, ForceMode2D.Impulse);
            
            // Запоминаем время рывка
            _lastDashTime = Time.time;
            
            // Опционально: Рывок сжигает фиксированное количество топлива (как 1 секунда езды)
            if (_fuel != null) _fuel.ConsumeFuel(1f); 
        }
    }

    private void ApplyLateralFriction()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(_rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(_rb.linearVelocity, transform.right);
        
        // Используем динамический фактор дрифта[cite: 9]
        _rb.linearVelocity = forwardVelocity + rightVelocity * _currentDriftFactor;
    }
}