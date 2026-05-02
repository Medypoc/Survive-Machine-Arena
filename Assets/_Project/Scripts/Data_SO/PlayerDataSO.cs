using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerProfile", menuName = "SurviveArena/Player Profile")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Economy & Progression")]
    public int money;
    public int currentXP;
    public int currentRank = 1;

    // Метод для расчета: сколько всего нужно опыта для следующего ранга.
    // Пока сделаем простую прогрессию: 1 ранг = 500 ХП, 2 ранг = 1000 ХП, 3 ранг = 1500 ХП и т.д.
    public int GetXPForNextRank() 
    {
        return currentRank * 500; 
    }

    // Этот метод мы будем вызывать из BattleManager после победы
    public void AddExperience(int amount)
    {
        currentXP += amount;

        // Если опыта больше или равно нужному - повышаем ранг
        while (currentXP >= GetXPForNextRank())
        {
            currentXP -= GetXPForNextRank(); // Вычитаем "стоимость" ранга
            currentRank++;                   // Повышаем ранг
        }
    }
}