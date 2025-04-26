using System.Collections.Generic;
using UnityEngine;

public class UpgradeRingButtonPositions : MonoBehaviour
{
    public List<Transform> buttonPositions;

    public List<UpgradeButtonController> buttonControllers = new List<UpgradeButtonController>();

    public void SpawnButtonControllers(UpgradeButtonController argButtonPrefab)
    {
        foreach (Transform buttonPosition in buttonPositions)
        {
            buttonControllers.Add(Instantiate(argButtonPrefab, buttonPosition));
        }
    }
}