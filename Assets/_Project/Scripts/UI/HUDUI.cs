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

    [Header("Health & Fuel UI")]
    public Slider healthSlider; 
    public Slider fuelSlider;
    public Image healthBar; 
    public Image fuelBar;

    [Header("Ammo & Reload UI")]
    public TextMeshProUGUI ammoText;
    public GameObject reloadRoot; 
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
        Instance = this;
    }

    private void Update()
    {
        UpdateHealthAndFuel();
        UpdateAmmoAndReload();
        if (_waveManager != null) UpdateWaveInfo();
    }

    private void UpdateHealthAndFuel()
    {
        if (_playerHealth != null)
        {
            float healthPercent = _playerHealth.maxHealth > 0 ? _playerHealth.currentHealth / _playerHealth.maxHealth : 0f;
            if (healthSlider != null) healthSlider.value = healthPercent;
            if (healthBar != null) healthBar.fillAmount = healthPercent;
        }
        
        if (_playerFuel != null)
        {
            float fuelPercent = _playerFuel.maxFuel > 0 ? _playerFuel.currentFuel / _playerFuel.maxFuel : 0f;
            if (fuelSlider != null) fuelSlider.value = fuelPercent;
            if (fuelBar != null) fuelBar.fillAmount = fuelPercent;
        }
    }

    private void UpdateAmmoAndReload()
    {
        // --- АДАПТАЦИЯ К НОВОЙ АРХИТЕКТУРЕ ---
        // Если пушка не найдена (еще не заспавнилась сборщиком), пытаемся найти её внутри игрока
        if (_playerWeapon == null && _playerStats != null)
        {
            _playerWeapon = _playerStats.GetComponentInChildren<WeaponFire>();
        }

        // Если пушки всё ещё нет, сбрасываем UI в 0
        if (_playerWeapon == null || _playerStats == null || _playerStats.Weapon == null)
        {
            if (ammoText != null) ammoText.text = "0 / 0";
            if (reloadRoot != null && reloadRoot.activeSelf) reloadRoot.SetActive(false);
            return;
        }

        // 1. Получаем патроны
        int currentAmmo = _playerWeapon.GetCurrentAmmo();
        int maxAmmo = _playerStats.Weapon.shootingStats.magazineSize;
        
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }

        // 2. Оптимизированный UI перезарядки
        bool isReloading = _playerWeapon.IsReloading();

        if (reloadRoot != null)
        {
            if (reloadRoot.activeSelf != isReloading)
            {
                reloadRoot.SetActive(isReloading);
            }
        }

        if (isReloading && reloadRadial != null)
        {
            reloadRadial.fillAmount = _playerWeapon.ReloadProgress;
        }
    }

    private void UpdateWaveInfo()
    {
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

    // Если вы вызываете этот метод при спавне игрока
    public void SetPlayer(GameObject player)
    {
        if (player == null) return;
        
        _playerStats = player.GetComponent<VehicleStats>();
        _playerHealth = player.GetComponent<Health>();
        _playerFuel = player.GetComponent<Fuel>();
        
        // Пытаемся найти пушку. Если её пока нет, UpdateAmmoAndReload найдет её позже.
        _playerWeapon = player.GetComponentInChildren<WeaponFire>();
    }
}