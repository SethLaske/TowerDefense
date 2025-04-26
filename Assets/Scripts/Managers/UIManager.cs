using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    public TextButton gameSpeedButton = null;
    
    public TextButton levelSelectionButton = null;
    
    public TextButton levelStartButton = null;

    public UpgradeRingController upgradeRingController = null;

    public UIGameStatsController gameStatsController = null;

    public GameObject gameplayUIObject;
    public GameObject mapUIObject;
    
    public enum UIState
    {
        Gameplay,
        Map
    }

    public UIState currentUIState;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void DoFirstUpdate()
    {
        if (gameSpeedButton != null)
        {
            gameSpeedButton.SetText($"X {GameManager.instance.gameLogic.GetCurrentGameSpeedMultiplier()}");
            gameSpeedButton.SetOnClicked(OnGameSpeedButtonClicked);
        }
        
        if (levelSelectionButton != null)
        {
            levelSelectionButton.SetText("Select Level");
            levelSelectionButton.SetOnClicked(OnLevelSelectedButtonClicked);
        }
        
        if (levelStartButton != null)
        {
            levelStartButton.SetText("Start Level");
            levelStartButton.SetOnClicked(OnLevelStartedButtonClicked);
        }
    }

    public void DoUpdate(float argDelta)
    {
        upgradeRingController.DoUpdate(argDelta);
    }

    public void SetUIState(UIState argUIState)
    {
        if (currentUIState == argUIState)
        {
            return;
        }

        currentUIState = argUIState;
        
        gameplayUIObject.SetActive(currentUIState == UIState.Gameplay);
        mapUIObject.SetActive(currentUIState == UIState.Map);
    }
    
    public void OnGameSpeedButtonClicked()
    {
        GameManager.instance.gameLogic.OnGameSpeedChanged();
        
        gameSpeedButton.SetText($"X {GameManager.instance.gameLogic.GetCurrentGameSpeedMultiplier()}");
    }
    
    public void OnLevelSelectedButtonClicked()
    {
        GameManager.instance.SelectLevel();
        levelStartButton.gameObject.SetActive(true);
    }
    
    public void OnLevelStartedButtonClicked()
    {
        GameManager.instance.gameLogic.StartLevel();
        levelStartButton.gameObject.SetActive(false);
    }

    public void SetUpgradeRingToObject(TowerSpace argTowerSpace, UpgradeOptionsSO argUpgradeOptions)
    {
        upgradeRingController.Configure(argTowerSpace, argUpgradeOptions);
    }

    public void HideUpgradeRing()
    {
        upgradeRingController.Configure(null, null);
    }
}
