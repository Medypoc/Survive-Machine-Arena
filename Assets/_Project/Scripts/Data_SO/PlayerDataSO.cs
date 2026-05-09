using UnityEngine;
using SurviveArena.Data;

[CreateAssetMenu(fileName = "NewPlayerProfile", menuName = "SurviveArena/Player Profile")]
public class PlayerDataSO : ScriptableObject
{
    // --- НОВОЕ: Ссылка на базовый тип машины ---
    [Header("Current Vehicle Type")]
    public VehicleClassSO selectedVehicleClass; 

    [Header("Economy & Progression")]
    public int money;
    public int currentXP;
    public int currentRank = 1;

    [Header("Runtime Status (Current State)")]
    public float currentHealth;
    public float currentFuel;

    // --- БЛОК: ЭКИПИРОВКА ---
    [Header("Equipped Parts")]
    public BodyDataSO equippedBody;
    public CabDataSO equippedCab;
    public WeaponDataSO equippedWeapon;
    // ------------------------------

    public float GetTotalMaxHealth()
    {
        // Берем базу не из константы, а из типа машины
        float totalMaxHealth = selectedVehicleClass != null ? selectedVehicleClass.baseHealth : 100f;
        
        if (equippedBody != null) totalMaxHealth += equippedBody.additionalHP;
        if (equippedCab != null) totalMaxHealth += equippedCab.additionalHP;

        return totalMaxHealth;
    }

    public float GetTotalMaxFuel()
    {
        // Берем базовое топливо из типа машины
        float totalMaxFuel = selectedVehicleClass != null ? selectedVehicleClass.baseFuel : 0f;
        
        // Топливо находится в кузове (BodyDataSO), поэтому проверяем именно его
        if (equippedBody != null) totalMaxFuel += equippedBody.fuelCapacity; 

        return totalMaxFuel;
    }

    public void ResetToMaxStatus()
    {
        currentHealth = GetTotalMaxHealth();
        currentFuel = GetTotalMaxFuel();
    }

    public int GetXPForNextRank() 
    {
        return currentRank * 500;
    }

    public void AddExperience(int amount)
    {
        currentXP += amount;
        while (currentXP >= GetXPForNextRank())
        {
            currentXP -= GetXPForNextRank();
            currentRank++;
        }
    }
}