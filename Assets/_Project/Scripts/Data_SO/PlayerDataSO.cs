using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "SurviveArena/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Economy")]
    public int money;

    [Header("Progression")]
    public int currentRank = 1;
    public int currentExp = 0;
    public int expToNextRank = 1000;

    // Метод для сохранения прогресса (вызовешь позже)
    public void AddMoney(int amount) => money += amount;
}