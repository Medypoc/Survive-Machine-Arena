using UnityEngine;
using System;

public class Fuel : MonoBehaviour
{
    [Header("Stats")]
    public float maxFuel;
    public float currentFuel;
    
    [Header("Consumption Tuning")]
    [Tooltip("Базовый расход топлива при максимальном газе (в секунду)")]
    public float baseConsumptionRate = 1f;

    // Событие для UI (чтобы сделать полоску бензина)
    public event Action OnFuelChanged;
    public event Action OnOutOfFuel; // Событие, если топливо кончилось

    private bool _isOutOfFuel = false;
    private VehicleStats _stats;

    private void Awake()
    {
        _stats = GetComponent<VehicleStats>();
    }

    private void Start()
    {
        // При старте даем полный бак
        if (currentFuel <= 0 && !_isOutOfFuel)
        {
            currentFuel = maxFuel;
        }
        NotifyFuelChanged();
    }

    // Этот метод будет вызывать VehicleMovement каждый кадр, когда игрок жмет газ
    public void ConsumeFuel(float gasInput)
    {
        if (_isOutOfFuel || _stats == null) return;

        // Если педаль не нажата, бензин не тратится (или можно сделать холостой ход, если нужно)
        if (Mathf.Abs(gasInput) < 0.1f) return;

        // 1. Считаем коэффициент веса. База: 1000 кг = коэф 1.0
        float totalWeight = _stats.TotalWeight;
        float weightMultiplier = totalWeight / 1000f; // 800+200 = 1000 -> коэф 1.

        // 2. Считаем итоговый расход за этот кадр
        // Берем абсолютное значение газа (и вперед, и назад тратит топливо)
        float consumptionThisFrame = baseConsumptionRate * Mathf.Abs(gasInput) * weightMultiplier * Time.fixedDeltaTime;

        // 3. Отнимаем бензин
        currentFuel -= consumptionThisFrame;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
        
        NotifyFuelChanged();

        if (currentFuel <= 0)
        {
            OutOfFuel();
        }
    }

    public void AddFuel(float amount)
    {
        if (amount <= 0) return;
        
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
        
        _isOutOfFuel = false; // Оживляем машину, если она стояла сухая
        NotifyFuelChanged();
    }

    public void NotifyFuelChanged()
    {
        OnFuelChanged?.Invoke();
    }

    private void OutOfFuel()
    {
        if (_isOutOfFuel) return;
        _isOutOfFuel = true;
        
        Debug.Log("Out of Fuel!");
        OnOutOfFuel?.Invoke();
    }
}