using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBaseController : MonoBehaviour
{
    public float towerRadius;

    public UpgradeOptionsSO towerUpgradeOptions = null;
    
    public int costToPurchase;
    public int baseSalePrice;

    public GameObject graphicsObject = null;

    public bool towerEnabled = false;

    public bool isRallying = false;
    
    [SerializeField] private LayerMask targetableEnemyLayers;
    
    public virtual void DoUpdate(float argDelta)
    {
        if (towerEnabled == false)
        {
            return;
        }
    }

    public virtual void OnTowerBuilt()
    {
        SetGraphicsVisibility(true);
        SetTowerEnabled(true);
    }

    protected EnemyBaseController GetTargetEnemy()
    {
        EnemyBaseController targettedEnemy = null;
        // Get all colliders within radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, towerRadius, targetableEnemyLayers);

        foreach (Collider2D col in colliders)
        {
            EnemyBaseController enemyController = col.GetComponent<EnemyBaseController>();
            if (enemyController != null && 
                (targettedEnemy == null || enemyController.currentSegment.sectionsFromEnd < targettedEnemy.currentSegment.sectionsFromEnd))
            {
                targettedEnemy = enemyController;
            }
        }

        return targettedEnemy;
    }

    public void SetGraphicsVisibility(bool argIsVisible)
    {
        graphicsObject.SetActive(argIsVisible);
    }

    public void SetTowerEnabled(bool argIsTowerEnabled)
    {
        towerEnabled = argIsTowerEnabled;
    }

    public void EnableRallyMode()
    {
        if (CanRally() == false)
        {
            return;
        }

        isRallying = true;
        InputManager.instance.SubscribeToTap(this, CheckRallyPoint);
    }

    protected virtual void CheckRallyPoint(Vector2 tappedPosition)
    {
        
    }

    public virtual bool CanRally()
    {
        return false;
    }
}
