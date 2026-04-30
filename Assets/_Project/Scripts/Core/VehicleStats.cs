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

    // Эти свойства нужны твоему AIController и другим скриптам
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
    
    public float DamageMultiplier { get; private set; } = 1f;

    private float _hpMult = 1f;
    private float _speedMult = 1f;
    private Health _healthComponent;

    public event Action OnStatsChanged;

    private void Awake()
    {
        _healthComponent = GetComponent<Health>();
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
        // Обновляем спрайты
        UpdateVisuals();

        // Базовые значения
        Acceleration = 0; 
        SteeringSpeed = 0; 
        Armor = 0; 
        MaxHealth = 100;

        // Считаем статы кабины
        if (_cabData != null)
        {
            Acceleration += _cabData.baseAcceleration;
            SteeringSpeed += _cabData.steeringSpeed;
            Armor += _cabData.armor;
            MaxHealth += _cabData.additionalHP;
        }

        // Применяем коэффициенты
        Acceleration *= _speedMult;
        SteeringSpeed *= _speedMult;
        MaxHealth = Mathf.RoundToInt(MaxHealth * _hpMult);

        // Синхронизируем с компонентом Health
        if (_healthComponent != null) 
        {
            float healthPercentage = _healthComponent.maxHealth > 0 
                ? (float)_healthComponent.currentHealth / _healthComponent.maxHealth 
                : 1f;
            
            _healthComponent.maxHealth = MaxHealth;
            _healthComponent.currentHealth = Mathf.RoundToInt(MaxHealth * healthPercentage); 
            
            // Исправленная строка:
            _healthComponent.NotifyHealthChanged(); 
        }

        OnStatsChanged?.Invoke();
    }

    private void UpdateVisuals()
    {
        // Используем partSprite, так как он определен в базовом классе PartDataSO
        if (_cabData != null && _cabRenderer != null) 
            _cabRenderer.sprite = _cabData.partSprite;

        if (_bodyData != null && _bodyRenderer != null) 
            _bodyRenderer.sprite = _bodyData.partSprite;

        if (_weaponData != null && _weaponRenderer != null) 
            _weaponRenderer.sprite = _weaponData.partSprite;
    }
}