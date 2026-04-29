using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private VehicleStats stats;
    private Transform vehicleTransform;

    void Start()
    {
        stats = GetComponentInParent<VehicleStats>();
        // Кэшируем трансформ родителя (машины)
        vehicleTransform = stats.transform;
    }

    void Update()
    {
        RotateWeapon();
        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.red);
    }

    void RotateWeapon()
    {
        // 1. Получаем позицию мыши в мировых координатах
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 2. Переводим мировую позицию мыши в ЛОКАЛЬНУЮ позицию относительно машины
        // Это "магическая" функция, которая убирает влияние вращения корпуса на расчеты
        Vector3 localMousePos = vehicleTransform.InverseTransformPoint(mouseWorldPos);

        // 3. Считаем угол в локальном пространстве
        float targetAngle = Mathf.Atan2(localMousePos.y, localMousePos.x) * Mathf.Rad2Deg - 90f;

        // 4. Ограничиваем угол (Clamp) согласно статам кабины
        float limit = stats.cabData.weaponRotationLimit / 2f;
        // Зажимаем угол между -лимит и +лимит
        float clampedAngle = Mathf.Clamp(targetAngle, -limit, limit);

        // 5. Применяем вращение ЛОКАЛЬНО
        // Используем RotateTowards для плавности, чтобы пушка не дергалась
        Quaternion targetRotation = Quaternion.Euler(0, 0, clampedAngle);
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation, 
            targetRotation, 
            stats.weaponData.rotationSpeed * 100f * Time.deltaTime
        );
    }
}