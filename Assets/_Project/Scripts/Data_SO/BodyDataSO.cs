using UnityEngine;

namespace SurviveArena.Data
{
    [CreateAssetMenu(fileName = "NewBodyData", menuName = "SurviveArena/Part/Body")]
    public class BodyDataSO : PartDataSO
    {
        [Header("Body Stats")]
        public int additionalHP = 50;
        public int inventorySlots = 1;
        public float armor = 0.2f;
        
        [Header("Weight & Fuel")]
        public float weight = 800f;
        public float fuelCapacity = 100f;
    }
}