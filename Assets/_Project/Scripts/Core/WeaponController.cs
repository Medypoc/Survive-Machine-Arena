using UnityEngine;
using SurviveArena.Data;

public class WeaponController : MonoBehaviour
{
    private VehicleStats _stats;
    private Vector3 _targetPoint;
    private bool _hasTarget;

    void Start()
    {
        _stats = GetComponentInParent<VehicleStats>();
    }

    // Метод, который будут вызывать ИИ или Игрок
    public void SetTargetPoint(Vector3 worldPoint)
    {
        _targetPoint = worldPoint;
        _hasTarget = true;
    }

    void Update()
    {
        // Если пушка еще не инициализирована или нет цели — не вращаем
        if (_stats == null || _stats.Weapon == null || !_hasTarget) return;

        Vector2 direction = (Vector2)_targetPoint - (Vector2)transform.position;

        // Расчет целевого угла
        float targetWorldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetWorldAngle);

        // Плавный поворот с использованием статов оружия
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation, 
            _stats.Weapon.shootingStats.rotationSpeed * Time.deltaTime 
        );

        // Ограничение угла (лимиты кабины)
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