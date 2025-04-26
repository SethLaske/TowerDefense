using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeButtonController : MonoBehaviour
{
    [SerializeField] 
    private TextButton button = null;

    private TowerBaseController upgradeOption = null;

    private bool isSelected = false;
    
    private UnityAction<UpgradeButtonController, TowerBaseController> onOptionSelected = null;
    private UnityAction onOptionConfirmed = null;

    public void Configure(TowerBaseController argTower, UnityAction<UpgradeButtonController, TowerBaseController> argOnOptionSelected, UnityAction argOnOptionConfirmed)
    {
        isSelected = false;
        button.SetText(argTower.costToPurchase.ToString());

        upgradeOption = argTower;

        onOptionSelected = argOnOptionSelected;
        onOptionConfirmed = argOnOptionConfirmed;
        
        button.SetOnClicked(OnButtonPressed);
        
        RefreshButton();
    }

    public void OnButtonPressed()
    {
        if (isSelected == false)
        {
            isSelected = true;
            onOptionSelected?.Invoke(this, upgradeOption);
            
            button.SetText("Confirm");
        }
        else
        {
            ResetButton();
            onOptionConfirmed?.Invoke();
        }
    }

    public void RefreshButton()
    {
        if (upgradeOption == null)
        {
            return;
        }

        button.interactable = GameManager.instance.playerController.CanAffordCost(upgradeOption.costToPurchase);
    }
    
    public void ResetButton()
    {
        isSelected = false;
        
        button.SetText(upgradeOption.costToPurchase.ToString());
    }
}
