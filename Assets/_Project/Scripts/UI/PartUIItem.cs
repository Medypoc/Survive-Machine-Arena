using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SurviveArena.Data;

public class PartUIItem : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text statsText;
    public TMP_Text priceText;
    public Button actionButton;

    private PartDataSO _partData;
    private GarageManager _manager;

    public void Setup(PartDataSO data, GarageManager manager)
    {
        _partData = data;
        _manager = manager;

        if (data == null) return;

        nameText.text = data.partName;
        iconImage.sprite = data.partSprite;
        priceText.text = $"{data.price}$";

        if (data is CabDataSO cab)
            statsText.text = $"Броня: +{cab.armor * 100}%\nСкорость: {cab.baseSpeed}";
        else if (data is BodyDataSO body)
            statsText.text = $"ХП: +{body.additionalHP}\nТопливо: {body.fuelCapacity}л";
        else if (data is WeaponDataSO weapon)
            statsText.text = $"Урон: {weapon.damageStats.minDamage}-{weapon.damageStats.maxDamage}\nRPM: {weapon.shootingStats.fireRateRPM}";

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(HandlePurchaseClick);
        }
    }

    private void HandlePurchaseClick()
    {
        // ПОСЛЕДНИЙ ШАНС: Если менеджер всё еще NULL, пытаемся найти его на сцене
        if (_manager == null)
        {
            _manager = FindObjectOfType<GarageManager>();
        }

        if (_manager != null && _partData != null)
        {
            _manager.OnPartSelected(_partData);
        }
        else
        {
            Debug.LogError($"[PartUIItem] Критическая ошибка: Не удалось найти GarageManager даже через поиск на сцене!");
        }
    }
}