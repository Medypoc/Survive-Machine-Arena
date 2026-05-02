using UnityEngine;
using SurviveArena.Core;
using SurviveArena.Data;

[CreateAssetMenu(fileName = "NewCab", menuName = "SurviveArena/Part/Cab Data")]

public class CabDataSO : PartDataSO
{
    [Header("Movement")]
    public float baseSpeed = 10f;
    public float baseAcceleration = 5f;
    public float steeringSpeed = 200f;
    
    [Header("Weight & Fuel")]
    public float weight = 200f;

    [Header("Combat & Defense")]
    public int additionalHP = 50;
    public float armor = 0.2f; 
    public float weaponRotationLimit = 180f; 
}