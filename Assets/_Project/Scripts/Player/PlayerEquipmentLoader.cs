using UnityEngine;
using SurviveArena.Data;

// МЫ УБРАЛИ [RequireComponent], так как Assembler теперь живет на Шасси!

public class PlayerEquipmentLoader : MonoBehaviour
{
    [Header("Player Data")]
    [Tooltip("Ссылка на профиль игрока с экипировкой")]
    public PlayerDataSO playerProfile;

    [Header("Spawn Points")]
    [Tooltip("Пустой объект-пустышка в руте персонажа (например, Chassis_prefab)")]
    public Transform chassisMountPoint;

    private VehicleAssembler _currentAssembler;

    private void Start()
    {
        LoadVehicle();
    }

    public void LoadVehicle()
    {
        // 1. Базовые проверки на ошибки
        if (playerProfile == null)
        {
            Debug.LogError("PlayerEquipmentLoader: Профиль игрока не назначен!");
            return;
        }

        if (chassisMountPoint == null)
        {
            Debug.LogError("PlayerEquipmentLoader: Точка крепления шасси (chassisMountPoint) не назначена! Перетащите пустышку из иерархии.");
            return;
        }

        // 2. Очищаем старое шасси перед сборкой нового
        foreach (Transform child in chassisMountPoint)
        {
            Destroy(child.gameObject);
        }

        // 3. Узнаем, какой класс шасси нужен для текущего кузова
        if (playerProfile.equippedBody == null || playerProfile.equippedBody.requiredClass == null)
        {
            Debug.LogError("PlayerEquipmentLoader: В надетый кузов не добавлен 'requiredClass' (Класс машины)!");
            return;
        }

        VehicleClassSO currentClass = playerProfile.equippedBody.requiredClass;

        if (currentClass.chassisPrefab != null)
        {
            // 4. Спавним физическое Шасси в пустышку
            GameObject chassisInstance = Instantiate(currentClass.chassisPrefab, chassisMountPoint);
            chassisInstance.transform.localPosition = Vector3.zero;
            chassisInstance.transform.localRotation = Quaternion.identity;

            // 5. Ищем Сборщик (VehicleAssembler) на только что заспавненном шасси
            _currentAssembler = chassisInstance.GetComponent<VehicleAssembler>();

            if (_currentAssembler != null)
            {
                // 6. Даем команду Шасси надеть нужные спрайты и пушку
                _currentAssembler.Assemble(
                    playerProfile.equippedBody, 
                    playerProfile.equippedCab, 
                    playerProfile.equippedWeapon
                );
            }
            else
            {
                Debug.LogError($"PlayerEquipmentLoader: На префабе шасси '{chassisInstance.name}' нет скрипта VehicleAssembler!");
            }

            // 7. Передаем данные о деталях в главный мозг игрока (VehicleStats) для расчета ХП и Скорости
            VehicleStats stats = GetComponent<VehicleStats>();
            if (stats != null)
            {
                stats.LoadModules(
                    playerProfile.equippedBody, 
                    playerProfile.equippedCab, 
                    playerProfile.equippedWeapon
                );
            }
        }
        else
        {
            Debug.LogError($"PlayerEquipmentLoader: В классе '{currentClass.name}' не назначен префаб шасси (chassisPrefab)!");
        }
    }
}