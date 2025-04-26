using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargettingTower : TowerBaseController
{
    private EnemyBaseController targettedEnemy = null;

    [SerializeField] 
    private float attackChargeTime = 1f;
    
    private UpdateTimer attackTimer = null;
    
    [SerializeField] 
    private int attackDamage = 10;

    private void Awake()
    {
        attackTimer = new UpdateTimer(attackChargeTime, HandleAttack, true);
    }

    public override void DoUpdate(float argDelta)
    {
        if (towerEnabled == false)
        {
            return;
        }

        if (targettedEnemy == null)
        {
            targettedEnemy = GetTargetEnemy();
        }

        if (targettedEnemy != null)
        {
            attackTimer.DoUpdate(argDelta);
        }
    }

    private void HandleAttack()
    {
        //Ensure a target is in range at the end of the charge
        targettedEnemy = GetTargetEnemy();
        
        //No more enemies
        if (targettedEnemy == null)
        {
            attackTimer.ResetTimer();
            return;
        }
        
        targettedEnemy.TakeDamage(attackDamage);
    }

    private void OnDrawGizmos()
    {
        if (towerEnabled == false)
        {
            return;
        }

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, towerRadius);

        float chargePercent = attackTimer.timerCompletionPercent;
        Gizmos.color = new Color(Mathf.Lerp(0, 1, chargePercent), 0, Mathf.Lerp(1, 0, chargePercent));
        if (targettedEnemy != null)
        {
            Gizmos.DrawLine(transform.position, targettedEnemy.transform.position);
        }
    }
}
