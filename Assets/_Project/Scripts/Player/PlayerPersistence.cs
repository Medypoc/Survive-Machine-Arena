using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    [SerializeField] private PlayerDataSO profile;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        
        if (profile != null && health != null)
        {
            // 1. Задаем макс. ХП на основе текущего обвеса
            health.maxHealth = profile.GetTotalMaxHealth();

            // 2. ЗАЩИТА №1: Устанавливаем текущее здоровье из профиля
            float startHealth = profile.currentHealth;
            
            // Если здоровье на нуле (машина уничтожена), даем 1 ХП для корректной работы получения урона
            if (startHealth <= 0)
            {
                startHealth = 1f;
            }
            
            health.currentHealth = startHealth;
        }
    }

    private void OnDisable()
    {
        // При выходе со сцены сохраняем ХП
        if (profile != null && health != null)
        {
            profile.currentHealth = health.currentHealth;
        }
    }
}