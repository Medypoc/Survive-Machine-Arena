using UnityEngine;
using SurviveArena.Core;
using SurviveArena.Data;

public class EnemySpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject enemyPrefab;
    public VehicleRegistry_SO registry;
    public VehicleClass spawnClass;

    [Header("Global Modifier")]
    // Назначь здесь EnemyModifierSO, чтобы усилить всех врагов из этого спавнера
    public EnemyModifierSO globalModifier;

    [Header("Settings")]
    public float spawnInterval = 5f;
    public float spawnRadius = 20f;
    
    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (registry == null || enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] Missing Registry or Prefab!");
            return;
        }

        // Получаем случайный набор частей
        registry.GetRandomParts(spawnClass, out var body, out var cab, out var weapon);

        // Определяем точку появления
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRadius;

        // Создаем объект врага
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        VehicleStats enemyStats = enemyObj.GetComponent<VehicleStats>();
        if (enemyStats != null)
        {
            // Сначала загружаем базовые части
            enemyStats.LoadModules(body, cab, weapon);

            // Если на спавнере есть модификатор — применяем его
            if (globalModifier != null)
            {
                enemyStats.ApplyModifiers(
                    globalModifier.healthMultiplier, 
                    globalModifier.speedMultiplier, 
                    globalModifier.damageMultiplier
                );

                // Покраска врага в цвет модификатора
                VehicleVisual visual = enemyObj.GetComponent<VehicleVisual>();
                if (visual != null)
                {
                    visual.ApplyTint(globalModifier.visualTint);
                }
            }
        }
    }
}