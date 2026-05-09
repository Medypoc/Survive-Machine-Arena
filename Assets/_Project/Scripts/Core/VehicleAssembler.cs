using UnityEngine;
using SurviveArena.Data;
using System.Collections.Generic;

public class VehicleAssembler : MonoBehaviour
{
    [Header("Visual Renderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer cabinRenderer;
    public Transform weaponSlot;

    [Header("Physics Hitboxes (Polygon)")]
    public PolygonCollider2D bodyCollider;
    public PolygonCollider2D cabinCollider;

    private GameObject _currentWeaponInstance;

    // Метод принимает готовые SO деталей и собирает визуал
    public void Assemble(BodyDataSO body, CabDataSO cab, WeaponDataSO weapon)
    {
        // 1. Устанавливаем кузов
        if (body != null) 
        {
            bodyRenderer.sprite = body.partSprite;
            UpdateCollider(bodyCollider, body.partSprite);
        }

        // 2. Устанавливаем кабину
        if (cab != null) 
        {
            cabinRenderer.sprite = cab.partSprite;
            UpdateCollider(cabinCollider, cab.partSprite);

            // --- НОВОЕ: Двигаем слот оружия туда, куда хочет кабина ---
            if (weaponSlot != null)
            {
                // Применяем смещение из ScriptableObject кабины
                weaponSlot.localPosition = new Vector3(cab.weaponSlotOffset.x, cab.weaponSlotOffset.y, 0);
            }
        }

        // 3. Устанавливаем пушку (она появится уже в новой позиции слота)
        if (weapon != null && weapon.weaponPrefab != null)
        {
            if (_currentWeaponInstance != null) Destroy(_currentWeaponInstance);
            
            _currentWeaponInstance = Instantiate(weapon.weaponPrefab, weaponSlot);
            _currentWeaponInstance.transform.localPosition = Vector3.zero;
        }
    }

    // Авто-генерация формы коллайдера по контуру спрайта
    private void UpdateCollider(PolygonCollider2D col, Sprite newSprite)
    {
        if (col == null || newSprite == null) return;
        
        col.pathCount = newSprite.GetPhysicsShapeCount();
        List<Vector2> path = new List<Vector2>();
        
        for (int i = 0; i < col.pathCount; i++)
        {
            newSprite.GetPhysicsShape(i, path);
            col.SetPath(i, path);
        }
    }

    // --- НОВЫЙ МЕТОД ДЛЯ ПОКРАСКИ ВРАГОВ ---
    public void ApplyTint(Color tintColor)
    {
        if (bodyRenderer != null) bodyRenderer.color = tintColor;
        if (cabinRenderer != null) cabinRenderer.color = tintColor;

        // Красим также все спрайты на префабе пушки
        if (_currentWeaponInstance != null)
        {
            SpriteRenderer[] weaponRenderers = _currentWeaponInstance.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in weaponRenderers)
            {
                sr.color = tintColor;
            }
        }
    }
}