using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Events (Inspector)")]
    public UnityEvent onDamageTaken;
    public UnityEvent onDeath;

    // Событие для обновления UI
    public event Action OnHealthChanged;

    private bool isDead = false;

    private void Start()
    {
        // Если VehicleStats (или другой скрипт) еще не установил здоровье, задаем его как максимальное
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = maxHealth;
        }
        
        // Оповещаем UI при появлении объекта
        NotifyHealthChanged();
    }

    public void TakeDamage(float amount)
    {
        // Оригинальная проверка на смерть
        if (isDead) return;

        // Отнимаем урон, но ограничиваем значения, чтобы полоска UI не ушла в минус
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); 
        
        // Оригинальный вызов события получения урона[cite: 1]
        onDamageTaken?.Invoke();
        
        // Оповещаем UI об изменении
        NotifyHealthChanged();

        // Оригинальная логика проверки на смерть[cite: 1]
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Выделен в отдельный публичный метод, чтобы VehicleStats мог дергать его при смене модулей
    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Оповещаем все системы (звук взрыва, визуальные эффекты)
        onDeath?.Invoke();

        if (gameObject.CompareTag("Player"))
        {
            // Выключаем объект игрока. Это мгновенно скроет машину 
            // и остановит все скрипты на ней (движение, стрельбу).
            gameObject.SetActive(false); 
            
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnPlayerDeath();
            }
        }
        else
        {
            // --- ИСПРАВЛЕННЫЙ БЛОК: Выброс награды ---
            EnemyReward reward = GetComponent<EnemyReward>();
            if (reward != null)
            {
                reward.DropRewards(); // Сообщаем BattleManager'у о заработке
            }
            // ----------------------------------------

            // Обязательно удаляем врага со сцены
            Destroy(gameObject, 0.1f);
        }
    }
}