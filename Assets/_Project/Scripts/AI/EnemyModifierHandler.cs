using UnityEngine;
using SurviveArena.Data;

namespace SurviveArena.Core
{
    public class EnemyModifierHandler : MonoBehaviour
    {
        [Header("Manual Setup")]
        // Если перетащить сюда модификатор в инспекторе, он применится при старте[cite: 6]
        [SerializeField] private EnemyModifierSO _manualModifier; 

        private void Start()
        {
            if (_manualModifier != null)
            {
                ApplyModifier(_manualModifier);
            }
        }

        public void ApplyModifier(EnemyModifierSO modifier)
        {
            if (modifier == null) return;

            // Передаем коэффициенты в VehicleStats[cite: 6]
            VehicleStats stats = GetComponent<VehicleStats>();
            if (stats != null)
            {
                stats.ApplyModifiers(modifier.healthMultiplier, modifier.speedMultiplier, modifier.damageMultiplier);
            }

            // Передаем цвет в VehicleVisual[cite: 6]
            VehicleVisual visual = GetComponent<VehicleVisual>();
            if (visual != null)
            {
                visual.ApplyTint(modifier.visualTint);
            }
        }
    }
}