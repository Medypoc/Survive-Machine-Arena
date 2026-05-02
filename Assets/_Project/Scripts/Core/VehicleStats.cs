using UnityEngine;
using System;
using SurviveArena.Core;
using SurviveArena.Data;

public class VehicleStats : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] private BodyDataSO _bodyData;
    [SerializeField] private CabDataSO _cabData;
    [SerializeField] private WeaponDataSO _weaponData;

    public BodyDataSO Body => _bodyData;
    public CabDataSO Cab => _cabData;
    public WeaponDataSO Weapon => _weaponData;

    [Header("Visual Slots")]
    [SerializeField] private SpriteRenderer _bodyRenderer;
    [SerializeField] private SpriteRenderer _cabRenderer;
    [SerializeField] private SpriteRenderer _weaponRenderer;

    [Header("Calculated Stats")]
    public float Acceleration { get; private set; }
    public float SteeringSpeed { get; private set; }
    public float Armor { get; private set; }
    public int MaxHealth { get; private set; }
    public float TotalWeight { get; private set; }
    
    // ДОБАВЛЕНО: Свойство для слотов инвентаря
    public int InventorySlots { get; private set; } 

    public float DamageMultiplier { get; private set; } = 1f;

    private float _hpMult = 1f;
    private float _speedMult = 1f;
    private Health _healthComponent;
    
    // ИСПРАВЛЕНО: Объявляем переменную для топлива, чтобы не было ошибки CS0103
    private Fuel _fuelComponent; 

    public event Action OnStatsChanged;

    private void Awake()
    {
        _healthComponent = GetComponent<Health>();
        _fuelComponent = GetComponent<Fuel>(); // Теперь скрипт знает, что такое _fuelComponent
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

        // 1. Базовые значения (сбрасываем перед новым расчетом)
        Acceleration = 0; 
        SteeringSpeed = 0; 
        Armor = 0; 
        MaxHealth = 100;
        TotalWeight = 0;
        InventorySlots = 0; 

        // 2. Считаем статы кабины
        if (_cabData != null)
        {
            Acceleration += _cabData.baseAcceleration;
            SteeringSpeed += _cabData.steeringSpeed;
            Armor += _cabData.armor;
            MaxHealth += _cabData.additionalHP;
            TotalWeight += _cabData.weight; 
        }

        // 3. Считаем статы кузова (ТЕПЕРЬ УЧИТЫВАЕТСЯ ВСЁ)
        if (_bodyData != null)
        {
            Armor += _bodyData.armor;
            MaxHealth += _bodyData.additionalHP;
            TotalWeight += _bodyData.weight; 
            
            // ВАЖНО: Убедись, что в скрипте BodyDataSO эта переменная называется именно inventorySlots 
            // (или extraStorageSlots, как было в твоих старых файлах). Если она называется иначе - поменяй тут.
            InventorySlots += _bodyData.inventorySlots; 
        }

        // 4. Применяем коэффициенты модификаторов
        Acceleration *= _speedMult;
        SteeringSpeed *= _speedMult;
        MaxHealth = Mathf.RoundToInt(MaxHealth * _hpMult);

        // 5. Синхронизируем здоровье
        if (_healthComponent != null) 
        {
            float healthPercentage = _healthComponent.maxHealth > 0 
                ? (float)_healthComponent.currentHealth / _healthComponent.maxHealth 
                : 1f;
            
            _healthComponent.maxHealth = MaxHealth;
            _healthComponent.currentHealth = Mathf.RoundToInt(MaxHealth * healthPercentage); 
            
            _healthComponent.NotifyHealthChanged(); 
        }

        // 6. Синхронизируем топливо (работает только для Игрока)
        if (_fuelComponent != null && _bodyData != null)
        {
            _fuelComponent.maxFuel = _bodyData.fuelCapacity; 
            
            if (_fuelComponent.currentFuel <= 0) 
            {
                _fuelComponent.currentFuel = _fuelComponent.maxFuel;
            }
            
            _fuelComponent.NotifyFuelChanged();
        }

        OnStatsChanged?.Invoke();
    }

    private void UpdateVisuals()
    {
        if (_cabData != null && _cabRenderer != null) 
            _cabRenderer.sprite = _cabData.partSprite;

        if (_bodyData != null && _bodyRenderer != null) 
            _bodyRenderer.sprite = _bodyData.partSprite;

        if (_weaponData != null && _weaponRenderer != null) 
            _weaponRenderer.sprite = _weaponData.partSprite;
    }
}