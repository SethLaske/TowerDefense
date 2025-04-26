using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUnitBaseController : UnitBaseController
{
    [Space]
    private UnitSpawningTower towerController = null;

    private EnemyBaseController targettedEnemy = null;

    private Vector2 unitRallyPosition;

    [SerializeField] 
    private float maxDistanceFromRallyPoint = .5f;

    [SerializeField, Tooltip("Using squared value")]
    private float toleranceFromRallyPoint = .05f;

    [SerializeField] 
    private LayerMask targetableEnemyLayers;

    //Update Timers
    private UpdateTimer enemyCheckTimer = null;
    [SerializeField] private float enemyCheckFrequency = .25f;

    protected override void Awake()
    {
        base.Awake();

        enemyCheckTimer = new UpdateTimer(enemyCheckFrequency, UpdateTargetEnemy, false);
    }

    public override void DoUpdate(float argDelta)
    {
        base.DoUpdate(argDelta);

        if (targettedEnemy != null)
        {
            if ((Util.ToVector3(targettedEnemy.transform.position) - transform.position).sqrMagnitude > Mathf.Pow(attackRange, 2))
            {
                transform.position = Vector3.MoveTowards(transform.position, targettedEnemy.transform.position, movementSpeed * argDelta);
            }
            else
            {
                attackTimer.DoUpdate(argDelta);
            }
        }
        else
        {
            if ((Util.ToVector3(unitRallyPosition) - transform.position).sqrMagnitude > toleranceFromRallyPoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, unitRallyPosition, movementSpeed * argDelta);
            }

            if (GameManager.instance.gameLogic.currentGameState == GameLogic.GameState.Playing)
            {
                enemyCheckTimer.DoUpdate(argDelta);
            }
        }
    }
    
    public void SetTowerController(UnitSpawningTower argTowerController)
    {
        if (towerController != null)
        {
            Debug.LogError("[TowerUnitBaseController] - towerController already set");
        }

        towerController = argTowerController;
    }

    public void SetRallyPosition(Vector2 argNewRallyPosition)
    {
        unitRallyPosition = argNewRallyPosition;
        
        if (targettedEnemy != null)
        {
            targettedEnemy.RemoveUnitFromBlockingEnemies(this);
            targettedEnemy = null;
        }
        
        enemyCheckTimer.PrimeTimer();
    }

    private void UpdateTargetEnemy()
    {
        if (targettedEnemy != null)
        {
            targettedEnemy.RemoveUnitFromBlockingEnemies(this);
        }

        EnemyBaseController foundEnemy = null;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(unitRallyPosition, maxDistanceFromRallyPoint, targetableEnemyLayers);

        foreach (Collider2D col in colliders)
        {
            Debug.Log($"[TowerUnitBaseController] - Found collider {col.gameObject.name}");
            EnemyBaseController enemyController = col.GetComponent<EnemyBaseController>();
            if (enemyController != null && 
                (foundEnemy == null || enemyController.currentSegment.sectionsFromEnd < foundEnemy.currentSegment.sectionsFromEnd))
            {
                foundEnemy = enemyController;
            }
        }
        
        //TODO: Check through the tower controller to find a target that one of the other units has found so they can all attack together
        
        targettedEnemy = foundEnemy;
        
        if (targettedEnemy != null)
        {
            targettedEnemy.AddUnitToBlockingEnemies(this);
        }
    }

    protected override void HandleAttack()
    {
        base.HandleAttack();
        
        if (targettedEnemy == null)
        {
            return;
        }
        
        if (isSplashDamage == false)
        {
            targettedEnemy.TakeDamage(attackDamage);
        }
        else
        {
            Vector2 attackCenterPoint = targettedEnemy.transform.position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCenterPoint, splashRange, targetableEnemyLayers);

            foreach (Collider2D col in colliders)
            {
                EnemyBaseController enemyController = col.GetComponent<EnemyBaseController>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(attackDamage);
                }
            }
        }
    }

    public void OnEnemyDied(EnemyBaseController argKilledEnemy)
    {
        if (argKilledEnemy != targettedEnemy)
        {
            return;
        }
        
        attackTimer.ResetTimer();
        enemyCheckTimer.PrimeTimer();
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        
        towerController.RemoveTowerUnit(this);

        if (targettedEnemy != null)
        {
            targettedEnemy.RemoveUnitFromBlockingEnemies(this);
        }
    }
}
