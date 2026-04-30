using UnityEngine;

public enum AIState { Chasing, Orbiting, Reversing }

public class AIController : MonoBehaviour
{
    [Header("Behavior Settings")]
    [SerializeField] private AIState _currentState = AIState.Chasing;
    [SerializeField] private float _attackRange = 12f;
    [SerializeField] private float _safetyDistance = 6f;

    [Header("Movement Tuning")]
    [SerializeField] private float _steeringSensitivity = 0.5f; 
    [SerializeField] private float _minGasForTurn = 0.4f;      

    private VehicleMovement _movement;
    private VehicleStats _stats;
    private WeaponFire _weaponFire;
    private WeaponController _weaponController; // ССЫЛКА НА КОНТРОЛЛЕР ПУШКИ
    private Transform _player;
    
    private float _orbitAngle;

    private void Awake()
    {
        _movement = GetComponent<VehicleMovement>();
        _stats = GetComponent<VehicleStats>();
        _weaponFire = GetComponentInChildren<WeaponFire>();
        // Находим контроллер пушки в дочерних объектах
        _weaponController = GetComponentInChildren<WeaponController>();
    }

    private void Start()
    {
        TryFindPlayer();
        
        if (_movement != null) _movement.driftFactor = 0.1f;
    }

    private void TryFindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            _player = playerObj.transform;
            // ПЕРЕДАЕМ ЦЕЛЬ ПУШКЕ
            if (_weaponController != null) _weaponController.target = _player;
        }
    }

    private void FixedUpdate()
    {
        if (_player == null) 
        {
            TryFindPlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, _player.position);
        UpdateAIState(distance);
        ExecuteState();
        HandleCombat(distance);
    }

    private void UpdateAIState(float distance)
    {
        if (distance > _attackRange + 2f) _currentState = AIState.Chasing;
        else if (distance < _safetyDistance) _currentState = AIState.Reversing;
        else _currentState = AIState.Orbiting;
    }

    private void ExecuteState()
    {
        Vector2 targetPos = _player.position;

        switch (_currentState)
        {
            case AIState.Chasing:
                DriveToPoint(_player.position, 1f);
                break;

            case AIState.Orbiting:
                _orbitAngle += Time.fixedDeltaTime * 0.5f;
                targetPos = _player.position + new Vector3(
                    Mathf.Cos(_orbitAngle) * _attackRange,
                    Mathf.Sin(_orbitAngle) * _attackRange,
                    0
                );
                DriveToPoint(targetPos, 0.8f);
                break;

            case AIState.Reversing:
                DriveToPoint(_player.position, -0.6f);
                break;
        }
    }

    private void DriveToPoint(Vector2 point, float gasMultiplier)
    {
        Vector2 relativePoint = transform.InverseTransformPoint(point);
        float angle = Mathf.Atan2(relativePoint.x, relativePoint.y) * Mathf.Rad2Deg;
        float steerInput = Mathf.Clamp(angle / _stats.SteeringSpeed, -1f, 1f);

        float gasInput = gasMultiplier;
        if (Mathf.Abs(angle) > 45f && gasMultiplier > 0)
        {
            gasInput = _minGasForTurn;
        }

        _movement.Move(gasInput, steerInput);
    }

    private void HandleCombat(float distance)
    {
        if (_weaponFire == null || _weaponFire.firePoint == null || distance > _stats.Weapon.range) return;

        // ВАЖНО: Профессиональный ИИ стреляет только тогда, когда пушка ДЕЙСТВИТЕЛЬНО наведена
        // Мы проверяем направление FirePoint.up (куда смотрит дуло) относительно игрока
        Vector2 dirToPlayer = (_player.position - _weaponFire.firePoint.position).normalized;
        float angleToPlayer = Vector2.Angle(_weaponFire.firePoint.up, dirToPlayer);

        if (angleToPlayer < 10f) // Допуск 10 градусов для выстрела
        {
            _weaponFire.Shoot();
        }
    }
}