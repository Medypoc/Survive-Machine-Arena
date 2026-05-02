using UnityEngine;
using UnityEngine.UI;

public class FuelBarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider _slider;
    [SerializeField] private Fuel _targetFuel;

    private void Start()
    {
        // Если цель не назначена вручную, попробуем найти игрока по тегу
        if (_targetFuel == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) 
            {
                // Используем GetComponentInParent, чтобы скрипт нашел Fuel на корне игрока, 
                // даже если тег Player висит на дочернем объекте (как Cab_Slot)[cite: 5].
                _targetFuel = player.GetComponentInParent<Fuel>();
            }
        }

        if (_targetFuel != null)
        {
            // Подписываемся на обновление топлива
            _targetFuel.OnFuelChanged += UpdateUI;
            UpdateUI(); // Обновляем сразу при старте
        }
    }

    private void OnDestroy()
    {
        // Обязательно отписываемся при уничтожении объекта
        if (_targetFuel != null)
            _targetFuel.OnFuelChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        if (_targetFuel != null && _slider != null)
        {
            // Синхронизируем значения слайдера с данными из Fuel
            _slider.maxValue = _targetFuel.maxFuel;
            _slider.value = _targetFuel.currentFuel;
        }
    }
}