using UnityEngine;
using TMPro;
using System;
using SurviveArena.Data; // Оставляем на всякий случай

public class GarageUI : MonoBehaviour 
{
    [Header("UI Elements")]
    [SerializeField] private PopUpUI _confirmPopup;
    [SerializeField] private TabController _tabs;

    // Теперь метод принимает ГОТОВЫЙ текст сообщения
    public void ShowConfirmation(string message, Action onConfirm) 
    {
        if (_confirmPopup != null)
        {
            _confirmPopup.Show(message, onConfirm);
        }
    }

    public void ShowNotification(string message) 
    {
        Debug.Log($"[Уведомление Гаража]: {message}");
    }
}