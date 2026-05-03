using UnityEngine;

public class FuelItem : PickUpItem
{
    [Header("Fuel Settings")]
    public float fuelAmount = 25f;

    protected override bool OnPickedUp(GameObject recipient)
    {
        // Ищем компонент топлива на корне игрока[cite: 7]
        Fuel fuelComponent = recipient.GetComponent<Fuel>();

        if (fuelComponent != null)
        {
            // Логика восполнения
            fuelComponent.currentFuel = Mathf.Clamp(
                fuelComponent.currentFuel + fuelAmount, 
                0, 
                fuelComponent.maxFuel
            );
            
            // Оповещаем UI[cite: 7]
            fuelComponent.NotifyFuelChanged();
            
            Debug.Log($"[FuelItem] Восстановлено {fuelAmount} топлива.");
            return true; // Успешно подобрано
        }

        return false; // Не нашли компонент топлива
    }
}