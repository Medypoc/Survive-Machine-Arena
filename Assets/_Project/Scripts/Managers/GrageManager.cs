using UnityEngine;
using SurviveArena.Data;

public class GarageManager : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private PlayerDataSO _playerProfile;

    [Header("Settings")]
    [Tooltip("Стоимость ремонта за 1 единицу HP")]
    [SerializeField] private float _repairCostCoefficient = 2f; 

    // Метод для расчета стоимости
    public int GetRepairCost()
    {
        if (_playerProfile == null) return 0;

        // Разница между Max и Current
        float missingHP = _playerProfile.GetTotalMaxHealth() - _playerProfile.currentHealth; 
        
        // Округляем в большую сторону
        return Mathf.CeilToInt(Mathf.Max(0, missingHP * _repairCostCoefficient));
    }

    // Метод выполнения ремонта
    public bool TryRepair()
    {
        int cost = GetRepairCost();

        if (_playerProfile.money >= cost && cost > 0)
        {
            _playerProfile.money -= cost; // Списываем деньги
            _playerProfile.currentHealth = _playerProfile.GetTotalMaxHealth(); // Восстанавливаем HP[cite: 9]
            return true;
        }

        return false;
    }
}