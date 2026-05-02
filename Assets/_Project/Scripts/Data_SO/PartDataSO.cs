using UnityEngine;
using SurviveArena.Core; // Для доступа к VehicleClass

namespace SurviveArena.Data
{
    // Класс абстрактный, чтобы нельзя было создать "просто запчасть" без типа
    public abstract class PartDataSO : ScriptableObject
    {
        public string partName;
        public Sprite partSprite;
        public VehicleClass vehicleClass;
        
        [Header("Rewards")]
        public int baseXP = 10; // Базовый опыт за деталь
    }
}