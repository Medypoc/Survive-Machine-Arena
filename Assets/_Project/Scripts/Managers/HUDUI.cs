using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Player References")]
    [SerializeField] private VehicleStats _playerStats;
    [SerializeField] private WeaponFire _playerWeapon;
    [SerializeField] private Health _playerHealth; 
    [SerializeField] private Fuel _playerFuel;     

    [Header("Wave Manager Reference")]
    [SerializeField] private WaveManager _waveManager;

    [Header("Health & Fuel UI (Sliders or Images)")]
    public Slider healthSlider; // Если используете компонент Slider
    public Slider fuelSlider;
    // Или оставьте Image, если используете Fill Amount
    public Image healthBar; 
    public Image fuelBar;

    [Header("Ammo & Reload UI")]
    public TextMeshProUGUI ammoText;
    public GameObject reloadRoot; // ТЕПЕРЬ ССЫЛКА НА ВЕСЬ ОБЪЕКТ (Круг + Фон + Текст)
    public Image reloadRadial;

    [Header("Wave UI Elements")]
    [SerializeField] private TMP_Text _timerText; 
    [SerializeField] private string messageLabel = "Next Wave in: ";
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private string waveLabel = "Wave: ";
    [SerializeField] private TMP_Text _enemyCountText;
    [SerializeField] private string enemyLabel = "Enemies: ";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (_waveManager == null) _waveManager = FindAnyObjectByType<WaveManager>();
        
        // Пытаемся найти игрока, если он уже есть на сцене
        if (_playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) SetPlayer(player);
        }
    }

    private void Update()
    {
        // Если ссылки потерялись (например, после респавна), пробуем найти снова
        if (_playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) SetPlayer(player);
            return; 
        }

        UpdateHealthAndFuel();
        UpdateAmmoAndReload();

        if (_waveManager != null) UpdateWaveInfo();
    }

    private void UpdateHealthAndFuel()
    {
        // Проверка здоровья
        if (_playerHealth != null && _playerHealth.maxHealth > 0)
        {
            float healthRatio = _playerHealth.currentHealth / _playerHealth.maxHealth;
            if (healthSlider != null) healthSlider.value = healthRatio;
            if (healthBar != null) healthBar.fillAmount = healthRatio;
        }

        // Проверка топлива
        if (_playerFuel != null && _playerFuel.maxFuel > 0)
        {
            float fuelRatio = _playerFuel.currentFuel / _playerFuel.maxFuel;
            if (fuelSlider != null) fuelSlider.value = fuelRatio;
            if (fuelBar != null) fuelBar.fillAmount = fuelRatio;
        }
    }

    private void UpdateAmmoAndReload()
    {
        if (_playerWeapon == null || _playerStats.Weapon == null) return;

        if (ammoText != null)
            ammoText.text = $"{_playerWeapon.GetCurrentAmmo()} / {_playerStats.Weapon.magazineSize}";

        // УПРАВЛЕНИЕ ВСЕМ ОБЪЕКТОМ ПЕРЕЗАРЯДКИ
        if (_playerWeapon.IsReloading())
        {
            if (reloadRoot != null) reloadRoot.SetActive(true); // Включаем всё (фон и круг)
            if (reloadRadial != null) reloadRadial.fillAmount = _playerWeapon.ReloadProgress;
        }
        else
        {
            if (reloadRoot != null) reloadRoot.SetActive(false); // Скрываем всё полностью
        }
    }

    private void UpdateWaveInfo()
    {
        // Логика таймера[cite: 2]
        if (_timerText != null)
        {
            if (_waveManager.IsWaitingForWave && _waveManager.CurrentWave < _waveManager.TotalWaves)
            {
                _timerText.enabled = true;
                _timerText.text = $"{messageLabel}{_waveManager.TimeRemaining.ToString("0.00", CultureInfo.InvariantCulture)}";
            }
            else _timerText.enabled = false;
        }

        if (_waveText != null) _waveText.text = $"{waveLabel}{_waveManager.CurrentWave}/{_waveManager.TotalWaves}";

        if (_enemyCountText != null)
        {
            bool isWaveActive = !_waveManager.IsWaitingForWave && _waveManager.CurrentWave > 0;
            _enemyCountText.enabled = isWaveActive;
            if (isWaveActive) _enemyCountText.text = $"{enemyLabel}{_waveManager.EnemiesAlive}/{_waveManager.TotalEnemiesInCurrentWave}";
        }
    }

    public void SetPlayer(GameObject player)
    {
        _playerStats = player.GetComponent<VehicleStats>();
        _playerWeapon = player.GetComponentInChildren<WeaponFire>();
        _playerHealth = player.GetComponent<Health>(); // Ссылка на Health.cs[cite: 5]
        _playerFuel = player.GetComponent<Fuel>();     // Ссылка на Fuel.cs[cite: 4]
    }
}