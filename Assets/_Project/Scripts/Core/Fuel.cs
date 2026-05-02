using UnityEngine;
using System;

public class Fuel : MonoBehaviour
{
    public event Action OnFuelChanged;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float currentFuel;
    
    [Header("Death Timer (No Fuel)")]
    [SerializeField] private float _timeToDieWithoutFuel = 10f;
    private float _currentDeathTimer;
    private bool _isCountingDown = false;

    private Health _playerHealth;

    private void Awake()
    {
        _playerHealth = GetComponent<Health>();
        currentFuel = maxFuel;
    }

    private void Update()
    {
        // Проверка критического состояния
        if (currentFuel <= 0 && !_isCountingDown)
        {
            StartDeathTimer();
        }
        else if (currentFuel > 0 && _isCountingDown)
        {
            StopDeathTimer();
        }

        if (_isCountingDown)
        {
            HandleDeathCountdown();
        }
    }

    // ИСПРАВЛЕНИЕ ОШИБКИ CS1061: Добавляем метод потребления топлива
    public void ConsumeFuel(float amount)
    {
        if (amount <= 0) return;

        currentFuel -= amount * Time.deltaTime;
        // Не даем упасть ниже нуля
        currentFuel = Mathf.Max(currentFuel, 0); 
        // Уведомляем UI только если значение изменилось
        NotifyFuelChanged();
    }

    public void AddFuel(float amount)
    {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0, maxFuel);
        NotifyFuelChanged();
    }

    public void NotifyFuelChanged()
    {
        OnFuelChanged?.Invoke();
    }

    private void StartDeathTimer()
    {
        _isCountingDown = true;
        _currentDeathTimer = _timeToDieWithoutFuel;
    }

    private void StopDeathTimer()
    {
        _isCountingDown = false;
    }

    private void HandleDeathCountdown()
    {
        // Теперь Fuel видит IsVictory в BattleManager
        if (BattleManager.Instance != null && BattleManager.Instance.IsVictory)
        {
            _isCountingDown = false;
            return;
        }

        _currentDeathTimer -= Time.deltaTime;

        if (_currentDeathTimer <= 0)
        {
            _isCountingDown = false;
            DieFromLackOfFuel();
        }
    }

    private void DieFromLackOfFuel()
    {
        if (_playerHealth != null)
        {
            _playerHealth.TakeDamage(9999f, false, null);
        }
    }
}