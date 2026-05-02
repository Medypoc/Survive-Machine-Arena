using UnityEngine;
using SurviveArena.Data;

[CreateAssetMenu(fileName = "NewPlayerProfile", menuName = "SurviveArena/Player Profile")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Economy & Progression")]
    public int money;
    public int currentXP;
    public int currentRank = 1;

    // --- НОВЫЙ БЛОК: ЭКИПИРОВКА ---
    [Header("Equipped Parts")]
    public BodyDataSO equippedBody;
    public CabDataSO equippedCab;
    public WeaponDataSO equippedWeapon;
    // ------------------------------

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