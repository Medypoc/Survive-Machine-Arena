using UnityEngine;
using TMPro; // Для работы с современным текстом
using UnityEngine.UI; // Для работы с кнопками
using System; // Для использования Action (передача методов)

public class PopUpUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _confirmButton; // Кнопка "Да/Купить/Починить"
    [SerializeField] private Button _cancelButton;  // Кнопка "Нет/Отмена"

    private Action _onConfirmAction;

    private void Awake()
    {
        // Очищаем слушателей на всякий случай, чтобы избежать двойных кликов
        _confirmButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        // Кнопка отмены просто закрывает окно
        _cancelButton.onClick.AddListener(Hide);
        
        // Кнопка подтверждения выполняет переданное действие и закрывает окно
        _confirmButton.onClick.AddListener(() => {
            _onConfirmAction?.Invoke();
            Hide();
        });
    }

    // Метод вызывается из GarageManager'а
    public void Show(string message, Action onConfirm)
    {
        _messageText.text = message;
        _onConfirmAction = onConfirm;
        gameObject.SetActive(true); // Включаем окно
    }

    public void Hide()
    {
        gameObject.SetActive(false); // Выключаем окно
    }
}