using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private FloatingText _floatingTextPrefab;
    
    [Header("Damage Text Size")]
    [SerializeField] private float _minTextSize = 12f;
    [SerializeField] private float _maxTextSize = 20f;
    [SerializeField] private float _critTextSize = 25f;

    [Header("Damage Colors")]
    [SerializeField] private Color _normalDamageColor = Color.white;
    [SerializeField] private Color _critColor = Color.red; // Настройка цвета критов в инспекторе

    [Header("XP Settings")]
    [SerializeField] private Color _xpColor = new Color(1f, 0.8f, 0f);
    [SerializeField] private float _xpTextSize = 18f;

    private VehicleStats _playerStats; 

    private void Awake()
    {
        Instance = this;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerStats = player.GetComponent<VehicleStats>();
    }

    public void ShowPlayerDamage(Vector3 position, float amount, bool isCritical)
    {
        float finalSize;
        Color finalColor;

        if (isCritical)
        {
            // Используем размер и цвет для критического удара
            finalSize = _critTextSize;
            finalColor = _critColor;
        }
        else
        {
            finalColor = _normalDamageColor;
            
            if (_playerStats != null && _playerStats.Weapon != null)
            {
                // ИСПРАВЛЕНО: Теперь обращаемся к переменным через структуру damageStats
                float min = _playerStats.Weapon.damageStats.minDamage; 
                float max = _playerStats.Weapon.damageStats.maxDamage;

                float t = Mathf.InverseLerp(min, max, amount);
                finalSize = Mathf.Lerp(_minTextSize, _maxTextSize, t);
            }
            else
            {
                finalSize = _minTextSize;
            }
        }

        Vector3 velocity = new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 6f), 0);
        FloatingText popup = Instantiate(_floatingTextPrefab, position, Quaternion.identity);
        popup.Setup(Mathf.RoundToInt(amount).ToString(), finalColor, finalSize, velocity);
    }

    public void ShowXP(Vector3 position, int amount)
    {
        Vector3 velocity = new Vector3(Random.Range(-1f, 1f), 5f, 0);
        FloatingText popup = Instantiate(_floatingTextPrefab, position, Quaternion.identity);
        popup.Setup($"+{amount} XP", _xpColor, _xpTextSize, velocity);
    }
}