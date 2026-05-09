using UnityEngine;

public enum VehiclePartType { Body, Cab }

public class VehiclePartHitbox : MonoBehaviour
{
    [Tooltip("Укажите, что это за деталь")]
    public VehiclePartType partType;

    private Health _rootHealth;
    private VehicleStats _rootStats;

    private void Start()
    {
        // Ищем главные компоненты один раз при старте
        _rootHealth = GetComponentInParent<Health>();
        _rootStats = GetComponentInParent<VehicleStats>();

        if (_rootHealth == null)
        {
            Debug.LogError($"Хитбокс {gameObject.name} не нашел скрипт Health на корневом объекте!");
        }
    }

    // Этот метод вызывает пуля при попадании
    public void TakeHit(float rawDamage, bool isCritical, GameObject attacker)
    {
        if (_rootHealth == null || _rootStats == null) return;

        float partArmor = 0f;

        // 1. Узнаем броню конкретной детали
        if (partType == VehiclePartType.Body && _rootStats.Body != null)
        {
            partArmor = _rootStats.Body.armor; 
        }
        else if (partType == VehiclePartType.Cab && _rootStats.Cab != null)
        {
            partArmor = _rootStats.Cab.armor;
        }

        // 2. ВАША ОРИГИНАЛЬНАЯ ЛОГИКА БАЛАНСА
        // Вычитаем процент урона (например, броня 20 = -20% урона)
        float armorModifier = Mathf.Clamp(partArmor / 100f, 0f, 1f);
        float finalDamage = rawDamage * (1f - armorModifier);

        // 3. Передаем итоговый (срезанный) урон в главный скрипт здоровья
        _rootHealth.TakeDamage(finalDamage, isCritical, attacker);
    }
}