using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
public class PlayerController : MonoBehaviour
{
    private VehicleMovement movement;

    void Awake()
    {
        movement = GetComponent<VehicleMovement>();
    }

    void FixedUpdate()
    {
        // Считываем кнопки клавиатуры (-1 до 1)
        float gas = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");

        // Передаем приказ Телу
        movement.Move(gas, steer);
    }
}