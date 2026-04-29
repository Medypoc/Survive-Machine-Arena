using UnityEngine;

public class VehiclePartSlot : MonoBehaviour
{
    // Ссылка на SpriteRenderer дочернего объекта (напр. Body_Visual)
    public SpriteRenderer visualRenderer;

    public void UpdatePart(Sprite newSprite, int sortingOrder)
    {
        if (visualRenderer != null)
        {
            visualRenderer.sprite = newSprite;
            visualRenderer.sortingOrder = sortingOrder;
            
            // Фиксируем только Z, чтобы спрайт не "проваливался", 
            // но сохраняем ваши ручные настройки X и Y из инспектора
            Vector3 currentPos = visualRenderer.transform.localPosition;
            visualRenderer.transform.localPosition = new Vector3(currentPos.x, currentPos.y, 0);
        }
    }
}