using UnityEngine;
using TMPro;

public class GarageUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GarageManager _garageManager;
    [SerializeField] private VehicleStats _vehicleStats; // Для обновления UI после ремонта

    [Header("Confirmation Popup")]
    [SerializeField] private GameObject _confirmationPopup;
    [SerializeField] private TextMeshProUGUI _confirmationText;

    // 1. Вызывается главной кнопкой "Починить" в меню гаража
    public void OpenRepairConfirmation()
    {
        int cost = _garageManager.GetRepairCost();

        if (cost <= 0)
        {
            Debug.Log("Машина уже в идеальном состоянии!");
            return;
        }

        // Устанавливаем текст в окошке
        if (_confirmationText != null)
        {
            _confirmationText.text = $"Починить машину за {cost}$?";
        }

        _confirmationPopup.SetActive(true);
    }

    // 2. Вызывается кнопкой "ДА" в окошке подтверждения
    public void ConfirmRepair()
    {
        if (_garageManager.TryRepair())
        {
            _confirmationPopup.SetActive(false);
            
            // Важно: обновляем статы, чтобы UI сразу увидел изменения
            if (_vehicleStats != null)
            {
                _vehicleStats.RefreshStats(); 
            }
        }
        else
        {
            Debug.Log("Недостаточно денег!");
            // Тут можно добавить визуальный эффект "красной вспышки" денег
        }
    }

    // 3. Вызывается кнопкой "НЕТ" или крестиком
    public void CloseConfirmation()
    {
        _confirmationPopup.SetActive(false);
    }
}