using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "VehicleRegistry", menuName = "Vehicle/Registry")]
public class VehicleRegistrySO : ScriptableObject
{
    [System.Serializable]
    public class VehicleClassContent
    {
        public VehicleStats.VehicleBase vehicleClass;
        public List<BodyDataSO> availableBodies; 
        public List<CabDataSO> availableCabs;
        public List<WeaponDataSO> availableWeapons;
    }

    public List<VehicleClassContent> classes = new List<VehicleClassContent>();

    public void GetRandomParts(VehicleStats.VehicleBase vClass, 
        out BodyDataSO body, out CabDataSO cab, out WeaponDataSO weapon)
{
    var content = classes.Find(x => x.vehicleClass == vClass);
    
    body = content.availableBodies[Random.Range(0, content.availableBodies.Count)];
    cab = content.availableCabs[Random.Range(0, content.availableCabs.Count)];
    weapon = content.availableWeapons[Random.Range(0, content.availableWeapons.Count)];
}
    
}