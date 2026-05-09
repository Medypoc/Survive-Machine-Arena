using UnityEngine;
using UnityEngine.UI; // Используйте TMPro, если у вас TextMeshPro

public class MoneyDisplayUI : MonoBehaviour
{
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private Text moneyText; 

    private void OnEnable()
    {
        // Подписываемся на событие изменения денег
        if (economyManager != null)
        {
            economyManager.OnMoneyChanged += UpdateMoneyText;
        }
    }

    private void OnDisable()
    {
        // Обязательно отписываемся при выключении объекта, чтобы не было ошибок
        if (economyManager != null)
        {
            economyManager.OnMoneyChanged -= UpdateMoneyText;
        }
    }

    // Этот метод вызовется АВТОМАТИЧЕСКИ, когда EconomyManager изменит баланс
    private void UpdateMoneyText(int currentMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"{currentMoney} $";
        }
    }
}