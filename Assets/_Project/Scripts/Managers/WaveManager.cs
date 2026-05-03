using UnityEngine;
using System.Collections.Generic;
using SurviveArena.Core;
using SurviveArena.Data;

public class WaveManager : MonoBehaviour
{
    [Header("Spawn Locations")]
    public List<EnemySpawnZone> spawnZones;

    [Header("Enemy Setup")]
    public GameObject enemyPrefab;
    public VehicleRegistry_SO registry;
    public VehicleClass spawnClass;
    public EnemyModifierSO globalModifier;

    [Header("Wave Rules")]
    public int totalWaves = 3;
    public int enemiesPerWave = 3;
    public int nextWaveThreshold = 0;
    public float waveDelay = 10f;

    [Header("State")]
    [SerializeField] private int currentWave = 0;
    private bool isWaitingForNextWave = false;
    private float nextWaveTimer;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // --- ПУБЛИЧНЫЕ СВОЙСТВА ДЛЯ UI ---
    public bool IsWaitingForWave => isWaitingForNextWave;
    public float TimeRemaining => Mathf.Max(0, nextWaveTimer - Time.time);
    public int CurrentWave => currentWave;
    public int TotalWaves => totalWaves;

    // Новые свойства для исправления ошибок в WaveUI
    public int TotalEnemiesInCurrentWave => enemiesPerWave; 
    public int EnemiesAlive => activeEnemies.Count;
    // ---------------------------------

    void Start()
    {
        isWaitingForNextWave = true;
        nextWaveTimer = Time.time + waveDelay;
    }

    void Update()
    {
        // Очистка списка от уничтоженных объектов (важно для точности EnemiesAlive)
        activeEnemies.RemoveAll(item => item == null);

        // Проверка условия победы
        if (currentWave == totalWaves && activeEnemies.Count == 0 && !isWaitingForNextWave)
        {
            BattleManager.Instance?.OnVictory();
            this.enabled = false;
            return;
        }

        // Логика перехода между волнами
        if (isWaitingForNextWave)
        {
            if (Time.time >= nextWaveTimer && currentWave < totalWaves)
            {
                SpawnWave();
            }
        }
        else if (activeEnemies.Count <= nextWaveThreshold && currentWave < totalWaves)
        {
            isWaitingForNextWave = true;
            nextWaveTimer = Time.time + waveDelay;
        }
    }

    void SpawnWave()
    {
        currentWave++;
        isWaitingForNextWave = false;

        if (spawnZones == null || spawnZones.Count == 0) return;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            EnemySpawnZone selectedZone = spawnZones[Random.Range(0, spawnZones.Count)];
            Vector2 spawnPos = selectedZone.GetRandomPointInZone();
            SpawnSingleEnemy(spawnPos);
        }
    }

    void SpawnSingleEnemy(Vector2 position)
    {
        if (registry == null || enemyPrefab == null) return;

        registry.GetRandomParts(spawnClass, out var body, out var cab, out var weapon);
        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
        activeEnemies.Add(enemyObj);

        VehicleStats stats = enemyObj.GetComponent<VehicleStats>();
        if (stats != null)
        {
            stats.LoadModules(body, cab, weapon);
            if (globalModifier != null) 
                stats.ApplyModifiers(globalModifier.healthMultiplier, globalModifier.speedMultiplier, globalModifier.damageMultiplier);
        }

        // Инициализация наград при смерти
        enemyObj.GetComponent<EnemyReward>()?.InitializeRewards(50, body, cab, weapon, globalModifier);
    }
}