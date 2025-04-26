using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int gold;

    [SerializeField]
    private int livesRemaining;

    public void Initialize(LevelConfigSO argLevelConfig)
    {
        gold = argLevelConfig.startingGold;
        ModifyGold(0);
        livesRemaining = argLevelConfig.startingLives;
        TakeLives(0);
    }

    public void ModifyGold(int argChange)
    {
        gold += argChange;

        if (gold < 0)
        {
            gold = 0;
        }
        
        UIManager.instance.gameStatsController.SetGoldText(gold);
        UIManager.instance.upgradeRingController.RefreshActiveButtons();
    }

    public bool CanAffordCost(int argCost)
    {
        return gold >= argCost;
    }

    public void TakeLives(int argLivesTaken)
    {
        livesRemaining -= argLivesTaken;
        
        if (livesRemaining <= 0)
        {
            livesRemaining = 0;
            
            GameManager.instance.gameLogic.GameLost();
        }
        
        UIManager.instance.gameStatsController.SetLivesText(livesRemaining);
    }
}
