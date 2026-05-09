using UnityEngine;
using System;
using SurviveArena.Data; // Пространство имен, где лежит ваш PlayerDataSO

public class EconomyManager : MonoBehaviour
{
    [Header("Player Profile")]
    [SerializeField] private PlayerDataSO playerProfile;

    // Событие (Event). На него смогут "подписаться" другие скрипты (например, UI)
    public event Action<int> OnMoneyChanged;

    private void Start()
    {
        // При старте игры сразу оповещаем всех о текущем балансе
        NotifyMoneyChanged();
    }

    /// <summary>
    /// Проверяет, хватает ли у игрока денег
    /// </summary>
    public bool CanAfford(int amount)
    {
        return playerProfile.money >= amount;
    }

    /// <summary>
    /// Пытается потратить деньги. Возвращает true, если покупка успешна.
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (CanAfford(amount))
        {
            playerProfile.money -= amount;
            NotifyMoneyChanged();
            Debug.Log($"[Economy] Потрачено: {amount}$. Текущий баланс: {playerProfile.money}$");
            return true;
        }
        
        Debug.LogWarning("[Economy] Недостаточно средств!");
        return false;
    }

    public void SpendMoney(int amount) 
    {
    playerProfile.money -= amount;
    OnMoneyChanged?.Invoke(playerProfile.money);
    }

    /// <summary>
    /// Добавляет деньги на баланс игрока (например, после победы на арене)
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount > 0)
        {
            playerProfile.money += amount;
            NotifyMoneyChanged();
            Debug.Log($"[Economy] Получено: {amount}$. Текущий баланс: {playerProfile.money}$");
        }
    }

    // Вспомогательный метод для вызова события
    private void NotifyMoneyChanged()
    {
        // Знак вопроса означает "Если на событие кто-то подписан, то вызываем его"
        OnMoneyChanged?.Invoke(playerProfile.money);
    }
}