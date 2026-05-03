using UnityEngine;

namespace SurviveArena.Data 
{
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

        [Header("Rewards")]
        public DifficultyTier tier = DifficultyTier.Simple; 
        public float xpMultiplier = 1.0f; 
        
        // --- НОВЫЕ ПАРАМЕТРЫ ДЕНЕЖНОЙ НАГРАДЫ ---
        [Tooltip("Минимальное количество денег за убийство")]
        public int minMoneyReward = 0;
        
        [Tooltip("Максимальное количество денег за убийство")]
        public int maxMoneyReward = 20;
    }
}