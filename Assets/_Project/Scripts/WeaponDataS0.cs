using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "SurviveArena/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;

    [Header("Combat Stats")]
    public int damage = 10;
    public float fireRate = 0.5f; // Задержка между выстрелами
    public float bulletSpeed = 20f;
    public float rotationSpeed = 5f; // Скорость доводки ствола до курсора
    public float range = 15f; // Дальность полета в метрах

    [Header("Visuals & Sound")]
    public GameObject bulletPrefab; // Ссылка на префаб пули
    public AudioClip fireSound;
}