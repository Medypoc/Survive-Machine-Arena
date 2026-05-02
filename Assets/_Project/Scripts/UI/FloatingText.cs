using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMesh;
    [SerializeField] private float _fadeSpeed = 2f;
    [SerializeField] private float _destroyTime = 1.5f;
    
    private Vector3 _velocity;
    private Color _textColor;

    public void Setup(string text, Color color, float size, Vector3 initialVelocity)
    {
        _textMesh.text = text;
        _textMesh.color = color;
        _textColor = color;
        _textMesh.fontSize = size;
        _velocity = initialVelocity;

        // Настройка слоев видимости
        var renderer = _textMesh.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "UI"; 
        renderer.sortingOrder = 100;

        Destroy(gameObject, _destroyTime);
    }

    private void Update()
    {
        // Движение на основе скорости
        transform.position += _velocity * Time.deltaTime;

        // Постепенное замедление (трение), чтобы текст останавливался
        _velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.deltaTime * 2f);

        // Исчезновение
        _textColor.a -= _fadeSpeed * Time.deltaTime;
        _textMesh.color = _textColor;
    }
}