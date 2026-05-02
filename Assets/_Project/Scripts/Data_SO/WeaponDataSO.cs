using UnityEngine;
using SurviveArena.Data;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "SurviveArena/Weapon Data")]
public class WeaponDataSO : PartDataSO // НАСЛЕДОВАНИЕ: теперь у пушки есть partSprite
{
    [Header("Damage Stats")]
    public float minDamage = 4f;
    public float maxDamage = 10f;
    
    [Tooltip("Шанс критического удара. 0.05 = 5%")]
    [Range(0f, 1f)] 
    public float criticalHitChance = 0.05f;
    
    [Tooltip("Множитель критического урона. 1.2 = 120%")]
    public float criticalDamageMultiplier = 1.2f;

    [Header("Shooting Stats")]
    [Tooltip("Скорострельность: Выстрелов в минуту (RPM)")]
    public float fireRateRPM = 300f; 
    public float bulletSpeed = 30f;
    public float range = 20f;       
    public float rotationSpeed = 90f;

    [Header("Visuals")]
    public GameObject bulletPrefab;
    public AudioClip fireSound;

    // --- УМНЫЕ СВОЙСТВА ДЛЯ КОДА ---

    // Конвертируем выстрелы в минуту в задержку в секундах для кулдауна стрельбы
    // 300 RPM = 60 / 300 = 0.2 секунды между выстрелами
    public float FireCooldown => 60f / fireRateRPM; 
}