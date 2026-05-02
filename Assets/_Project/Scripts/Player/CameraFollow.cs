using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Zoom Settings")]
    [SerializeField] private float _minZoom = 10f;
    [SerializeField] private float _maxZoom = 20f;
    [SerializeField] private float _zoomStep = 1f;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        
        // Если скрипт висит не на самой камере, пытаемся найти основную
        if (_cam == null) _cam = Camera.main;
    }

    private void LateUpdate()
    {
        // 1. Логика следования за целью
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        // 2. Логика зума
        HandleZoom();
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            // В Unity положительный scroll — это прокрутка "вверх" (приближение)
            // Для приближения в 2D нужно УМЕНЬШАТЬ orthographicSize
            float direction = (scrollInput > 0) ? -1f : 1f;
            
            float targetSize = _cam.orthographicSize + (direction * _zoomStep);

            // Ограничиваем зум в пределах 10 - 20
            _cam.orthographicSize = Mathf.Clamp(targetSize, _minZoom, _maxZoom);
        }
    }
}