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
        // Если это НЕ игрок, и здоровье не задано в инспекторе — ставим максимум.
        // Здоровье Игрока мы больше не трогаем, так как за него отвечает PlayerPersistence.
        if (!gameObject.CompareTag("Player") && currentHealth <= 0 && !isDead)
        {
            currentHealth = maxHealth;
        }
        
        // Оповещаем UI при появлении объекта
        NotifyHealthChanged();
    }

    public void TakeDamage(float amount, bool isCritical, GameObject attacker)
    {
        // Если уже мертвы, игнорируем урон
        if (isDead || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Вызываем события получения урона и обновления UI
        onDamageTaken?.Invoke(); 
        NotifyHealthChanged();

        // Показываем цифры ТОЛЬКО если атакующий — игрок
        if (attacker != null && attacker.CompareTag("Player") && PopupManager.Instance != null)
        {
            PopupManager.Instance.ShowPlayerDamage(transform.position, amount, isCritical);
        }

        if (currentHealth <= 0) Die();
    }

    // Выделен в отдельный публичный метод для обновления интерфейса извне
    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Оповещаем все системы (звук взрыва, визуальные эффекты и т.д.)
        onDeath?.Invoke();

        if (gameObject.CompareTag("Player"))
        {
            // Выключаем объект игрока: это мгновенно скроет машину и 
            // остановит все активные скрипты на ней.
            gameObject.SetActive(false); 
            
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnPlayerDeath();
            }
        }
        else
        {
            // 1. Выброс наград за уничтожение врага
            EnemyReward reward = GetComponent<EnemyReward>();
            if (reward != null)
            {
                reward.DropRewards(); 
            }

            // 2. Удаление объекта со сцены.
            Destroy(gameObject, 0.1f);
        }
    }
}