using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitSpawningTower : TowerBaseController
{
    public Vector2 unitCenterPoint;

    public float unitSpreadRange = 1;

    [SerializeField] 
    private int startingUnits = 3;
    
    [SerializeField]
    private int maxUnits = 3;

    public List<TowerUnitBaseController> activeUnits = new List<TowerUnitBaseController>(); 
    
    //private List<TowerUnitBaseController> toBeAddedUnits = new List<TowerUnitBaseController>();
    private List<TowerUnitBaseController> toBeRemovedUnits = new List<TowerUnitBaseController>();
    
    [SerializeField] 
    private TowerUnitBaseController towerSpawnType = null;
    
    [SerializeField] 
    private float unitSpawnTime = 1f;

    private UpdateTimer unitSpawnTimer;

    private void Awake()
    {
        unitSpawnTimer = new UpdateTimer(unitSpawnTime, HandleSpawns, true);
    }

    public override void DoUpdate(float argDelta)
    {
        base.DoUpdate(argDelta);
        
        if (towerEnabled == false)
        {
            return;
        }

        UpdateUnits(argDelta);
        
        if (maxUnits != activeUnits.Count)
        {
            unitSpawnTimer.DoUpdate(argDelta);
        }
    }

    public override void OnTowerBuilt()
    {
        base.OnTowerBuilt();

        SetNewRallyPoint(GameManager.instance.gameLogic.GetClosestPathPointToPoint(transform.position));
        
        for (int i = 0; i < startingUnits; i++)
        {
            SpawnUnit();
        }
    }

    private void UpdateUnits(float argDelta)
    {
        /*if (toBeAddedUnits.Count > 0)
        {
            foreach (TowerUnitBaseController unitToAdd in toBeAddedUnits)
            {
                if (activeUnits.Contains(unitToAdd) == false)
                {
                    activeUnits.Add(unitToAdd);
                }
            }
            
            toBeAddedUnits.Clear();
        }*/
        
        foreach (TowerUnitBaseController unit in activeUnits)
        {
            if (unit != null)
            {
                unit.DoUpdate(argDelta);
            }

        }

        if (toBeRemovedUnits.Count > 0)
        {
            foreach (TowerUnitBaseController unitToRemove in toBeRemovedUnits)
            {
                if (activeUnits.Contains(unitToRemove))
                {
                    activeUnits.Remove(unitToRemove);
                }
            }
            
            toBeRemovedUnits.Clear();
        }
    }

    private void HandleSpawns()
    {
        if (activeUnits.Count < maxUnits)
        {
            SpawnUnit();
        }

        //No more units can be spawned
        if (activeUnits.Count >= maxUnits)
        {
            unitSpawnTimer.ResetTimer();
            return;
        }
    }
    
    private void SpawnUnit()
    {
        TowerUnitBaseController newUnit = Instantiate(towerSpawnType, transform.position, quaternion.identity, transform);
        
        newUnit.SetTowerController(this);
        
        activeUnits.Add(newUnit);
        
        UpdateUnitRallyPoints();
    }
    
    public void RemoveTowerUnit(TowerUnitBaseController argUnit)
    {
        toBeRemovedUnits.Add(argUnit);
    }
    
    protected override void CheckRallyPoint(Vector2 tappedPosition)
    {
        if (isRallying == false)
        {
            return;
        }

        float distanceFromTower = (tappedPosition - Util.ToVector2(transform.position)).magnitude;

        if (distanceFromTower <= towerRadius && GameManager.instance.gameLogic.IsPointOnPath(tappedPosition))
        {
            SetNewRallyPoint(tappedPosition);
            isRallying = false;
            Debug.Log("Found new rally position");
            InputManager.instance.UnsubscribeFromTap(this);
        }
        else
        {
            Debug.Log("Not valid rally point");
        }

    }

    //Assume the point is valid
    private void SetNewRallyPoint(Vector2 argNewRallyPoint)
    {
        unitCenterPoint = argNewRallyPoint;
        
        UpdateUnitRallyPoints();
    }

    private void UpdateUnitRallyPoints()
    {
        int unitCount = activeUnits.Count;
        
        if (unitCount <= 0)
        {
            return;
        }

        if (unitCount == 1)
        {
            activeUnits[0].SetRallyPosition(unitCenterPoint);
            return;
        }

        float angleStep = 360f / unitCount;

        for (int i = 0; i < unitCount; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 offset = new Vector2(
                Mathf.Cos(rad) * unitSpreadRange,
                Mathf.Sin(rad) * unitSpreadRange
            );

            if (activeUnits[i] != null)
            {
                activeUnits[i].SetRallyPosition(unitCenterPoint + offset);
            }
        }
    }

    public EnemyBaseController GetTargettableEnemy()
    {
        return null;
    }

    public override bool CanRally()
    {
        return true;
    }
    
    private void OnDrawGizmos()
    {
        if (towerEnabled == false)
        {
            return;
        }

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, towerRadius);
        
        Gizmos.color = Color.green;

        if (isRallying)
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(unitCenterPoint, unitSpreadRange);
    }
}
