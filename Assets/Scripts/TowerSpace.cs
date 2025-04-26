using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpace : MonoBehaviour, ISelectable
{
    public TowerBaseController towerController = null;

    public UpgradeOptionsSO defaultUpgradeOptions = null;

    private TowerBaseController selectedUpgradeTowerController = null;
    
    public void DoUpdate(float argDelta)
    {
        if (towerController != null)
        {
            towerController.DoUpdate(argDelta);
        }
    }

    public void Select()
    {
        if (towerController == null)
        {
            //Debug.Log($"Tower Space Selected: {gameObject.name}");
            UIManager.instance.SetUpgradeRingToObject(this, defaultUpgradeOptions);
        }
        else
        {
            //Debug.Log($"Tower Selected: {towerController.name}");
            UIManager.instance.SetUpgradeRingToObject(this, towerController.towerUpgradeOptions);
        }
    }

    public void OnUpgradeOptionSelected(TowerBaseController argSelectedTowerBase)
    {
        if (towerController != null)
        {
            towerController.SetGraphicsVisibility(false);
        }

        if (selectedUpgradeTowerController != null)
        {
            //DestroyImmediate(selectedUpgradeTowerController);
            selectedUpgradeTowerController.gameObject.SetActive(false);
        }

        selectedUpgradeTowerController = Instantiate(argSelectedTowerBase, transform);
        selectedUpgradeTowerController.SetGraphicsVisibility(true);
        selectedUpgradeTowerController.SetTowerEnabled(false);
    }

    public void OnUpgradeOptionConfirmed()
    {
        if (towerController != null)
        {
            //DestroyImmediate(towerController);
            towerController.gameObject.SetActive(false);
        }

        towerController = selectedUpgradeTowerController;
        selectedUpgradeTowerController = null;
        
        towerController.OnTowerBuilt();
    }

    public void OnUpgradeOptionCancelled()
    {
        if (towerController != null)
        {
            towerController.SetGraphicsVisibility(true);
        }

        if (selectedUpgradeTowerController != null)
        {
            //DestroyImmediate(selectedUpgradeTowerController);
            selectedUpgradeTowerController.gameObject.SetActive(false);
            selectedUpgradeTowerController = null;
        }
    }

    public void SellTower()
    {
        if (towerController == null)
        {
            return;
        }
        
        //Destroy(towerController);
        towerController.gameObject.SetActive(false);
        towerController = null;
    }
}
