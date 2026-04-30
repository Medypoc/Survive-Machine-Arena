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

    [Header("Calculated Stats")]
    public float Acceleration { get; private set; }
    public float SteeringSpeed { get; private set; }
    public float Armor { get; private set; }
    public int MaxHealth { get; private set; }
    
    public float DamageMultiplier { get; private set; } = 1f;

    // Множители статов
    private float _hpMult = 1f;
    private float _speedMult = 1f;

    // Кешируем компонент здоровья для оптимизации
    private Health _healthComponent;

    public event Action OnStatsChanged;

    private void Awake()
    {
        // Получаем ссылку на компонент один раз при создании объекта
        _healthComponent = GetComponent<Health>();
    }

    private void Start() 
    {
        // Вызываем первичный пересчет в Start. 
        // Это гарантирует, что Health.Awake() уже отработал и не перезапишет наши статы.
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
        // 1. Сбрасываем статы до базовых перед новым расчетом
        Acceleration = 0; 
        SteeringSpeed = 0; 
        Armor = 0; 
        MaxHealth = 100; // Базовое здоровье "голого" шасси

        // 2. Суммируем статы от кабины
        if (_cabData != null)
        {
            Acceleration += _cabData.baseAcceleration;
            SteeringSpeed += _cabData.steeringSpeed;
            Armor += _cabData.armor;
            MaxHealth += _cabData.additionalHP;
        }

        // TODO: Добавь сюда статы от других модулей, если они у них есть в скриптаблах.
        // Пример того, как это должно выглядеть:
        // if (_bodyData != null) { Armor += _bodyData.additionalArmor; }
        // if (_weaponData != null) { Acceleration -= _weaponData.weightPenalty; }

        // 3. Применяем множители (баффы, дебаффы, сложность) ко всем собранным статам
        Acceleration *= _speedMult;
        SteeringSpeed *= _speedMult;
        MaxHealth = Mathf.RoundToInt(MaxHealth * _hpMult);

        // 4. Безопасно обновляем здоровье, сохраняя процент ранений
        if (_healthComponent != null) 
        {
            // Вычисляем процент текущего здоровья (от 0.0 до 1.0)
            // Если maxHealth еще 0 (самый первый старт), считаем что машина полностью цела (1f)
            float healthPercentage = _healthComponent.maxHealth > 0 
                ? (float)_healthComponent.currentHealth / _healthComponent.maxHealth 
                : 1f;
            
            // Задаем новый максимум
            _healthComponent.maxHealth = MaxHealth;
            
            // Восстанавливаем текущее здоровье в том же процентном соотношении
            _healthComponent.currentHealth = Mathf.RoundToInt(MaxHealth * healthPercentage); 
        }

        // 5. Оповещаем другие системы (например, UI полоски здоровья)
        OnStatsChanged?.Invoke();
    }
}