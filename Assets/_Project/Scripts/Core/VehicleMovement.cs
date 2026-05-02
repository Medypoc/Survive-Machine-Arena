using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private VehicleStats _stats;
    private Fuel _fuel; 

    [Header("Grip & Handbrake")]
    [Range(0, 1)] public float defaultDriftFactor = 0.95f; 
    [Range(0, 1)] public float handbrakeDriftFactor = 0.99f; 
    public float handbrakeSteerMultiplier = 1.5f; 
    
    [Header("Dash Settings")]
    public float dashForceMultiplier = 3f;
    public float dashCooldown = 3f;
    public float dashFuelCost = 15f; // Стоимость рывка (настрой в Инспекторе)
    private float _lastDashTime = -10f;

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

    public void Move(float gasInput, float steerInput, bool isHandbrake)
    {
        if (_stats == null) return;

        if (_fuel != null && _fuel.currentFuel <= 0)
        {
            gasInput = 0f; 
            steerInput = 0f; 
        }

        _currentDriftFactor = isHandbrake ? handbrakeDriftFactor : defaultDriftFactor;
        
        float appliedGas = isHandbrake ? gasInput * 0.5f : gasInput;
        float appliedSteer = isHandbrake ? steerInput * handbrakeSteerMultiplier : steerInput;

        _rb.AddForce(transform.up * appliedGas * _stats.Acceleration);

        float speedFactor = Mathf.Clamp01(_rb.linearVelocity.magnitude / 5f);
        _rb.AddTorque(appliedSteer * _stats.SteeringSpeed * speedFactor * -1f);

        if (_fuel != null)
        {
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
            
            // Мгновенное списание топлива за рывок
            if (_fuel != null) 
            {
                _fuel.currentFuel -= dashFuelCost;
                
                // Защита от отрицательного значения в UI
                if (_fuel.currentFuel < 0) 
                {
                    _fuel.currentFuel = 0;
                }
                
                // Оповещаем шкалу интерфейса об изменении
                _fuel.NotifyFuelChanged();
            }
        }
    }

    private void ApplyLateralFriction()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(_rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(_rb.linearVelocity, transform.right);
        
        _rb.linearVelocity = forwardVelocity + rightVelocity * _currentDriftFactor;
    }
}