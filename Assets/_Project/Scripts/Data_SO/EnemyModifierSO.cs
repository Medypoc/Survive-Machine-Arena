using UnityEngine;

namespace SurviveArena.Data
{
    [CreateAssetMenu(fileName = "NewEnemyModifier", menuName = "SurviveArena/Enemy Modifier")]
    public class EnemyModifierSO : ScriptableObject
    {
        [Header("Modifier Stats")]
        public string modifierName = "Elite";
        public float healthMultiplier = 1.5f;
        public float speedMultiplier = 1.2f;
        public float damageMultiplier = 1.2f;
        
        [Header("Visuals")]
        public Color visualTint = Color.red;
    }
}