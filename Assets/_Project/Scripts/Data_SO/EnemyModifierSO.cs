using UnityEngine;

namespace SurviveArena.Data 
{
    // 1. Создаем список уровней сложности
    public enum DifficultyTier 
    { 
        Simple, 
        Medium, 
        Hard 
    }

    [CreateAssetMenu(fileName = "NewEnemyModifier", menuName = "SurviveArena/Enemy Modifier")]
    public class EnemyModifierSO : ScriptableObject
    {
        [Header("Combat Modifiers")]
        public float healthMultiplier = 1.0f;
        public float speedMultiplier = 1.0f;
        public float damageMultiplier = 1.0f;
        public Color visualTint = Color.white;

        // --- БЛОК ДЛЯ ЭКОНОМИКИ ХРАНИТСЯ ЗДЕСЬ ---
        [Header("Rewards")]
        public DifficultyTier tier = DifficultyTier.Simple; 
        public float xpMultiplier = 1.0f; 
    }
}