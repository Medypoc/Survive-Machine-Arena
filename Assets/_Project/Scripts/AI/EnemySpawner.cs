using UnityEngine;
using System.Collections.Generic;
using SurviveArena.Core;
using SurviveArena.Data;

public class EnemySpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject enemyPrefab;
    public VehicleRegistry_SO registry;
    public VehicleClass spawnClass;
    public EnemyModifierSO globalModifier;

    [Header("Wave Settings")]
    [Tooltip("Общее количество волн")]
    public int totalWaves = 3;
    
    [Tooltip("Количество врагов в одной волне")]
    public int enemiesPerWave = 3;
    
    [Tooltip("Радиус спавна врагов от центра")]
    public float spawnRadius = 40f;
    
    [Tooltip("Сколько врагов должно остаться, чтобы триггернуть новую волну")]
    public int nextWaveThreshold = 0;
    
    [Tooltip("Задержка перед появлением волны (в секундах)")]
    public float waveDelay = 10f;

    [Header("Current State (Read Only)")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private bool isWaitingForNextWave = false;
    
    private float nextWaveTimer;
    
    // Список для отслеживания живых противников
    private List<GameObject> activeEnemies = new List<GameObject>();

    public bool IsWaitingForWave => isWaitingForNextWave;
    public float TimeRemaining => Mathf.Max(0, nextWaveTimer - Time.time);
    public int CurrentWave => currentWave;
    public int TotalWaves => totalWaves;

    void Start()
    {
        // Инициализируем задержку перед самой первой волной
        isWaitingForNextWave = true;
        nextWaveTimer = Time.time + waveDelay;
    }

    void Update()
    {
        // 1. Очищаем список от мертвых врагов 
        activeEnemies.RemoveAll(item => item == null);

        // --- НОВЫЙ БЛОК: ПРОВЕРКА ПОБЕДЫ ---
        // Если все волны вышли, врагов не осталось и мы не ждем новую волну
        if (currentWave == totalWaves && activeEnemies.Count == 0 && !isWaitingForNextWave)
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnVictory();
                this.enabled = false; // Выключаем скрипт, чтобы не вызывать победу многократно
            }
            return; // Выходим из Update, так как бой закончен
        }
        // ----------------------------------

        // 2. Логика ожидания (таймер)[cite: 3]
        if (isWaitingForNextWave)
        {
            if (Time.time >= nextWaveTimer && currentWave < totalWaves)
            {
                SpawnWave();
            }
        }
        // 3. Логика боя (ждем, пока умрут враги)[cite: 3]
        else
        {
            // Если живых врагов осталось меньше или равно порогу, и волны еще есть[cite: 3]
            if (activeEnemies.Count <= nextWaveThreshold && currentWave < totalWaves)
            {
                isWaitingForNextWave = true;
                nextWaveTimer = Time.time + waveDelay;
            }
        }
    }

    void SpawnWave()
    {
        currentWave++;
        isWaitingForNextWave = false;

        if (enemiesPerWave <= 0) return;

        // Распределяем врагов равномерно по кругу, как мы делали раньше
        float angleStep = 360f / enemiesPerWave;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 spawnDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 spawnPos = spawnDirection * spawnRadius;

            SpawnEnemy(spawnPos);
        }
    }

    void SpawnEnemy(Vector2 localOffset)
    {
        if (registry == null || enemyPrefab == null) return;

        registry.GetRandomParts(spawnClass, out var body, out var cab, out var weapon);

        // Расчет позиции и поворота носом в центр арены (0,0)
        Vector2 finalSpawnPos = (Vector2)transform.position + localOffset;
        Vector2 directionToCenter = Vector2.zero - finalSpawnPos;

        float lookAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg - 90f;
        Quaternion spawnRotation = Quaternion.Euler(0, 0, lookAngle);

        // Создаем машину
        GameObject enemyObj = Instantiate(enemyPrefab, finalSpawnPos, spawnRotation);
        
        // ДОБАВЛЕНО: Записываем машину в журнал учета живых
        activeEnemies.Add(enemyObj);

        // Применяем статы и модули[cite: 2]
        VehicleStats enemyStats = enemyObj.GetComponent<VehicleStats>();
        if (enemyStats != null)
        {
            enemyStats.LoadModules(body, cab, weapon);

            if (globalModifier != null)
            {
                enemyStats.ApplyModifiers(
                    globalModifier.healthMultiplier, 
                    globalModifier.speedMultiplier, 
                    globalModifier.damageMultiplier
                );

                VehicleVisual visual = enemyObj.GetComponent<VehicleVisual>();
                if (visual != null) visual.ApplyTint(globalModifier.visualTint);
            }
        }
        EnemyReward rewardSys = enemyObj.GetComponent<EnemyReward>();
        if (rewardSys != null)
        {
            // classBaseXP можешь передавать как константу, например 50 за этот класс машины
            rewardSys.InitializeRewards(50, body, cab, weapon, globalModifier);
        }
    }

}