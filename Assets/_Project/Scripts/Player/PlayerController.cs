using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
public class PlayerController : MonoBehaviour
{
    private VehicleMovement movement;
    private float gas;
    private float steer;
    private bool handbrake;

    void Awake()
    {
        movement = GetComponent<VehicleMovement>();
    }

    void Update()
    {
        // Считываем "одиночные" нажатия в Update, чтобы не пропустить кадр[cite: 8]
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movement.Dash();
        }

        // Считываем оси и удержание ручника[cite: 8]
        gas = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        handbrake = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        // Передаем приказ Телу в физическом цикле[cite: 8]
        movement.Move(gas, steer, handbrake);
    }
}