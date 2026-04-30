using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider _slider;
    [SerializeField] private Health _targetHealth;

    private void Start()
    {
        // Если цель не назначена вручную, попробуем найти игрока по тегу
        if (_targetHealth == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _targetHealth = player.GetComponent<Health>();
        }

        if (_targetHealth != null)
        {
            // Подписываемся на обновление здоровья
            _targetHealth.OnHealthChanged += UpdateUI;
            UpdateUI(); // Обновляем сразу при старте
        }
    }

    private void OnDestroy()
    {
        // Обязательно отписываемся при уничтожении объекта
        if (_targetHealth != null)
            _targetHealth.OnHealthChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        if (_targetHealth != null && _slider != null)
        {
            _slider.maxValue = _targetHealth.maxHealth;
            _slider.value = _targetHealth.currentHealth;
        }
    }
}