using UnityEngine;

public enum AIState { Chasing, Orbiting, Reversing }

public class AIController : MonoBehaviour
{
    [Header("Behavior Settings")]
    [SerializeField] private AIState _currentState = AIState.Chasing;
    [SerializeField] private float _attackRange = 12f;
    [SerializeField] private float _safetyDistance = 6f;

    [Header("Movement Tuning")]
    //[SerializeField] private float _steeringSensitivity = 0.5f; 
    [SerializeField] private float _minGasForTurn = 0.4f;      

    private VehicleMovement _movement;
    private VehicleStats _stats;
    private WeaponFire _weaponFire;
    private WeaponController _weaponController; 
    
    // Теперь цель всегда одна - игрок
    private Transform _playerTarget; 
    private float _orbitAngle;

    private void Awake()
    {
        _movement = GetComponent<VehicleMovement>();
        _stats = GetComponent<VehicleStats>();
        _weaponFire = GetComponentInChildren<WeaponFire>();
        _weaponController = GetComponentInChildren<WeaponController>();
    }

    private void Start()
    {
        if (_movement != null) _movement.defaultDriftFactor = 0.1f;
        FindPlayer(); 
    }

    private void FixedUpdate()
    {
        // Если игрока нет на сцене или он уничтожен - просто стоим
        if (_playerTarget == null || !_playerTarget.gameObject.activeInHierarchy) 
        {
            FindPlayer();
            return; 
        }

        float distance = Vector2.Distance(transform.position, _playerTarget.position);
        UpdateAIState(distance);
        ExecuteState();
        HandleCombat(distance);
    }

    // Поиск игрока по тегу
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy)
        {
            _playerTarget = playerObj.transform;
            if (_weaponController != null) 
            {
                _weaponController.target = _playerTarget;
            }
        }
    }

    private void UpdateAIState(float distance)
    {
        if (distance > _attackRange + 2f) _currentState = AIState.Chasing;
        else if (distance < _safetyDistance) _currentState = AIState.Reversing;
        else _currentState = AIState.Orbiting;
    }

    private void ExecuteState()
    {
        Vector2 targetPos = _playerTarget.position;

        switch (_currentState)
        {
            case AIState.Chasing:
                DriveToPoint(_playerTarget.position, 1f);
                break;

            case AIState.Orbiting:
                _orbitAngle += Time.fixedDeltaTime * 0.5f;
                targetPos = _playerTarget.position + new Vector3(
                    Mathf.Cos(_orbitAngle) * _attackRange,
                    Mathf.Sin(_orbitAngle) * _attackRange,
                    0
                );
                DriveToPoint(targetPos, 0.8f);
                break;

            case AIState.Reversing:
                DriveToPoint(_playerTarget.position, -0.6f);
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

        _movement.Move(gasInput, steerInput, false);
    }

    private void HandleCombat(float distance)
    {
        if (_weaponFire == null || _weaponFire.firePoint == null || _stats.Weapon == null || distance > _stats.Weapon.range) return;

        // ИИ всегда пытается стрелять только в направлении героя
        Vector2 dirToTarget = (_playerTarget.position - _weaponFire.firePoint.position).normalized;
        float angleToTarget = Vector2.Angle(_weaponFire.firePoint.up, dirToTarget);

        if (angleToTarget < 10f) 
        {
            _weaponFire.Shoot();
        }
    }
}