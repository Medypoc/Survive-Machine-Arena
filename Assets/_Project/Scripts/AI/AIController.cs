using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private float _targetRefreshInterval = 0.5f;

    [Header("Orbit Settings")]
    [SerializeField] private float _orbitRadius = 7f;
    [SerializeField] private float _orbitThreshold = 2f;
    [SerializeField] private float _inwardPull = 0.3f; // Немного увеличил, чтобы лучше держал круг

    [Header("Anti-Drift Settings")]
    [SerializeField, Range(0, 1)] private float _aiDriftFactor = 0.1f; // Высокое сцепление для ИИ
    [SerializeField] private float _brakingAngleThreshold = 60f; // Угол, при котором ИИ бьет по тормозам

    [Header("Movement Settings")]
    [SerializeField] private float _steeringAngleForMaxInput = 45f;
    [SerializeField] private float _deadzoneAngle = 2f; // Угол, при котором руль стоит прямо

    [Header("Combat Settings")]
    [SerializeField] private float _shootAngleMargin = 15f;
    [SerializeField] private Vector2 _switchDirectionIntervalRange = new Vector2(5f, 10f);

    private VehicleMovement _movement;
    private VehicleStats _stats;
    private WeaponFire _weaponFire;
    private WeaponController _weaponController;
    private Transform _player;

    private int _orbitDirection;
    private float _nextDirectionSwitchTime;
    private float _nextTargetRefreshTime;

    private void Awake()
    {
        _movement = GetComponent<VehicleMovement>();
        _stats = GetComponent<VehicleStats>();
        _weaponFire = GetComponentInChildren<WeaponFire>();
        _weaponController = GetComponentInChildren<WeaponController>();
    }

    private void Start()
    {
        _orbitDirection = Random.value > 0.5f ? 1 : -1;
        ScheduleNextDirectionSwitch();
        TryFindPlayer();
        SyncWeaponTarget();

        // Устанавливаем ИИ "хорошие шины", чтобы не заносило
        if (_movement != null)
        {
            _movement.driftFactor = _aiDriftFactor;
        }
    }

    private void FixedUpdate()
    {
        RefreshTargetIfNeeded();
        if (_player == null || _movement == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        HandleDirectionSwitch();
        HandleMovement(distanceToPlayer);
        HandleCombat(distanceToPlayer);
    }

    // --- ЛОГИКА ДВИЖЕНИЯ (ИСПРАВЛЕНА) ---
    private void HandleMovement(float distanceToPlayer)
    {
        Vector2 toPlayer = (_player.position - transform.position).normalized;
        Vector2 desiredDirection = GetDesiredMovementDirection(toPlayer, distanceToPlayer);

        float angleToTarget = Vector2.SignedAngle(transform.up, desiredDirection);
        float absAngle = Mathf.Abs(angleToTarget);

        // 1. Руление (оставляем как было, это работает хорошо)
        float steerInput = 0;
        if (absAngle > _deadzoneAngle)
        {
            steerInput = Mathf.Clamp(angleToTarget / _steeringAngleForMaxInput, -1f, 1f);
        }

        // 2. ГИБКИЙ ГАЗ (ИСПРАВЛЕНО)
        float gasInput = 1f;

        if (absAngle > 90f) 
        {
            // Только если цель ПОЗАДИ нас, мы бьем по тормозам или сдаем назад
            gasInput = -0.3f; 
        }
        else if (absAngle > 30f)
        {
            // В обычном повороте даем 60-70% мощности вместо 20%. 
            // Этого хватит, чтобы преодолеть трение, но не уйти в неуправляемый занос.
            gasInput = 0.7f; 
        }
        else 
        {
            // На прямой или плавном дуге - полный газ
            gasInput = 1f;
        }

        // Если мы слишком близко к игроку (ближе, чем порог орбиты), 
        // можем еще немного снизить газ, чтобы не "таранить"
        if (distanceToPlayer < _orbitRadius - _orbitThreshold)
        {
            gasInput *= 0.5f;
        }

        _movement.Move(gasInput, steerInput);
    }

    // --- ОСТАЛЬНАЯ ЛОГИКА (БЕЗ ИЗМЕНЕНИЙ) ---

    private Vector2 GetDesiredMovementDirection(Vector2 toPlayer, float distanceToPlayer)
    {
        if (distanceToPlayer > _orbitRadius + _orbitThreshold) return toPlayer;
        if (distanceToPlayer < _orbitRadius - _orbitThreshold) return -toPlayer;

        Vector2 orbitTangent = new Vector2(-toPlayer.y, toPlayer.x) * _orbitDirection;
        return (orbitTangent + toPlayer * _inwardPull).normalized;
    }

    private void HandleCombat(float distanceToPlayer)
    {
        if (_weaponFire == null || _weaponFire.firePoint == null || _stats == null || _stats.Weapon == null) return;
        if (distanceToPlayer > _stats.Weapon.range) return;

        Vector2 directionToPlayer = (_player.position - _weaponFire.firePoint.position).normalized;
        float angleToTarget = Vector2.Angle(_weaponFire.firePoint.up, directionToPlayer);
        
        if (angleToTarget <= _shootAngleMargin)
        {
            _weaponFire.Shoot();
        }
    }

    private void RefreshTargetIfNeeded()
    {
        if (_player != null || Time.time < _nextTargetRefreshTime) return;
        TryFindPlayer();
        SyncWeaponTarget();
        _nextTargetRefreshTime = Time.time + _targetRefreshInterval;
    }

    private void TryFindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(_playerTag);
        _player = playerObject != null ? playerObject.transform : null;
    }

    private void SyncWeaponTarget()
    {
        if (_weaponController != null) _weaponController.target = _player;
    }

    private void HandleDirectionSwitch()
    {
        if (Time.time <= _nextDirectionSwitchTime) return;
        _orbitDirection *= -1;
        ScheduleNextDirectionSwitch();
    }

    private void ScheduleNextDirectionSwitch()
    {
        float min = Mathf.Max(0.1f, _switchDirectionIntervalRange.x);
        float max = Mathf.Max(min, _switchDirectionIntervalRange.y);
        _nextDirectionSwitchTime = Time.time + Random.Range(min, max);
    }
}