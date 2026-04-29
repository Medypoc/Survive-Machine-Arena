using UnityEngine;

// Базовый класс для всех запчастей
public abstract class PartDataSO : ScriptableObject
{
    [Header("Visuals")]
    public string partName;
    public Sprite partSprite; 

    [Header("Physical Stats")]
    public float weight; 

    [Header("Compatibility")]
    // Ссылаемся на перечисление, которое находится в скрипте VehicleStats
    public VehicleStats.VehicleBase compatibleBase; 
}