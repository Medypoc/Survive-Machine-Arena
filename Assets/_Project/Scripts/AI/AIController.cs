using UnityEngine;

public class AIController : MonoBehaviour
{
    private VehicleMovement _movement;
    private VehicleStats _stats;
    private WeaponFire _weaponFire;
    private WeaponController _weaponController;
    private Transform _player;

    [Header("Orbit Settings")]
    [SerializeField] private float _orbitRadius = 7f;      // Дистанция кружения
    [SerializeField] private float _orbitThreshold = 2f;   // Допуск (насколько можно отклоняться от круга)
    
    [Header("Combat Settings")]
    [SerializeField] private float _shootAngleMargin = 15f; // Угол точности пушки для выстрела

    private int _orbitDirection = 1; // 1 - по часовой, -1 - против часовой
    private float _directionSwitchTime;

    void Awake()
    {
        _movement = GetComponent<VehicleMovement>();
        _stats = GetComponent<VehicleStats>();
        _weaponFire = GetComponentInChildren<WeaponFire>();
        _weaponController = GetComponentInChildren<WeaponController>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    void Start()
    {
        if (_weaponController != null) _weaponController.target = _player;
        // Случайно выбираем направление кружения при старте
        _orbitDirection = Random.value > 0.5f ? 1 : -1;
    }

    void FixedUpdate()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);
        
        // Меняем направление кружения раз в 5-10 секунд, чтобы быть непредсказуемым
        if (Time.time > _directionSwitchTime)
        {
            _orbitDirection *= -1;
            _directionSwitchTime = Time.time + Random.Range(5f, 10f);
        }

        HandleAdvancedMovement(distance);
        HandleCombat(distance);
    }

    private void HandleAdvancedMovement(float distance)
    {
        Vector2 vectorToPlayer = (_player.position - transform.position).normalized;
        Vector2 desiredDirection;

        if (distance > _orbitRadius + _orbitThreshold)
        {
            // СОСТОЯНИЕ 1: Сближение
            // Едем прямо на игрока
            desiredDirection = vectorToPlayer;
        }
        else if (distance < _orbitRadius - _orbitThreshold)
        {
            // СОСТОЯНИЕ 2: Отход
            // Мы слишком близко, нужно отъехать
            desiredDirection = -vectorToPlayer;
        }
        else
        {
            // СОСТОЯНИЕ 3: Орбита (Кружение)
            // Вычисляем перпендикуляр к вектору игрока (касательную)
            Vector2 orbitTangent = new Vector2(-vectorToPlayer.y, vectorToPlayer.x) * _orbitDirection;
            
            // Смешиваем касательную с небольшим вектором "к игроку", чтобы не улетать по инерции
            desiredDirection = (orbitTangent + vectorToPlayer * 0.2f).normalized;
        }

        // РУЛЕЖКА
        float angleToTarget = Vector2.SignedAngle(transform.up, desiredDirection);
        float steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        // ГАЗ (снижаем скорость на крутых поворотах, чтобы не было заносов)
        float gasInput = 1f;
        if (Mathf.Abs(steerInput) > 0.7f) gasInput = 0.5f;

        _movement.Move(gasInput, steerInput);
    }

    private void HandleCombat(float distance)
    {
        if (_weaponFire == null || _weaponFire.firePoint == null) return;

        // Стреляем, если игрок в радиусе и пушка смотрит на него
        if (distance <= _stats.Weapon.range)
        {
            Vector2 dirToPlayer = (_player.position - _weaponFire.firePoint.position).normalized;
            float angleToTarget = Vector2.Angle(_weaponFire.firePoint.up, dirToPlayer);

            if (angleToTarget < _shootAngleMargin)
            {
                _weaponFire.Shoot();
            }
        }
    }
}