using UnityEngine;

public class PlayerEquipmentLoader : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private PlayerDataSO _playerProfile;

    private void Start()
    {
        if (_playerProfile == null) return;

        // Ищем компонент VehicleStats на нашей машине
        VehicleStats stats = GetComponent<VehicleStats>();
        
        if (stats != null)
        {
            // Передаем сохраненные детали в метод загрузки
            stats.LoadModules(
                _playerProfile.equippedBody, 
                _playerProfile.equippedCab, 
                _playerProfile.equippedWeapon
            );

            // Если у тебя здоровье зависит от деталей, стоит обновить его
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.maxHealth = stats.MaxHealth; // Подставь свою переменную из VehicleStats
                health.currentHealth = health.maxHealth;
                health.NotifyHealthChanged();
            }
        }
    }
}