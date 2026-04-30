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

    public event Action OnStatsChanged;

    private void Awake() => RefreshStats();

    public void LoadModules(BodyDataSO body, CabDataSO cab, WeaponDataSO weapon)
    {
        _bodyData = body;
        _cabData = cab;
        _weaponData = weapon;
        RefreshStats();
    }

    public void RefreshStats()
    {
        Acceleration = 0; SteeringSpeed = 0; Armor = 0; MaxHealth = 100;

        if (_cabData != null)
        {
            Acceleration = _cabData.baseAcceleration;
            SteeringSpeed = _cabData.steeringSpeed;
            Armor = _cabData.armor;
            MaxHealth += _cabData.additionalHP;
        }

        var health = GetComponent<Health>();
        if (health != null) health.maxHealth = MaxHealth;

        OnStatsChanged?.Invoke();
    }
}