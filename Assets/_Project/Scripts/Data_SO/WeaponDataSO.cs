using UnityEngine;

namespace SurviveArena.Data
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "SurviveArena/Weapon Data")]
    public class WeaponDataSO : PartDataSO
    {
        [Header("Weapon Models (Префабы)")]
        [Tooltip("Префаб самой пушки (башни), который крепится к машине")]
        public GameObject weaponPrefab; 
        
        [Tooltip("Префаб пули/снаряда")]
        public GameObject bulletPrefab;

        [Header("Weapon Statistics")]
        [Space(10)]
        public DamageStats damageStats;
        
        [Space(10)]
        public ShootingStats shootingStats;
        
        [Header("Visuals & Audio")]
        [Space(10)]
        public WeaponVisuals visuals;

        // --- ВСПОМОГАТЕЛЬНЫЕ СВОЙСТВА ---
        // Автоматически переводит выстрелы в минуту (RPM) во время между выстрелами (в секундах)
        public float FireCooldown => 60f / shootingStats.fireRateRPM;
    }

    // =========================================================
    // СТРУКТУРЫ ДАННЫХ (Группировка параметров для Инспектора)
    // =========================================================

    [System.Serializable]
    public struct DamageStats
    {
        [Tooltip("Минимальный базовый урон")]
        public float minDamage;
        
        [Tooltip("Максимальный базовый урон")]
        public float maxDamage;
        
        [Tooltip("Шанс критического удара (0.0 = 0%, 1.0 = 100%)")]
        [Range(0f, 1f)] public float criticalHitChance;
        
        [Tooltip("Во сколько раз увеличится урон при крите (например, 2.0 = двойной урон)")]
        public float criticalDamageMultiplier;
    }

    [System.Serializable]
    public struct ShootingStats
    {
        [Tooltip("Количество патронов в магазине")]
        public int magazineSize;
        
        [Tooltip("Время перезарядки (в секундах)")]
        public float reloadTime;
        
        [Tooltip("Скорострельность: Выстрелов в минуту (RPM)")]
        public float fireRateRPM;
        
        [Tooltip("Скорость полета пули")]
        public float bulletSpeed;
        
        [Tooltip("Максимальная дальность стрельбы")]
        public float range;

        // --- ДОБАВЛЕНА ПОТЕРЯННАЯ ПЕРЕМЕННАЯ ---
        [Tooltip("Скорость поворота башни (градусов в секунду)")]
        public float rotationSpeed; 
    }
    
    // --- ТА САМАЯ НЕДОСТАЮЩАЯ СТРУКТУРА ---
    [System.Serializable]
    public struct WeaponVisuals
    {
        [Tooltip("Звук при выстреле")]
        public AudioClip fireSound;
        
        [Tooltip("Эффект вспышки от выстрела (спавнится на конце ствола)")]
        public GameObject muzzleFlashPrefab;
    }
}