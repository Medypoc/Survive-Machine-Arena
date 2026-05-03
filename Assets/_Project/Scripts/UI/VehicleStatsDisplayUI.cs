using UnityEngine;
using TMPro;

public class VehicleStatsDisplayUI : MonoBehaviour
{
    private VehicleStats _stats;
    private Fuel _fuel; 

    [Header("Data Sources")]
    // МЫ ДОБАВИЛИ ЭТУ СТРОКУ:
    [SerializeField] private PlayerDataSO _playerProfile; 

    [Header("Health & Armor")]
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _currentHealthText; 
    [SerializeField] private TextMeshProUGUI _cabArmorText;
    [SerializeField] private TextMeshProUGUI _bodyArmorText;
    
    [Header("Movement")]
    [SerializeField] private TextMeshProUGUI _maxSpeedText; 
    [SerializeField] private TextMeshProUGUI _accelText;    
    
    [Header("General Vehicle Stats")]
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private TextMeshProUGUI _fuelCapacityText;
    [SerializeField] private TextMeshProUGUI _fuelConsumptionText; 

    [Header("Weapon Stats")]
    [SerializeField] private TextMeshProUGUI _weaponAngleText;
    [SerializeField] private TextMeshProUGUI _weaponDamageText; 
    [SerializeField] private TextMeshProUGUI _weaponCritText;   
    [SerializeField] private TextMeshProUGUI _weaponFireRateText;
    [SerializeField] private TextMeshProUGUI _weaponRangeText;
    [SerializeField] private TextMeshProUGUI _weaponBulletSpeedText; 

    private void Start()
    {
        GameObject vehicle = GameObject.FindGameObjectWithTag("Player");
        if (vehicle != null)
        {
            _stats = vehicle.GetComponent<VehicleStats>();
            _fuel = vehicle.GetComponent<Fuel>();
        }
    }

    private void Update()
    {
        if (_stats == null) return;

        // 1. Здоровье
        if (_healthText != null) _healthText.text = $"Макс. Здоровье: {_stats.MaxHealth}";

        // ТЕПЕРЬ ПЕРЕМЕННАЯ ОБЪЯВЛЕНА И ИСПОЛЬЗУЕТСЯ ПРАВИЛЬНО
        if (_currentHealthText != null && _playerProfile != null)
        {
            _currentHealthText.text = $"Текущее Здоровье: {_playerProfile.currentHealth}";
        }

        // 2. Броня
        if (_cabArmorText != null)
        {
            float cabArmorPercent = (_stats.Cab != null ? _stats.Cab.armor : 0f);
            _cabArmorText.text = $"Броня кабины: {cabArmorPercent:F0}%";
        }

        if (_bodyArmorText != null)
        {
            float bodyArmorPercent = (_stats.Body != null ? _stats.Body.armor : 0f);
            _bodyArmorText.text = $"Броня кузова: {bodyArmorPercent:F0}%";
        }

        // 3. Передвижение
        if (_maxSpeedText != null)
        {
            float maxSpeed = _stats.Cab != null ? _stats.Cab.baseSpeed : 0f;
            _maxSpeedText.text = $"Макс. скорость: {maxSpeed}";
        }
        
        if (_accelText != null)
        {
            _accelText.text = $"Ускорение: {_stats.Acceleration:F1}";
        }

        // 4. Вес
        if (_weightText != null) 
        {
            float weightInTons = _stats.TotalWeight / 1000f;
            _weightText.text = $"Вес: {weightInTons:F1} т";
        }

        // 5. Топливная система
        if (_fuelCapacityText != null)
        {
            float capacity = _stats.Body != null ? _stats.Body.fuelCapacity : 0f;
            _fuelCapacityText.text = $"Объем бака: {capacity} л";
        }
        
        if (_fuelConsumptionText != null)
        {
            float weightMultiplier = _stats.TotalWeight / 1000f;
            _fuelConsumptionText.text = $"Коэфф. расхода: x{weightMultiplier:F2}";
        }

        // 6. Оружие
        if (_stats.Weapon != null)
        {
            if (_weaponDamageText != null) 
                _weaponDamageText.text = $"Урон: {_stats.Weapon.minDamage:F0} - {_stats.Weapon.maxDamage:F0}";

            if (_weaponCritText != null)
            {
                float critChancePercent = _stats.Weapon.criticalHitChance * 100f;
                _weaponCritText.text = $"Крит: {critChancePercent:F0}% (x{_stats.Weapon.criticalDamageMultiplier:F1})";
            }

            if (_weaponFireRateText != null) 
                _weaponFireRateText.text = $"Скорострельность: {_stats.Weapon.fireRateRPM} RPM";

            if (_weaponRangeText != null) 
                _weaponRangeText.text = $"Дальность: {_stats.Weapon.range}";

            if (_weaponBulletSpeedText != null)
                _weaponBulletSpeedText.text = $"Скорость пули: {_stats.Weapon.bulletSpeed}";
        }
        else
        {
            if (_weaponDamageText != null) _weaponDamageText.text = "Урон: 0";
            if (_weaponCritText != null) _weaponCritText.text = "Крит: 0";
            if (_weaponFireRateText != null) _weaponFireRateText.text = "Скорострельность: 0";
            if (_weaponRangeText != null) _weaponRangeText.text = "Дальность: 0";
            if (_weaponBulletSpeedText != null) _weaponBulletSpeedText.text = "Скорость пули: 0";
        }

        if (_weaponAngleText != null)
        {
            float angleLimit = _stats.Cab != null ? _stats.Cab.weaponRotationLimit : 0f;
            _weaponAngleText.text = $"Угол обстрела: {angleLimit}°";
        }
    }
}