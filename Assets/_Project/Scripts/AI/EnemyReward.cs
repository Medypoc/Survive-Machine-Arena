using UnityEngine;
using SurviveArena.Data; // Подставь свой namespace для данных

public class EnemyReward : MonoBehaviour
{
    private int _grantedXP;
    private int _grantedMoney;

    // Вызывается из EnemySpawner сразу после загрузки модулей
    public void InitializeRewards(int classBaseXP, PartDataSO body, PartDataSO cab, PartDataSO weapon, EnemyModifierSO modifier)
    {
        // 1. Считаем стабильный ОПЫТ
        int partsXP = (body != null ? body.baseXP : 0) + 
                      (cab != null ? cab.baseXP : 0) + 
                      (weapon != null ? weapon.baseXP : 0);
                      
        float modMult = modifier != null ? modifier.xpMultiplier : 1.0f;
        
        _grantedXP = Mathf.RoundToInt((classBaseXP + partsXP) * modMult);

        // 2. Считаем рандомные ДЕНЬГИ на основе тира сложности
        DifficultyTier currentTier = modifier != null ? modifier.tier : DifficultyTier.Simple;
        
        switch (currentTier)
        {
            case DifficultyTier.Simple:
                _grantedMoney = Random.Range(0, 21);
                break;
            case DifficultyTier.Medium:
                _grantedMoney = Random.Range(25, 101);
                break;
            case DifficultyTier.Hard:
                _grantedMoney = Random.Range(50, 201);
                break;
        }
    }

    // Вызывается скриптом Health при смерти
    public void DropRewards()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.AddMatchRewards(_grantedXP, _grantedMoney);
            
            // --- НОВАЯ СТРОЧКА: Показываем заработанный опыт перед удалением врага ---
            PopupManager.Instance?.ShowXP(transform.position, _grantedXP);
            // -------------------------------------------------------------------------
        }
    }
}