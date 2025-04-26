using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseController : UnitBaseController
{
    [Range(0,1)]    //Determines where along the path the enemy walks
    public float pathOffset = .5f;
    
    //How far down the path, also used to determine which enemy is furthest
    //public int currentSegment = 0;
    public PathSegment currentSegment = null;
    
    public Vector2 targetPoint;
    //public PathSegment targetSegment = null;
    public Vector2 movementDirection;
    
    protected PathController pathController = null;

    [SerializeField] 
    private int goldOnDeath = 1;

    [SerializeField] 
    private LayerMask targetableUnitLayers;
    
    private List<TowerUnitBaseController> unitsBlockingEnemy = new List<TowerUnitBaseController>();

    protected override void Initialize()
    {
        currentSegment = pathController.GetSegmentAtIndex(0);
        SetNextPoint();
        
        base.Initialize();
    }

    public override void DoUpdate(float argDelta)
    {
        base.DoUpdate(argDelta);

        if (unitsBlockingEnemy.Count <= 0)
        {
            transform.position += Util.ToVector3(movementDirection) * (movementSpeed * argDelta);
            
            if (HasPastTargetPoint())
            {
                SetNextPoint();
            }
        }
        else
        {
            attackTimer.DoUpdate(argDelta);
        }
    }

    public void SetPathController(int argPathIndex)
    {
        pathController = GameManager.instance.gameLogic.pathControllers[argPathIndex];
    }

    private void SetNextPoint(bool argAdvanceImmediately = true)
    {
        currentSegment = currentSegment.nextSegment;
        
        if (currentSegment == null)
        {
            // Reached end of path
            GameManager.instance.playerController.TakeLives(1);
            HandleDeath();
            return;
        }

        targetPoint = currentSegment.GetPositionOnLine(pathOffset);

        if (argAdvanceImmediately)
        {
            AdvanceToTargetPoint();
        }
    }

    private void AdvanceToTargetPoint()
    {
        Vector3 currentPosition = transform.position;
        movementDirection = (targetPoint - new Vector2(currentPosition.x, currentPosition.y)).normalized;
    }

    private bool HasPastTargetPoint()
    {
        Vector3 currentPosition = transform.position;
        Vector2 currentDirectionToTarget = targetPoint - new Vector2(currentPosition.x, currentPosition.y);
        return Vector2.Dot(currentDirectionToTarget, movementDirection) <= 0;    //If the dot product is less than 0, the enemy has passed the target point
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        
        GameManager.instance.playerController.ModifyGold(goldOnDeath);
        
        EnemyManager.instance.RemoveEnemy(this);

        foreach (TowerUnitBaseController blockingUnits in unitsBlockingEnemy)
        {
            blockingUnits.OnEnemyDied(this);
        }
    }
    
    protected override void HandleAttack()
    {
        base.HandleAttack();
        
        if (unitsBlockingEnemy == null || unitsBlockingEnemy.Count <= 0)
        {
            return;
        }
        
        if (isSplashDamage == false)
        {
            unitsBlockingEnemy[0].TakeDamage(attackDamage);
        }
        else
        {
            Vector2 attackCenterPoint = unitsBlockingEnemy[0].transform.position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCenterPoint, splashRange, targetableUnitLayers);

            foreach (Collider2D col in colliders)
            {
                TowerUnitBaseController towerUnitController = col.GetComponent<TowerUnitBaseController>();
                if (towerUnitController != null)
                {
                    towerUnitController.TakeDamage(attackDamage);
                }
            }
        }
    }

    public void AddUnitToBlockingEnemies(TowerUnitBaseController argBlockingUnitController)
    {
        if (argBlockingUnitController != null)
        {
            unitsBlockingEnemy.Add(argBlockingUnitController);
        }
    }

    public void RemoveUnitFromBlockingEnemies(TowerUnitBaseController argUnBlockingUnitController)
    {
        for (int i = unitsBlockingEnemy.Count - 1; i >= 0; i--)
        {
            if (unitsBlockingEnemy[i] == argUnBlockingUnitController)
            {
                unitsBlockingEnemy.RemoveAt(i);
            }
        }
    }
}
