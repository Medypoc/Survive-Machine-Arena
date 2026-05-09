using UnityEngine;
using SurviveArena.Data;

[RequireComponent(typeof(VehicleStats))]
public class EnemyEquipmentLoader : MonoBehaviour
{
    [Header("Generation Settings")]
    [Tooltip("Ссылка на базу данных всех запчастей")]
    public VehicleRegistry_SO vehicleRegistry;
    
    [Tooltip("Пустой объект внутри врага, куда будет спавниться шасси")]
    public Transform chassisMountPoint;

    private VehicleAssembler _currentAssembler;

    // ИСПРАВЛЕНО: Теперь метод принимает 4 аргумента от WaveManager
    public void GenerateAndLoadEnemy(VehicleClassSO enemyClass, BodyDataSO randomBody, CabDataSO randomCab, WeaponDataSO randomWeapon)
    {
        if (enemyClass == null || chassisMountPoint == null) return;

        // 1. Очищаем старое шасси
        foreach (Transform child in chassisMountPoint)
        {
            Destroy(child.gameObject);
        }

        // 2. Спавним новое шасси
        if (enemyClass.chassisPrefab != null)
        {
            GameObject chassisInstance = Instantiate(enemyClass.chassisPrefab, chassisMountPoint);
            chassisInstance.transform.localPosition = Vector3.zero;
            chassisInstance.transform.localRotation = Quaternion.identity;

            _currentAssembler = chassisInstance.GetComponent<VehicleAssembler>();

            if (_currentAssembler != null)
            {
                // 3. Собираем визуал из тех деталей, что передал WaveManager
                _currentAssembler.Assemble(randomBody, randomCab, randomWeapon);
            }

            // 4. Загружаем статы
            VehicleStats stats = GetComponent<VehicleStats>();
            if (stats != null)
            {
                stats.LoadModules(randomBody, randomCab, randomWeapon);
            }
        }
        else
        {
            Debug.LogError($"EnemyEquipmentLoader: У класса {enemyClass.name} нет префаба шасси!");
        }
    }
}