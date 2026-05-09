using UnityEngine;
using SurviveArena.Data;

public class RepairModule : MonoBehaviour
{
    [Header("Данные")]
    [SerializeField] private PlayerDataSO playerProfile;
    [SerializeField] private VehicleStats hubVehicleStats;

    [Header("Настройки стоимости")]
    [Tooltip("Цена за 1 единицу восстановленного здоровья")]
    [SerializeField] private int pricePerUnit = 2;

    /// <summary>
    /// Проверяет, нужно ли вообще чинить машину.
    /// </summary>
    public bool NeedsRepair()
    {
        if (hubVehicleStats == null) return false;
        return playerProfile.currentHealth < hubVehicleStats.MaxHealth;
    }

    /// <summary>
    /// Рассчитывает стоимость ремонта на основе текущего состояния.
    /// </summary>
    public int GetRepairCost()
    {
        if (hubVehicleStats == null) 
        {
            Debug.LogError("RepairModule: Не назначена ссылка на hubVehicleStats!");
            return 0;
        }

        float maxH = hubVehicleStats.MaxHealth;
        float currentH = playerProfile.currentHealth;
        float missing = maxH - currentH;

        return (missing > 0) ? Mathf.CeilToInt(missing * pricePerUnit) : 0;
    }

    /// <summary>
    /// Физически восстанавливает здоровье в профиле до максимума.
    /// </summary>
    public void RestoreHealth()
    {
        if (hubVehicleStats != null)
        {
            playerProfile.currentHealth = hubVehicleStats.MaxHealth;
            Debug.Log("[RepairModule] Здоровье игрока успешно восстановлено.");
        }
    }
}