using UnityEngine;
using System;
using SurviveArena.Core;
using SurviveArena.Data;

public class VehicleStats : MonoBehaviour
{
    private BodyDataSO _bodyData;
    private CabDataSO _cabData;
    private WeaponDataSO _weaponData;

    public BodyDataSO Body => _bodyData;
    public CabDataSO Cab => _cabData;
    public WeaponDataSO Weapon => _weaponData;

    [Header("Visual Slots")]
    [SerializeField] private VehiclePartSlot _bodySlot;
    [SerializeField] private VehiclePartSlot _cabSlot;
    [SerializeField] private VehiclePartSlot _weaponSlot;

    [Header("Calculated Stats")]
    public float Acceleration { get; private set; }
    public float SteeringSpeed { get; private set; }
    public float Armor { get; private set; }
    public int MaxHealth { get; private set; }
    public float TotalWeight { get; private set; }
    public int InventorySlots { get; private set; } 
    public float DamageMultiplier { get; private set; } = 1f;

    private float _hpMult = 1f;
    private float _speedMult = 1f;
    private Health _healthComponent;
    private Fuel _fuelComponent; 

    public event Action OnStatsChanged;

    private void Awake()
    {
        _healthComponent = GetComponent<Health>();
        _fuelComponent = GetComponent<Fuel>(); 
    }

    private void Start() 
    {
        RefreshStats();
    }

    public void LoadModules(BodyDataSO body, CabDataSO cab, WeaponDataSO weapon)
    {
        _bodyData = body;
        _cabData = cab;
        _weaponData = weapon;
        RefreshStats();
    }

    // НОВЫЙ МЕТОД: Расчет урона для текущего оружия
    public float CalculateAttackDamage(out bool isCritical)
    {
        isCritical = false;
        if (_weaponData == null) return 0f;

        // Берем случайное значение из диапазона оружия
        float baseDamage = UnityEngine.Random.Range(_weaponData.minDamage, _weaponData.maxDamage);

        // Проверяем шанс критического удара
        if (UnityEngine.Random.value <= _weaponData.criticalHitChance)
        {
            isCritical = true;
            baseDamage *= _weaponData.criticalDamageMultiplier;
        }

        // Применяем общий модификатор урона машины
        return baseDamage * DamageMultiplier;
    }

    public void ApplyModifiers(float hp, float speed, float damage)
    {
        _hpMult = hp;
        _speedMult = speed;
        DamageMultiplier = damage;
        RefreshStats();
    }

    public void RefreshStats()
    {
        UpdateVisuals();

        // Сброс и расчет базовых характеристик
        Acceleration = 0; 
        SteeringSpeed = 0; 
        Armor = 0; 
        MaxHealth = 100;
        TotalWeight = 0;
        InventorySlots = 0; 

        if (_cabData != null)
        {
            Acceleration += _cabData.baseAcceleration;
            SteeringSpeed += _cabData.steeringSpeed;
            Armor += _cabData.armor;
            MaxHealth += _cabData.additionalHP;
            TotalWeight += _cabData.weight; 
        }

        if (_bodyData != null)
        {
            Armor += _bodyData.armor;
            MaxHealth += _bodyData.additionalHP;
            TotalWeight += _bodyData.weight; 
            InventorySlots += _bodyData.inventorySlots; 
        }

        Acceleration *= _speedMult;
        SteeringSpeed *= _speedMult;
        MaxHealth = Mathf.RoundToInt(MaxHealth * _hpMult);

        // УДАЛЕН КОНФЛИКТУЮЩИЙ БЛОК СИНХРОНИЗАЦИИ ЗДОРОВЬЯ

        // Оставляем только синхронизацию топлива (если она тебе нужна здесь)
        if (_fuelComponent != null && _bodyData != null)
        {
            _fuelComponent.maxFuel = _bodyData.fuelCapacity; 
            if (_fuelComponent.currentFuel <= 0) _fuelComponent.currentFuel = _fuelComponent.maxFuel;
            _fuelComponent.NotifyFuelChanged();
        }

        // Если нам нужно обновить UI максимального здоровья, вызываем метод из Health
        if (_healthComponent != null)
        {
            // Передаем актуальный максимум, но НЕ ТРОГАЕМ currentHealth
            _healthComponent.maxHealth = MaxHealth; 
            _healthComponent.NotifyHealthChanged();
        }

        OnStatsChanged?.Invoke();
    }

    private void UpdateVisuals()
    {
        if (_cabData != null && _cabSlot != null) _cabSlot.UpdatePart(_cabData.partSprite, 10);
        if (_bodyData != null && _bodySlot != null) _bodySlot.UpdatePart(_bodyData.partSprite, 5);
        if (_weaponData != null && _weaponSlot != null) _weaponSlot.UpdatePart(_weaponData.partSprite, 15);
    }
}