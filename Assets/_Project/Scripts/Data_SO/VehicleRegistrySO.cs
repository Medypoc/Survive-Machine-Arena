using UnityEngine;
using System.Collections.Generic;

namespace SurviveArena.Data
{
    [CreateAssetMenu(fileName = "VehicleRegistry", menuName = "SurviveArena/Vehicle/Registry")]
    public class VehicleRegistry_SO : ScriptableObject
    {
        // [System.Serializable] обязателен, чтобы эта структура отображалась в Инспекторе Unity
        [System.Serializable]
        public class VehicleClassContent
        {
            [Tooltip("Перетащите сюда ассет класса машины (например, HeavyTruck_Class)")]
            public VehicleClassSO vehicleClass; 
            
            [Header("Available Parts")]
            public List<BodyDataSO> availableBodies = new List<BodyDataSO>();
            public List<CabDataSO> availableCabs = new List<CabDataSO>();
            public List<WeaponDataSO> availableWeapons = new List<WeaponDataSO>();
        }

        [Header("Vehicle Classes Database")]
        [Tooltip("Добавьте сюда все классы машин и запчасти к ним")]
        public List<VehicleClassContent> classes = new List<VehicleClassContent>();

        /// <summary>
        /// Возвращает случайный набор запчастей для указанного класса машины.
        /// Используется WaveManager'ом для генерации врагов с правильными деталями.
        /// </summary>
        public void GetRandomParts(VehicleClassSO vClass, out BodyDataSO body, out CabDataSO cab, out WeaponDataSO weapon)
        {
            body = null;
            cab = null;
            weapon = null;

            // Защита: проверяем, что нам вообще передали класс для поиска
            if (vClass == null) 
            {
                Debug.LogWarning("VehicleRegistry: Передан пустой VehicleClassSO для генерации запчастей!");
                return;
            }

            // Ищем контент, привязанный к конкретному ассету класса
            var content = classes.Find(x => x.vehicleClass == vClass);
            
            if (content != null)
            {
                // Выбираем случайный кузов (если список не пуст)
                if(content.availableBodies.Count > 0)
                    body = content.availableBodies[Random.Range(0, content.availableBodies.Count)];
                
                // Выбираем случайную кабину
                if(content.availableCabs.Count > 0)
                    cab = content.availableCabs[Random.Range(0, content.availableCabs.Count)];
                
                // Выбираем случайное оружие
                if(content.availableWeapons.Count > 0)
                    weapon = content.availableWeapons[Random.Range(0, content.availableWeapons.Count)];
            }
            else
            {
                // Полезный лог, если вы пытаетесь заспавнить врага класса, которого нет в Реестре
                Debug.LogWarning($"VehicleRegistry: В реестре не найдены данные для класса {vClass.name}! " +
                                 $"Убедитесь, что вы добавили его в массив 'classes' в ассете VehicleRegistry.");
            }
        }
    }
}