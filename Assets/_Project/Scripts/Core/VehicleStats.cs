using UnityEngine; // Это решает проблему с [Header] и SpriteRenderer

public class VehicleStats : MonoBehaviour
{
    public enum VehicleBase { Courier, HeavyTruck, Interceptor }

    [Header("Configuration")]
    public VehicleBase currentBase = VehicleBase.Courier;
    public CabDataSO cabData;
    public BodyDataSO bodyData;
    public WeaponDataSO weaponData;

    [Header("Anchor Slots")]
    public VehiclePartSlot bodySlot;
    public VehiclePartSlot cabinSlot;
    public VehiclePartSlot weaponSlot;

    [Header("Live Stats (Required by CarController)")]
    public float maxSpeed;
    public float acceleration;
    public float totalWeight;
    public float maxHP;
    public float currentHP;
    public float fuelCapacity;
    public float currentFuel;

    void Start()
    {
        InitializeVehicle();
    }

    public void InitializeVehicle()
    {
        if (cabData == null || bodyData == null || weaponData == null)
        {
            Debug.LogError("VehicleStats: Назначьте все ScriptableObjects!");
            return;
        }

        // 1. Расчет характеристик (Математика)
        maxSpeed = cabData.baseSpeed;
        acceleration = cabData.baseAcceleration;
        totalWeight = cabData.weight + bodyData.weight;
        maxHP = cabData.additionalHP + bodyData.additionalHP;
        currentHP = maxHP;
        fuelCapacity = bodyData.fuelCapacity;
        currentFuel = fuelCapacity;

        // 2. Позиционирование АНКЕРОВ (Логических гнезд)
        if (cabinSlot != null)
            cabinSlot.transform.localPosition = (Vector3)bodyData.cabinAnchorPoint;

        // 3. Обновление ВИЗУАЛА (Спрайтов) через слоты
        if (bodySlot != null) bodySlot.UpdatePart(bodyData.partSprite, 10);
        if (cabinSlot != null) cabinSlot.UpdatePart(cabData.partSprite, 20);
        if (weaponSlot != null) weaponSlot.UpdatePart(weaponData.weaponSprite, 30);

        // 4. Обновление физических границ
        UpdateColliders();
    }

    void UpdateColliders()
    {
        // Профессиональный подход: обновляем коллайдеры на тех объектах, где есть спрайты
        if (bodySlot != null && bodySlot.visualRenderer != null)
        {
            var col = bodySlot.visualRenderer.gameObject.GetComponent<PolygonCollider2D>() ?? 
                      bodySlot.visualRenderer.gameObject.AddComponent<PolygonCollider2D>();
            // Здесь можно добавить логику пересчета путей коллайдера, если нужно
        }
    }
}