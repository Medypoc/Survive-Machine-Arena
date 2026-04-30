using UnityEngine;
using SurviveArena.Core; // Подключаем пространство имен для VehicleClass

public class EnemySpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject enemyPrefab;    
    public VehicleRegistry_SO registry; // ИСПРАВЛЕНО: добавлено подчеркивание _SO
    public VehicleClass spawnClass;     // ИСПРАВЛЕНО: заменено VehicleStats.VehicleBase на VehicleClass

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
            Debug.LogError("[EnemySpawner] Не назначен префаб или Реестр!");
            return;
        }

        // 1. Получаем случайные детали из реестра
        registry.GetRandomParts(spawnClass, out var body, out var cab, out var weapon);

        // 2. Рассчитываем позицию появления за пределами экрана
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRadius;

        // 3. Создаем врага
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 4. Инициализируем его через новый метод в VehicleStats
        VehicleStats enemyStats = enemyObj.GetComponent<VehicleStats>();
        if (enemyStats != null)
        {
            // Передаем модули. Метод LoadModules сам вызовет RefreshStats
            enemyStats.LoadModules(body, cab, weapon);
        }
        else
        {
            Debug.LogWarning($"[EnemySpawner] На префабе {enemyPrefab.name} нет скрипта VehicleStats!");
        }
    }
}