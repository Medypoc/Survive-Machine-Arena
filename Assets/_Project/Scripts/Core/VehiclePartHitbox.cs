using UnityEngine;

public enum VehiclePartType { Cab, Body }

public class VehiclePartHitbox : MonoBehaviour
{
    public VehiclePartType partType;
    public VehicleStats ownerStats;

    private void Awake()
    {
        // Автоматически находим статы, если они на родителе
        if (ownerStats == null)
            ownerStats = GetComponentInParent<VehicleStats>();
    }
}