using UnityEngine;
using SurviveArena.Data;

public class GarageManager : MonoBehaviour 
{
    [Header("Системы и Данные")]
    public PlayerDataSO playerProfile;
    [SerializeField] private EconomyManager _economy;
    [SerializeField] private GarageUI _garageUI;
    [SerializeField] private CatalogGenerator _catalogGenerator;
    [SerializeField] private RepairModule _repairModule;

    [Header("Визуализация")]
    // ТА САМАЯ ПЕРЕМЕННАЯ, КОТОРОЙ НЕ ХВАТАЛО
    [SerializeField] private PlayerEquipmentLoader vehicleVisualizer; 

    [Header("Панели Интерфейса")]
    [SerializeField] private GameObject catalogPanel;
    [SerializeField] private GameObject vehicleContent;
    [SerializeField] private GameObject cabContent;
    [SerializeField] private GameObject bodyContent;

    private void Start() => CloseCatalog();

    public void ShowVehicles() => SwitchContent(vehicleContent);

    public void ShowCabs() 
    {
        if (_catalogGenerator != null) _catalogGenerator.GenerateCabs();
        SwitchContent(cabContent);
    }

    public void ShowBodies() 
    {
        if (_catalogGenerator != null) _catalogGenerator.GenerateBodies();
        SwitchContent(bodyContent);
    }

    public void CloseCatalog() => catalogPanel?.SetActive(false);

    private void SwitchContent(GameObject activeContent)
    {
        if (catalogPanel != null) catalogPanel.SetActive(true);
        vehicleContent?.SetActive(false);
        cabContent?.SetActive(false);
        bodyContent?.SetActive(false);
        activeContent?.SetActive(true);
    }

    public void OnPartSelected(PartDataSO data) 
    {
        if (_economy.CanAfford(data.price)) 
        {
            _garageUI.ShowConfirmation($"Купить {data.partName} за {data.price}$?", () => ConfirmPurchase(data));
        }
        else _garageUI.ShowNotification("Недостаточно средств!");
    }

    private void ConfirmPurchase(PartDataSO data) 
    {
        if (_economy.TrySpend(data.price))
        {
            if (data is BodyDataSO body) playerProfile.equippedBody = body;
            else if (data is CabDataSO cab) playerProfile.equippedCab = cab;
            else if (data is WeaponDataSO weapon) playerProfile.equippedWeapon = weapon;
            
            // Обновляем визуал машины
            if (vehicleVisualizer != null) vehicleVisualizer.LoadVehicle();

            _garageUI.ShowNotification($"{data.partName} установлен!");
        }
    }

    public void RequestRepair()
    {
        if (!_repairModule.NeedsRepair())
        {
            _garageUI.ShowNotification("Машина уже исправна!");
            return;
        }

        int cost = _repairModule.GetRepairCost();
        _garageUI.ShowConfirmation($"Починить за {cost}$?", () => 
        {
            if (_economy.TrySpend(cost))
            {
                _repairModule.RestoreHealth();
                _garageUI.ShowNotification("Готово!");
            }
        });
    }
}