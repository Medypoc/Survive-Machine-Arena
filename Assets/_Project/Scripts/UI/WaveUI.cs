using UnityEngine;
using TMPro;
using System.Globalization;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    // Заменяем EnemySpawner на WaveManager
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private TMP_Text _timerText; 

    [Header("Settings")]
    [Tooltip("Используйте {0} для вставки времени")]
    public string messageLabel = "Next Wave in: ";
    public string finalWaveMessage = "Final Wave Starting!";

    private void Start()
    {
        // Ищем новый менеджер волн, если он не задан в инспекторе
        if (_waveManager == null)
            _waveManager = FindAnyObjectByType<WaveManager>();
            
        if (_timerText == null)
            _timerText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_waveManager == null || _timerText == null) return;

        // Используем свойства из WaveManager для отображения таймера
        if (_waveManager.IsWaitingForWave && _waveManager.CurrentWave < _waveManager.TotalWaves)
        {
            _timerText.enabled = true; 

            // Проверка на финальную волну[cite: 7]
            if (_waveManager.CurrentWave + 1 == _waveManager.TotalWaves && _waveManager.TimeRemaining < 2f)
            {
                _timerText.text = finalWaveMessage;
            }
            else
            {
                // Форматируем оставшееся время до следующей волны[cite: 7]
                string timeFormatted = _waveManager.TimeRemaining.ToString("0.00", CultureInfo.InvariantCulture);
                _timerText.text = messageLabel + timeFormatted;
            }
        }
        else
        {
            // Скрываем текст, когда волна уже идет или все волны пройдены[cite: 7]
            _timerText.enabled = false; 
        }
    }
}