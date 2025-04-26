using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeRingController : MonoBehaviour
{
    private Camera mainCamera = null;
    private RectTransform thisRect = null;
    
    [SerializeField] 
    private TowerSpace targetedTowerSpace = null;
    
    [SerializeField] 
    private Button sellButton = null;
    
    [SerializeField] 
    private Button rallyButton = null;

    [SerializeField] 
    private List<UpgradeRingButtonPositions> buttonPositions = null;

    private UpgradeRingButtonPositions activeButtonPositions = null;
    
    public UpgradeButtonController buttonControllerPrefab = null;
    
    public void Awake()
    {
        mainCamera = Camera.main;
        thisRect = GetComponent<RectTransform>();
        
        for (int i = 0; i < buttonPositions.Count; i++)
        {
            if ((i + 1) != buttonPositions[i].buttonPositions.Count)
            {
                Debug.LogError("[UpgradeRingController] - Button positions not scaling correctly");
            }

            buttonPositions[i].SpawnButtonControllers(buttonControllerPrefab);
        }
        
        sellButton.onClick.AddListener(OnSellButtonConfirmed);
        
        rallyButton.onClick.AddListener(OnRallyButtonPressed);
    }

    public void Configure(TowerSpace argTarget, UpgradeOptionsSO argUpgradeOptions)
    {
        if (targetedTowerSpace != null)
        {
            targetedTowerSpace.OnUpgradeOptionCancelled();
        }

        targetedTowerSpace = argTarget;
        if (targetedTowerSpace == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        
        
        sellButton.gameObject.SetActive(targetedTowerSpace.towerController != null);

        bool hasTroops = targetedTowerSpace.towerController != null &&
                         targetedTowerSpace.towerController.CanRally();
        rallyButton.gameObject.SetActive(hasTroops);

        int upgradeOptionsCount = argUpgradeOptions != null ? argUpgradeOptions.towerUpgrades.Count : 0;
        
        for (int i = 0; i < buttonPositions.Count; i++)
        {
            if (buttonPositions[i].buttonPositions.Count != upgradeOptionsCount)
            {
                buttonPositions[i].gameObject.SetActive(false);
            }
            else
            {
                activeButtonPositions = buttonPositions[i];
                activeButtonPositions.gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < upgradeOptionsCount; i++)
        {
            activeButtonPositions.buttonControllers[i].Configure(argUpgradeOptions.towerUpgrades[i], OnUpgradeOptionSelected, OnUpgradeOptionConfirmed);
        }
        
        CenterOnObject();
    }

    public void DoUpdate(float argDelta)
    {
        if (targetedTowerSpace == null)
        {
            gameObject.SetActive(false);
            activeButtonPositions = null;
            return;
        }
        
        CenterOnObject();
    }

    private void CenterOnObject()
    {
        if (targetedTowerSpace == null)
        {
            return;
        }
        
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetedTowerSpace.transform.position);

        // Check if object is in front of the camera
        if (screenPosition.z > 0)
        {
            thisRect.position = screenPosition;
        }
    }

    public void OnUpgradeOptionSelected(UpgradeButtonController argSelectedButton, TowerBaseController argSelectedOption)
    {
        foreach (UpgradeButtonController buttonController in activeButtonPositions.buttonControllers)
        {
            if (buttonController != argSelectedButton)
            {
                buttonController.ResetButton();
            }
        }
        
        targetedTowerSpace.OnUpgradeOptionSelected(argSelectedOption);
    }

    public void OnUpgradeOptionConfirmed()
    {
        targetedTowerSpace.OnUpgradeOptionConfirmed();
        targetedTowerSpace = null;
        gameObject.SetActive(false);
    }

    private void OnSellButtonConfirmed()
    {
        if (targetedTowerSpace != null)
        {
            targetedTowerSpace.SellTower();
        }
        targetedTowerSpace = null;
        gameObject.SetActive(false);
        activeButtonPositions = null;
    }
    
    private void OnRallyButtonPressed()
    {
        targetedTowerSpace.towerController.EnableRallyMode();
        targetedTowerSpace = null;
        gameObject.SetActive(false);
        activeButtonPositions = null;
    }

    public void RefreshActiveButtons()
    {
        if (activeButtonPositions == null)
        {
            return;
        }

        foreach (UpgradeButtonController upgradeButton in activeButtonPositions.buttonControllers)
        {
            upgradeButton.RefreshButton();
        }
    }
}


