using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(VehicleStats))]
public class VehicleMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private VehicleStats _stats;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _stats = GetComponent<VehicleStats>();
    }

    public void Move(float gasInput, float steerInput)
    {
        if (_stats == null) return;

        _rb.AddForce(transform.up * gasInput * _stats.Acceleration);

        float speedFactor = Mathf.Clamp01(_rb.linearVelocity.magnitude / 5f);
        _rb.AddTorque(steerInput * _stats.SteeringSpeed * speedFactor * -1f);
    }

    public void ApplyBrake(float force)
    {
        _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, force * Time.deltaTime);
        _rb.angularVelocity = Mathf.Lerp(_rb.angularVelocity, 0, force * Time.deltaTime);
    }
}