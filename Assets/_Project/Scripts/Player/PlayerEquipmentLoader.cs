using UnityEngine;

public class PlayerEquipmentLoader : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private PlayerDataSO _playerProfile;

    private void Start()
    {
        if (_playerProfile == null) return;

        // Ищем компонент VehicleStats на нашей машине
        VehicleStats stats = GetComponent<VehicleStats>();
        
        if (stats != null)
        {
            // Передаем сохраненные детали в метод загрузки
            stats.LoadModules(
                _playerProfile.equippedBody, 
                _playerProfile.equippedCab, 
                _playerProfile.equippedWeapon
            );

            // МЫ ПОЛНОСТЬЮ УДАЛИЛИ БЛОК РАБОТЫ СО ЗДОРОВЬЕМ ОТСЮДА,
            // так как PlayerPersistence уже сделал эту работу в Awake().
        }
    }
}