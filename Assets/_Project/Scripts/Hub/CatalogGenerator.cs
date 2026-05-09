using UnityEngine;
using SurviveArena.Data;
using System.Collections.Generic;

public class CatalogGenerator : MonoBehaviour
{
    [Header("Настройки Префабов")]
    [SerializeField] private GameObject partItemPrefab;

    [Header("Ссылки на Данные")]
    [SerializeField] private VehicleRegistry_SO registry;
    [SerializeField] private PlayerDataSO playerProfile;

    [Header("Контейнеры UI")]
    [SerializeField] private Transform cabContentParent;
    [SerializeField] private Transform bodyContentParent;

    [Header("Связь")]
    [SerializeField] private GarageManager _garageManager;

    private void Awake()
    {
        // АВТО-ПОИСК: Если забыли перетащить ссылку в инспекторе, 
        // скрипт сам найдет GarageManager на этом же объекте.
        if (_garageManager == null)
        {
            _garageManager = GetComponent<GarageManager>();
        }
    }

    public void GenerateCabs()
    {
        ClearContainer(cabContentParent);
        PopulateCategory(PartType.Cab, cabContentParent);
    }

    public void GenerateBodies()
    {
        ClearContainer(bodyContentParent);
        PopulateCategory(PartType.Body, bodyContentParent);
    }

    private void PopulateCategory(PartType type, Transform container)
    {
        if (playerProfile.selectedVehicleClass == null) return;

        var classContent = registry.classes.Find(x => x.vehicleClass == playerProfile.selectedVehicleClass);
        if (classContent == null) return;

        if (type == PartType.Cab)
            SpawnItems(classContent.availableCabs, container);
        else if (type == PartType.Body)
            SpawnItems(classContent.availableBodies, container);
    }

    private void SpawnItems<T>(List<T> parts, Transform parent) where T : PartDataSO
    {
        // КРИТИЧЕСКАЯ ПРОВЕРКА: Если менеджера всё еще нет, ищем его по всей сцене
        if (_garageManager == null)
        {
            _garageManager = FindObjectOfType<GarageManager>();
            Debug.LogWarning("CatalogGenerator: Ссылка на GarageManager была пуста, пришлось искать через FindObjectOfType!");
        }

        foreach (var partData in parts)
        {
            GameObject item = Instantiate(partItemPrefab, parent);
            PartUIItem script = item.GetComponent<PartUIItem>();
            
            if (script != null)
            {
                script.Setup(partData, _garageManager);
            }
        }
    }

    private void ClearContainer(Transform parent)
    {
        if (parent == null) return;
        foreach (Transform child in parent) Destroy(child.gameObject);
    }

    private enum PartType { Cab, Body }
}