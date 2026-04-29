using UnityEngine;

public class VehicleStats : MonoBehaviour
{
    // Перечисление для проверки совместимости деталей
    public enum VehicleBase { Courier, HeavyTruck, Interceptor }

    [Header("Configuration & Compatibility")]
    public VehicleBase currentBase = VehicleBase.Courier;
    public CabDataSO cabData;
    public BodyDataSO bodyData;

    [Header("Visual Slots")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer cabinRenderer;
    public Transform cabinSocket; // Дочерний объект-пустышка для позиции кабины

    [Header("Live Stats")]
    public float currentHP;
    public float maxHP;
    public float totalWeight;
    public float maxSpeed;
    public float acceleration;
    public float currentFuel;
    public float fuelCapacity;

    void Start()
    {
        InitializeVehicle();
    }

    public void InitializeVehicle()
    {
        // Проверка на наличие данных
        if (cabData == null || bodyData == null)
        {
            Debug.LogError("VehicleStats: Кабина или Кузов не назначены!");
            return;
        }

        // Проверка совместимости базы (Твоя идея)
        if (cabData.compatibleBase != currentBase || bodyData.compatibleBase != currentBase)
        {
            Debug.LogWarning("Внимание: Детали не подходят к базе " + currentBase);
        }

        // 1. Расчет характеристик
        maxHP = cabData.additionalHP + bodyData.additionalHP;
        currentHP = maxHP;
        totalWeight = cabData.weight + bodyData.weight;
        maxSpeed = cabData.baseSpeed;
        acceleration = cabData.baseAcceleration;
        fuelCapacity = bodyData.fuelCapacity;
        currentFuel = fuelCapacity;

        // 2. Визуальная сборка
        if (bodyRenderer != null) bodyRenderer.sprite = bodyData.partSprite;
        if (cabinRenderer != null) cabinRenderer.sprite = cabData.partSprite;

        // 3. Установка кабины в точку крепления (Якорь), прописанную в данных кузова
        if (cabinSocket != null)
        {
            cabinSocket.localPosition = bodyData.cabinAnchorPoint;
        }

        // 4. Обновление колайдеров под новые спрайты
        UpdateColliders();
    }

    void UpdateColliders()
    {
        // Удаляем старые PolygonCollider2D и добавляем новые, чтобы они облегали новые спрайты
        if (bodyRenderer != null)
        {
            Destroy(bodyRenderer.GetComponent<PolygonCollider2D>());
            bodyRenderer.gameObject.AddComponent<PolygonCollider2D>();
        }
        
        if (cabinRenderer != null)
        {
            Destroy(cabinRenderer.GetComponent<PolygonCollider2D>());
            cabinRenderer.gameObject.AddComponent<PolygonCollider2D>();
        }
    }
}