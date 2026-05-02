using UnityEngine;

public enum AIState { Chasing, Orbiting, Reversing }

public class AIController : MonoBehaviour
{
    [Header("Behavior Settings")]
    [SerializeField] private AIState _currentState = AIState.Chasing;
    [SerializeField] private float _attackRange = 12f;
    [SerializeField] private float _safetyDistance = 6f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _detectionRange = 6f; 
    [SerializeField] private float _avoidanceForce = 1.2f;

    [Header("Stuck Recovery (Система эвакуации)")]
    [SerializeField] private float _stuckVelocityThreshold = 0.5f; // Порог скорости, ниже которой считаем, что стоим
    [SerializeField] private float _stuckWaitTime = 1.0f;          // Сколько ждать до начала маневра
    [SerializeField] private float _recoveryDuration = 1.5f;      // Сколько времени сдавать назад
    
    private float _stuckTimer;
    private float _recoveryTimer;
    private bool _isRecovering;
    private float _recoverySteerDirection; // В какую сторону крутить руль при отъезде

    [Header("Movement Tuning")]
    [SerializeField] private float _minGasForTurn = 0.4f;      

    private VehicleMovement _movement;
    private Rigidbody2D _rb;
    private VehicleStats _stats;
    private WeaponFire _weaponFire;
    private WeaponController _weaponController; 
    
    private Transform _playerTarget; 
    private float _orbitAngle;

    private void Awake()
    {
        _movement = GetComponent<VehicleMovement>();
        _rb = GetComponent<Rigidbody2D>();
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
        if (_playerTarget == null || !_playerTarget.gameObject.activeInHierarchy) 
        {
            FindPlayer();
            return; 
        }

        float distance = Vector2.Distance(transform.position, _playerTarget.position);
        
        // 1. Проверяем, не застряли ли мы
        CheckIfStuck();

        // 2. Если мы в процессе эвакуации — выполняем маневр отъезда
        if (_isRecovering)
        {
            ExecuteRecovery();
        }
        else
        {
            // 3. Обычная логика состояний
            UpdateAIState(distance);
            ExecuteState();
        }

        HandleCombat(distance);
    }

    private void CheckIfStuck()
    {
        if (_isRecovering) return;

        // Если ИИ пытается ехать (gas != 0), но скорость машины слишком мала
        if (_rb.linearVelocity.magnitude < _stuckVelocityThreshold)
        {
            _stuckTimer += Time.fixedDeltaTime;
            if (_stuckTimer >= _stuckWaitTime)
            {
                StartRecovery();
            }
        }
        else
        {
            _stuckTimer = 0f;
        }
    }

    private void StartRecovery()
    {
        _isRecovering = true;
        _recoveryTimer = _recoveryDuration;
        _stuckTimer = 0f;

        // Определяем, в какую сторону крутить руль, чтобы выехать
        // Пускаем лучи: если препятствие справа — рулим влево при отъезде назад
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 30) * transform.up, _detectionRange, _obstacleMask);
        _recoverySteerDirection = (leftHit.collider != null) ? -1f : 1f;
    }

    private void ExecuteRecovery()
    {
        _recoveryTimer -= Time.fixedDeltaTime;

        // Сдаем назад и крутим руль
        _movement.Move(-0.7f, _recoverySteerDirection, false);

        if (_recoveryTimer <= 0)
        {
            _isRecovering = false;
        }
    }

    // --- Дальнейшая логика остается почти без изменений, но используем DriveWithAvoidance ---

    private void ExecuteState()
    {
        switch (_currentState)
        {
            case AIState.Chasing:
                DriveWithAvoidance(_playerTarget.position, 1f);
                break;
            case AIState.Orbiting:
                _orbitAngle += Time.fixedDeltaTime * 0.5f;
                Vector2 orbitPos = (Vector2)_playerTarget.position + new Vector2(
                    Mathf.Cos(_orbitAngle) * _attackRange,
                    Mathf.Sin(_orbitAngle) * _attackRange
                );
                DriveWithAvoidance(orbitPos, 0.8f);
                break;
            case AIState.Reversing:
                DriveWithAvoidance(_playerTarget.position, -0.6f);
                break;
        }
    }

    private void DriveWithAvoidance(Vector2 targetPoint, float gasMultiplier)
    {
        float avoidanceSteer = GetAvoidanceSteer();

        if (Mathf.Abs(avoidanceSteer) > 0.1f)
        {
            _movement.Move(gasMultiplier * 0.7f, avoidanceSteer * _avoidanceForce, false);
        }
        else
        {
            DriveToPoint(targetPoint, gasMultiplier);
        }
    }

    private float GetAvoidanceSteer()
    {
        float steer = 0f;
        RaycastHit2D centerHit = Physics2D.Raycast(transform.position, transform.up, _detectionRange, _obstacleMask);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 35) * transform.up, _detectionRange, _obstacleMask);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -35) * transform.up, _detectionRange, _obstacleMask);

        if (leftHit.collider != null) steer -= 1f;
        if (rightHit.collider != null) steer += 1f;
        if (centerHit.collider != null && steer == 0) steer = 1f;

        return steer;
    }

    private void DriveToPoint(Vector2 point, float gasMultiplier)
    {
        Vector2 relativePoint = transform.InverseTransformPoint(point);
        float angle = Mathf.Atan2(relativePoint.x, relativePoint.y) * Mathf.Rad2Deg;
        float steerInput = Mathf.Clamp(angle / (_stats != null ? _stats.SteeringSpeed : 45f), -1f, 1f);

        float gasInput = gasMultiplier;
        if (Mathf.Abs(angle) > 45f && gasMultiplier > 0) gasInput = _minGasForTurn;

        _movement.Move(gasInput, steerInput, false);
    }

    private void UpdateAIState(float distance)
    {
        if (distance > _attackRange + 2f) _currentState = AIState.Chasing;
        else if (distance < _safetyDistance) _currentState = AIState.Reversing;
        else _currentState = AIState.Orbiting;
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy)
        {
            _playerTarget = playerObj.transform;
            if (_weaponController != null) _weaponController.target = _playerTarget;
        }
    }

    private void HandleCombat(float distance)
    {
        if (_weaponFire == null || _weaponFire.firePoint == null || _stats.Weapon == null || distance > _stats.Weapon.range) return;

        Vector2 dirToTarget = (_playerTarget.position - _weaponFire.firePoint.position).normalized;
        float angleToTarget = Vector2.Angle(_weaponFire.firePoint.up, dirToTarget);

        if (angleToTarget < 10f) _weaponFire.Shoot();
    }
}