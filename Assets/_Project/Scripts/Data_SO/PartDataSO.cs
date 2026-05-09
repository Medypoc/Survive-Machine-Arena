using UnityEngine;

namespace SurviveArena.Data
{
    public abstract class PartDataSO : ScriptableObject
    {
        public string partName;
        public Sprite partSprite;
        
        [Tooltip("Для какого класса машины предназначена эта деталь?")]
        public VehicleClassSO requiredClass;
        
        [Header("Rewards")]
        public int baseXP = 10;

        [Header("Price")]
        public int price = 100;
    }
}