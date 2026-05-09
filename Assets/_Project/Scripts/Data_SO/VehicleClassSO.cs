using UnityEngine;

namespace SurviveArena.Data
{
    [CreateAssetMenu(fileName = "NewVehicleClass", menuName = "SurviveArena/Vehicle Class")]
    public class VehicleClassSO : ScriptableObject
    {
        // Имя самого файла в Unity (например, "HeavyTruck_Class") будет служить идентификатором
        
        [Header("Base Core Stats")]
        public float baseHealth = 100f;
        public float baseFuel = 50f;
        
        [Header("Physics Prefab")]
        [Tooltip("Физический префаб шасси (со слотами Body, Cab, Weapon и скриптом VehicleAssembler)")]
        public GameObject chassisPrefab; 
    }
}