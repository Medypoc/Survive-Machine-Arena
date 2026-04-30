using UnityEngine;

public class VehicleVisual : MonoBehaviour
{
    [Header("Renderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer cabRenderer;
    public SpriteRenderer weaponRenderer;

    private VehicleStats _stats;

    private void Awake()
    {
        _stats = GetComponent<VehicleStats>();
    }

    private void OnEnable()
    {
        if (_stats != null) _stats.OnStatsChanged += UpdateVisuals;
    }

    private void OnDisable()
    {
        if (_stats != null) _stats.OnStatsChanged -= UpdateVisuals;
    }

    public void UpdateVisuals()
    {
        if (_stats == null) return;

        // Теперь везде используем partSprite, так как все модули наследуются от PartDataSO
        if (_stats.Body != null && bodyRenderer != null) 
            bodyRenderer.sprite = _stats.Body.partSprite;

        if (_stats.Cab != null && cabRenderer != null) 
            cabRenderer.sprite = _stats.Cab.partSprite;

        if (_stats.Weapon != null && weaponRenderer != null) 
            weaponRenderer.sprite = _stats.Weapon.partSprite;
    }
}