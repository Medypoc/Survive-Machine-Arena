using UnityEngine;
using TMPro;
using System.Globalization; // Добавляем для работы с точкой

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner _spawner;
    [SerializeField] private TMP_Text _timerText; 

    [Header("Settings")]
    [Tooltip("Используйте {0} для вставки времени")]
    public string messageLabel = "Next Wave in: ";
    public string finalWaveMessage = "Final Wave Starting!";

    private void Start()
    {
        if (_spawner == null)
            _spawner = FindAnyObjectByType<EnemySpawner>();
            
        if (_timerText == null)
            _timerText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_spawner == null || _timerText == null) return;

        // Используем IsWaitingForWave из нашего спавнера
        if (_spawner.IsWaitingForWave && _spawner.CurrentWave < _spawner.TotalWaves)
        {
            _timerText.enabled = true; 

            // Если до старта меньше 2 секунд и это последняя волна
            if (_spawner.CurrentWave + 1 == _spawner.TotalWaves && _spawner.TimeRemaining < 2f)
            {
                _timerText.text = finalWaveMessage;
            }
            else
            {
                // Форматируем число: 
                // "0.00" — два знака после точки (миллисекунды)
                // CultureInfo.InvariantCulture — гарантирует точку вместо запятой
                string timeFormatted = _spawner.TimeRemaining.ToString("0.00", CultureInfo.InvariantCulture);
                
                _timerText.text = messageLabel + timeFormatted;
            }
        }
        else
        {
            _timerText.enabled = false; 
        }
    }
}