using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
// Раскомментируй строку ниже и впиши свой namespace, если PlayerDataSO лежит в нем
// using SurviveArena.Data; 

public class BattleManager : MonoBehaviour
{
    // Синглтон для удобного доступа из других скриптов
    public static BattleManager Instance;

    [Header("Scene Settings")]
    [SerializeField] private string _hubSceneName = "HUB_Scene"; // Имя сцены Хаба
    [SerializeField] private float _delayBeforeReturn = 3.0f;    // Задержка перед загрузкой

    [Header("Match Economy")]
    [SerializeField] private PlayerDataSO _playerProfile; // Ссылка на твой файл сохранения
    private int _totalMatchXP = 0;
    private int _totalMatchMoney = 0;

    [Header("Time Constraints (in seconds)")]
    [SerializeField] private float _timeForSRank = 60f;
    [SerializeField] private float _timeForARank = 120f;
    [SerializeField] private float _timeForBRank = 180f;
    [SerializeField] private float _timeForCRank = 240f;

    private float _matchStartTime;

    private void Awake()
    {
        // Базовая инициализация синглтона
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Запускаем секундомер в начале боя
        _matchStartTime = Time.time;
    }

    // Вызывается скриптом EnemyReward при смерти КАЖДОГО врага
    public void AddMatchRewards(int xp, int money)
    {
        _totalMatchXP += xp;
        _totalMatchMoney += money;
    }

    // Вызывается из Health.cs при смерти ИГРОКА
    public void OnPlayerDeath()
    {
        Debug.Log("Поражение! Возврат в Хаб...");
        // При поражении мы не выдаем награды (или можно выдать, скажем, 10% от собранного)
        StartCoroutine(ReturnToHubRoutine());
    }

    // Вызывается из EnemySpawner.cs после зачистки ПОСЛЕДНЕЙ волны
    public void OnVictory()
    {
        // 1. Считаем время матча
        float matchDuration = Time.time - _matchStartTime;
        
        // 2. Определяем ранг и множитель
        float rankMultiplier = CalculateRankMultiplier(matchDuration, out string finalRank);

        // 3. Применяем множитель к собранному луту
        int finalXP = Mathf.RoundToInt(_totalMatchXP * rankMultiplier);
        int finalMoney = Mathf.RoundToInt(_totalMatchMoney * rankMultiplier);

        // 4. Сохраняем награды в профиль
        if (_playerProfile != null)
        {
            _playerProfile.money += finalMoney;
            _playerProfile.AddExperience(finalXP);
        }

        Debug.Log($"Победа! Все волны зачищены. Время: {matchDuration:F1}с | Ранг: {finalRank} | ХП: {finalXP} | Деньги: {finalMoney}");

        // 5. Запускаем выход
        StartCoroutine(ReturnToHubRoutine());
    }

    // Вспомогательный метод для определения ранга
    private float CalculateRankMultiplier(float duration, out string rank)
    {
        if (duration <= _timeForSRank) { rank = "S"; return 1.30f; }
        if (duration <= _timeForARank) { rank = "A"; return 1.20f; }
        if (duration <= _timeForBRank) { rank = "B"; return 1.10f; }
        if (duration <= _timeForCRank) { rank = "C"; return 1.00f; }
        
        // Если игрок проходил арену дольше С-ранга
        rank = "D"; return 1.00f; 
    }

    // Корутина для плавной задержки перед переключением сцены
    private IEnumerator ReturnToHubRoutine()
    {
        yield return new WaitForSeconds(_delayBeforeReturn);
        SceneManager.LoadScene(_hubSceneName);
    }
}