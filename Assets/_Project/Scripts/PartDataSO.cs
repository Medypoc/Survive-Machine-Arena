using UnityEngine;

// Этот класс не будет создаваться напрямую, он нужен как основа для других
public abstract class PartDataSO : ScriptableObject
{
    [Header("Visuals")]
    public string partName;
    public Sprite partSprite; // Изображение детали

    [Header("Physical Stats")]
    public float weight; // Вес, который влияет на расход топлива
}