using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthRing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private Image _ringImage;

    [Header("Settings")]
    public bool keepRotationFixed = true; // Чтобы круг не крутился вместе с машиной

    private void Start()
    {
        // Автоматически ищем компонент Health на родительском объекте
        if (_health == null) _health = GetComponentInParent<Health>();

        if (_health != null)
        {
            _health.OnHealthChanged += UpdateRing;
            UpdateRing(); // Инициализация при спавне
        }
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.OnHealthChanged -= UpdateRing;
    }

    private void LateUpdate()
    {
        // Если машина вращается, канвас будет вращаться вместе с ней.
        // Этот код заставляет кольцо всегда "смотреть" прямо, чтобы анимация убывания не кружилась.
        if (keepRotationFixed)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void UpdateRing()
    {
        if (_health != null && _ringImage != null && _health.maxHealth > 0)
        {
            // Поскольку currentHealth и maxHealth у тебя типа float, 
            // мы можем просто разделить их для получения значения от 0 до 1
            _ringImage.fillAmount = _health.currentHealth / _health.maxHealth;
        }
    }
}