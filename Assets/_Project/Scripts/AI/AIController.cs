using UnityEngine;

public enum AIState { Chasing, Orbiting, Reversing, AntiFlanking }

public class AIController : MonoBehaviour
{
    [Header("Behavior Settings")]
    [SerializeField] private AIState _currentState = AIState.Chasing;
    [SerializeField] private float _attackRange = 12f;
    [SerializeField] private float _safetyDistance = 6f;

    [Header("Combat Tactics")]
    [Tooltip("Если игрок находится под углом больше этого (в градусах), ИИ начнет защищать фланги")]
    [SerializeField] private float _flankAngleThreshold = 65f; 
    
    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _detectionRange = 6f; 
    [SerializeField] private float _avoidanceForce = 1.2f;

    [Header("Stuck Recovery")]
    [SerializeField] private float _stuckVelocityThreshold = 0.5f; 
    [SerializeField] private float _stuckWaitTime = 1.0f;          
    [SerializeField] private float _recoveryDuration = 1.5f;      
    
    private float _stuckTimer;
    private float _recoveryTimer;
    private bool _isRecovering;
    private float _recoverySteerDirection; 

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
        // ВНИМАНИЕ: Мы больше не ищем пушку в Awake, так как она спавнится динамически позже
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

        Vector2 dirToPlayer = _playerTarget.position - transform.position;
        float distance = dirToPlayer.magnitude;
        
        float angleToPlayer = Vector2.SignedAngle(transform.up, dirToPlayer);

        CheckIfStuck();

        if (_isRecovering)
        {
            ExecuteRecovery();
        }
        else
        {
            ThinkAndChooseState(distance, angleToPlayer);
            ExecuteState(distance, angleToPlayer);
        }

        // --- НОВАЯ ЛОГИКА ОРУЖИЯ ---
        UpdateWeaponTarget();
        HandleCombat(distance);
    }

    private void ThinkAndChooseState(float distance, float angleToPlayer)
    {
        if (Mathf.Abs(angleToPlayer) > _flankAngleThreshold && distance < _attackRange)
        {
            _currentState = AIState.AntiFlanking;
            return;
        }

        if (distance > _attackRange) 
            _currentState = AIState.Chasing;
        else if (distance < _safetyDistance) 
            _currentState = AIState.Reversing;
        else 
            _currentState = AIState.Orbiting;
    }

    private void ExecuteState(float distance, float angleToPlayer)
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

            case AIState.AntiFlanking:
                float steerDirection = Mathf.Sign(angleToPlayer); 
                _movement.Move(-0.8f, steerDirection, false);
                break;
        }
    }

    private void DriveWithAvoidance(Vector2 targetPoint, float gasMultiplier)
    {
        float avoidanceSteer = GetAvoidanceSteer();

        if (Mathf.Abs(avoidanceSteer) > 0.1f)
        {
            _movement.Move(gasMultiplier * 0.4f, avoidanceSteer * _avoidanceForce, false);
        }
        else
        {
            DriveToPoint(targetPoint, gasMultiplier);
        }
    }

    private float GetAvoidanceSteer()
    {
        float steer = 0f;
        float dynamicRange = _detectionRange + (_rb.linearVelocity.magnitude * 0.2f);

        RaycastHit2D centerHit = Physics2D.Raycast(transform.position, transform.up, dynamicRange, _obstacleMask);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 35) * transform.up, dynamicRange * 0.8f, _obstacleMask);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -35) * transform.up, dynamicRange * 0.8f, _obstacleMask);

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
        if (Mathf.Abs(angle) > 30f && gasMultiplier > 0) gasInput = 0.5f; 

        _movement.Move(gasInput, steerInput, false);
    }

    private void CheckIfStuck()
    {
        if (_isRecovering) return;

        if (_rb.linearVelocity.magnitude < _stuckVelocityThreshold)
        {
            _stuckTimer += Time.fixedDeltaTime;
            if (_stuckTimer >= _stuckWaitTime) StartRecovery();
        }
        else _stuckTimer = 0f;
    }

    private void StartRecovery()
    {
        _isRecovering = true;
        _recoveryTimer = _recoveryDuration;
        _stuckTimer = 0f;

        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 30) * transform.up, _detectionRange, _obstacleMask);
        _recoverySteerDirection = (leftHit.collider != null) ? -1f : 1f;
    }

    private void ExecuteRecovery()
    {
        _recoveryTimer -= Time.fixedDeltaTime;
        _movement.Move(-0.8f, _recoverySteerDirection, false);
        if (_recoveryTimer <= 0) _isRecovering = false;
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy)
        {
            _playerTarget = playerObj.transform;
            // УДАЛЕНО: прямое присвоение target контроллеру пушки
        }
    }

    // --- НОВЫЙ МЕТОД: Постоянно передает координаты Игрока в пушку ---
    private void UpdateWeaponTarget()
    {
        if (_playerTarget == null) return;

        // Динамически ищем контроллер, если он еще не найден
        if (_weaponController == null)
        {
            _weaponController = GetComponentInChildren<WeaponController>();
        }

        if (_weaponController != null)
        {
            _weaponController.SetTargetPoint(_playerTarget.position);
        }
    }

    private void HandleCombat(float distance)
    {
        // Динамически ищем скрипт стрельбы
        if (_weaponFire == null)
        {
            _weaponFire = GetComponentInChildren<WeaponFire>();
            if (_weaponFire == null) return; 
        }

        if (_stats == null || _stats.Weapon == null || _weaponFire.firePoint == null || distance > _stats.Weapon.shootingStats.range) return;

        Vector2 dirToTarget = (_playerTarget.position - _weaponFire.firePoint.position).normalized;
        float angleToTarget = Vector2.Angle(_weaponFire.firePoint.up, dirToTarget);

        if (angleToTarget < 10f) _weaponFire.Shoot();
    }
}