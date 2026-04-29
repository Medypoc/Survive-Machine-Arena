using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private VehicleStats stats;
    private Transform vehicleTransform;

    void Start()
    {
        // Ищем VehicleStats в родителе (Player)
        stats = GetComponentInParent<VehicleStats>();
        
        if (stats != null)
        {
            vehicleTransform = stats.transform;
        }
        else
        {
            Debug.LogError("WeaponController: Не найден компонент VehicleStats на родителе!");
        }
    }

    // Update — это стандартный метод Unity, он должен вызывать RotateWeapon
    void Update()
    {
        if (stats == null || vehicleTransform == null) return;

        RotateWeapon();
    }

    // Теперь это обычный метод класса, а не "локальная функция"
    void RotateWeapon()
    {
        // 1. Получаем позицию мыши в мире
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 2. Переводим позицию мыши в ЛОКАЛЬНЫЕ координаты игрока
        // Это позволяет игнорировать текущий разворот машины при расчете лимитов
        Vector3 localMousePos = vehicleTransform.InverseTransformPoint(mouseWorldPos);

        // 3. Считаем угол между "носом" машины (Vector2.up) и направлением на мышь
        // SignedAngle возвращает значения от -180 до 180
        float targetAngle = Vector2.SignedAngle(Vector2.up, localMousePos);

        // 4. Ограничиваем угол согласно статам кабины
        // Если лимит 90 градусов, то пушка будет ходить от -45 до +45
        float limit = stats.cabData.weaponRotationLimit / 2f;
        float clampedAngle = Mathf.Clamp(targetAngle, -limit, limit);

        // 5. Плавное вращение (RotateTowards)
        // Вращаем текущий объект (Weapon_Slot) относительно родителя
        Quaternion targetRotation = Quaternion.Euler(0, 0, clampedAngle);
        
        float rotationSpeed = stats.weaponData.rotationSpeed * 10f; // Коэффициент скорости
        
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmos()
    {
        if (stats == null || stats.cabData == null || vehicleTransform == null) return;

        Vector3 origin = transform.position;
        float limit = stats.cabData.weaponRotationLimit / 2f;
        Vector3 baseDirection = vehicleTransform.up;

        // 1. ОТРИСОВКА ГРАНИЦ (Желтые линии)
        Gizmos.color = Color.yellow;
        Vector3 leftLimitDir = Quaternion.Euler(0, 0, limit) * baseDirection;
        Vector3 rightLimitDir = Quaternion.Euler(0, 0, -limit) * baseDirection;

        Gizmos.DrawRay(origin, leftLimitDir * 2f); 
        Gizmos.DrawRay(origin, rightLimitDir * 2f);

        // 2. ОТРИСОВКА ЛУЧА ПРИЦЕЛИВАНИЯ (Яркий длинный луч)
        // Используем transform.up, так как это текущий поворот пушки
        Gizmos.color = new Color(1f, 1f, 0f, 1f); // Насыщенный желтый
        Gizmos.DrawRay(origin, transform.up * 5f); // Длина 5 метров

        // 3. ОТРИСОВКА ТОЧКИ ПОПАДАНИЯ (Маленькая сфера на конце луча)
        Gizmos.DrawWireSphere(origin + transform.up * 5f, 0.1f);

        // Бонус: Сектор обстрела
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(1, 1, 0, 0.05f);
        UnityEditor.Handles.DrawSolidArc(origin, Vector3.forward, leftLimitDir, -limit * 2, 2f);
#endif
    }
}