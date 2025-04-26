using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance = null;

    //public List<Transform> towerPositions = new List<Transform>();

    public TowerSpace towerSpacePrefab;

    private List<TowerSpace> towers = new List<TowerSpace>();
    
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
        /*foreach (Transform towerPosition in towerPositions)
        {
            if (towerPosition != null)
            {
                TowerSpace newTowerSpace = Instantiate(towerSpacePrefab, towerPosition);
                towers.Add(newTowerSpace);
            }
        }*/
    }

    public void DoUpdate(float argDelta)
    {
        foreach (TowerSpace tower in towers)
        {
            tower.DoUpdate(argDelta);
        }
    }

    public void SetTowerPositions(List<Transform> towerPositions)
    {
        foreach (Transform towerPosition in towerPositions)
        {
            if (towerPosition != null)
            {
                TowerSpace newTowerSpace = Instantiate(towerSpacePrefab, towerPosition);
                towers.Add(newTowerSpace);
            }
        }
    }
}
