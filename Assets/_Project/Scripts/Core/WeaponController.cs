using UnityEngine;
using SurviveArena.Data;

public class WeaponController : MonoBehaviour
{
    private VehicleStats _stats;
    private Camera _cam;
    
    [Header("AI Settings")]
    public Transform target; // Если заполнено, пушка следит за целью, а не за мышью

    void Start()
    {
        _stats = GetComponentInParent<VehicleStats>();
        _cam = Camera.main;
    }

    void Update()
    {
        // 1. Проверка необходимых компонентов
        if (_stats == null || _stats.Weapon == null) return;

        Vector2 direction;

        // 2. Определение цели: приоритет у назначенного Transform (для ИИ), 
        // затем проверка на игрока (для мыши)
        if (target != null)
        {
            direction = (Vector2)target.position - (Vector2)transform.position;
        }
        else if (transform.root.CompareTag("Player"))
        {
            Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            direction = (Vector2)mousePos - (Vector2)transform.position;
        }
        else 
        {
            // Если цели нет и это не игрок — ничего не делаем
            return;
        }

        // 3. Вычисление целевого угла
        // Atan2 возвращает угол в радианах, переводим в градусы. 
        // -90f нужно, если спрайт пушки изначально смотрит вверх.
        float targetWorldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetWorldAngle);

        // 4. Плавный поворот в сторону цели
        // Используем rotationSpeed из настроек оружия
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation, 
            _stats.Weapon.rotationSpeed * Time.deltaTime
        );

        // 5. Ограничение угла поворота относительно кабины
        ApplyRotationLimit();
    }

    private void ApplyRotationLimit()
    {
        if (_stats.Cab == null || _stats.Cab.weaponRotationLimit >= 360f) return;

        float localAngle = transform.localEulerAngles.z;
        if (localAngle > 180) localAngle -= 360;

        float halfLimit = _stats.Cab.weaponRotationLimit * 0.5f;
        float clampedAngle = Mathf.Clamp(localAngle, -halfLimit, halfLimit);

        transform.localRotation = Quaternion.Euler(0, 0, clampedAngle);
    }
}