using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform[] spawnPoints; // Точки, которые вы расставите на арене
    public float spawnInterval = 15f;
    public int maxItemsOnMap = 3;

    private List<GameObject> _activeItems = new List<GameObject>();
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            _timer = 0;
            TrySpawnItem();
        }
    }

    private void TrySpawnItem()
    {
        // Очищаем список от уже подобранных предметов
        _activeItems.RemoveAll(item => item == null);

        if (_activeItems.Count < maxItemsOnMap)
        {
            // Выбираем случайную точку из списка
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Можно добавить проверку, нет ли уже предмета в этой точке
            GameObject newItem = Instantiate(itemPrefab, point.position, Quaternion.identity);
            _activeItems.Add(newItem);
        }
    }
}