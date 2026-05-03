using UnityEngine;
using SurviveArena.Data;

[CreateAssetMenu(fileName = "NewPlayerProfile", menuName = "SurviveArena/Player Profile")]
public class PlayerDataSO : ScriptableObject
{
    // Базовые константы пустой машины
    private const float BASE_HEALTH = 100f;
    private const float BASE_FUEL = 0f; // Базовое значение топлива (можно изменить по необходимости)

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
        float totalMaxHealth = BASE_HEALTH;
        
        // Предполагается, что в BodyDataSO и CabDataSO есть поле healthadditional или аналогичное
        if (equippedBody != null) totalMaxHealth += equippedBody.additionalHP; 
        if (equippedCab != null) totalMaxHealth += equippedCab.additionalHP;

        return totalMaxHealth;
    }

    public float GetTotalMaxFuel()
    {
        float totalMaxFuel = BASE_FUEL;
        
        // Предполагается, что вместимость бака зависит от кабины
        if (equippedCab != null) totalMaxFuel += equippedBody.fuelCapacity; 

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