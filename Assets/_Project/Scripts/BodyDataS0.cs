using UnityEngine;

[CreateAssetMenu(fileName = "NewBody", menuName = "SurviveArena/Body Data")]
public class BodyDataSO : PartDataSO
{
    [Header("Stats")]
    public int additionalHP = 100;
    public float armor = 0.1f;
    public float fuelCapacity = 50f; 

    [Header("Mounting Points")]
    // Координаты, где на этом кузове должна стоять кабина
    public Vector2 cabinAnchorPoint; 

    [Header("Inventory")]
    public int inventorySlots = 8;
}