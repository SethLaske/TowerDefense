using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeOptions", menuName = "UpgradeOptionsSO")]
public class UpgradeOptionsSO : ScriptableObject
{
    public List<TowerBaseController> towerUpgrades = new List<TowerBaseController>();
    
    //List of upgrades
}


