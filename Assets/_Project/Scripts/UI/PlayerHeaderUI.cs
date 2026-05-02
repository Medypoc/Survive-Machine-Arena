using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHeaderUI : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private PlayerDataSO _playerProfile; // Объединенный источник данных[cite: 8, 9]

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _moneyText;  // Из старого скрипта
    [SerializeField] private TextMeshProUGUI _rankText;   // Общее поле[cite: 8, 9]
    [SerializeField] private Slider _xpSlider;            // Из нового скрипта
    [SerializeField] private TextMeshProUGUI _xpText;     // Из нового скрипта[cite: 8]

    private void Update()
    {
        // Предохранитель, чтобы не было ошибок, если профиль не назначен[cite: 8, 9]
        if (_playerProfile == null) return;

        // 1. Обновляем деньги[cite: 9]
        if (_moneyText != null) 
        {
            _moneyText.text = $"MONEY: {_playerProfile.money}";
        }

        // 2. Узнаем планку для текущего ранга[cite: 8]
        int requiredXP = _playerProfile.GetXPForNextRank();

        // 3. Обновляем опыт (текст)[cite: 8]
        if (_xpText != null)
        {
            _xpText.text = $"{_playerProfile.currentXP} / {requiredXP}";
        }

        // 4. Обновляем текст ранга[cite: 8, 9]
        if (_rankText != null) 
        {
            _rankText.text = $"RANK: {_playerProfile.currentRank}";
        }

        // 5. Настраиваем и заполняем слайдер опыта[cite: 8]
        if (_xpSlider != null)
        {
            _xpSlider.maxValue = requiredXP; // Слайдер подстроит масштаб[cite: 8]
            _xpSlider.value = _playerProfile.currentXP; // Текущее заполнение[cite: 8]
        }
    }
}