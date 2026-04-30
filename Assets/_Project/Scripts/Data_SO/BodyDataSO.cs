using UnityEngine;

namespace SurviveArena.Data
{
    [CreateAssetMenu(fileName = "NewBodyData", menuName = "SurviveArena/Part/Body")]
    public class BodyDataSO : PartDataSO // Теперь он видит PartDataSO, так как они в одном namespace
    {
        [Header("Body Specific Stats")]
        public float weightPenalty = 0f;
        public int extraStorageSlots = 0;
    }
}