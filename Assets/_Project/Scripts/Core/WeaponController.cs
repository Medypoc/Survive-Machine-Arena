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
        if (_stats == null || _stats.Weapon == null) return;

        Vector2 targetDir;

        if (target != null)
        {
            // Логика для ИИ: направление на цель
            targetDir = target.position - transform.position;
        }
        else if (transform.root.CompareTag("Player"))
        {
            // Логика для игрока: направление на мышь
            Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            targetDir = mousePos - transform.position;
        }
        else return;

        float targetWorldAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
        
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetWorldAngle);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation, 
            _stats.Weapon.rotationSpeed * Time.deltaTime
        );

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