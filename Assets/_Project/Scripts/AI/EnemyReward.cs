using UnityEngine;
using SurviveArena.Data; 

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

        // 2. Считаем рандомные ДЕНЬГИ из модификатора
        if (modifier != null)
        {
            // Берем диапазон напрямую из ScriptableObject (+1 нужен, чтобы включить максимум)
            _grantedMoney = Random.Range(modifier.minMoneyReward, modifier.maxMoneyReward + 1);
        }
        else
        {
            // Дефолтное значение-заглушка на случай, если модификатор забыли передать
            _grantedMoney = Random.Range(0, 21);
        }
    }

    // Вызывается скриптом Health при смерти
    public void DropRewards()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.AddMatchRewards(_grantedXP, _grantedMoney);
            
            PopupManager.Instance?.ShowXP(transform.position, _grantedXP);
        }
    }
}