using UnityEngine;

public class PlayerWeaponAimer : MonoBehaviour
{
    private WeaponController _weaponController;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        // Динамический поиск контроллера пушки
        // Мы ищем его каждый кадр, пока не найдем (на случай, если пушка спавнится не сразу)
        if (_weaponController == null)
        {
            _weaponController = GetComponentInChildren<WeaponController>();
            if (_weaponController == null) return;
        }

        // 1. Получаем позицию мыши в мировых координатах
        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Нам не нужна глубина в 2D

        // 2. Передаем эту точку в контроллер пушки
        _weaponController.SetTargetPoint(mousePos);
    }
}