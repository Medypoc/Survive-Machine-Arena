using UnityEngine;
using SurviveArena.Data;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "SurviveArena/Weapon Data")]
public class WeaponDataSO : PartDataSO // НАСЛЕДОВАНИЕ: теперь у пушки есть partSprite
{
    [Header("Combat Stats")]
    public float damage = 10f;
    public float fireRate = 0.5f;
    public float range = 15f;
    public float bulletSpeed = 20f;
    public float rotationSpeed = 90f;

    [Header("Visuals")]
    public GameObject bulletPrefab;
    public AudioClip fireSound;
}