using UnityEngine;
using System.Collections.Generic;
using SurviveArena.Core; // Подключаем пространство имен, где лежит наш enum
using SurviveArena.Data;

[CreateAssetMenu(fileName = "VehicleRegistry", menuName = "Vehicle/Registry")]
public class VehicleRegistry_SO : ScriptableObject
{
    [System.Serializable]
    public class VehicleClassContent
    {
        // ИСПРАВЛЕНО: Убрали VehicleStats. и заменили на VehicleClass
        public VehicleClass vehicleClass; 
        
        public List<BodyDataSO> availableBodies;
        public List<CabDataSO> availableCabs;
        public List<WeaponDataSO> availableWeapons;
    }

    public List<VehicleClassContent> classes = new List<VehicleClassContent>();

    public void GetRandomParts(VehicleClass vClass, 
        out BodyDataSO body, out CabDataSO cab, out WeaponDataSO weapon)
    {
        body = null;
        cab = null;
        weapon = null;

        var content = classes.Find(x => x.vehicleClass == vClass);
        
        if (content != null)
        {
            if(content.availableBodies.Count > 0)
                body = content.availableBodies[Random.Range(0, content.availableBodies.Count)];
            
            if(content.availableCabs.Count > 0)
                cab = content.availableCabs[Random.Range(0, content.availableCabs.Count)];
            
            if(content.availableWeapons.Count > 0)
                weapon = content.availableWeapons[Random.Range(0, content.availableWeapons.Count)];
        }
    }
}