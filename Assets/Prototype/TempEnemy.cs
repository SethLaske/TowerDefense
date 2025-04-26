using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP = 0;
    
    public float lineT = .5f;

    //Starting at -1 so can walk from off screen
    public int currentSegment = -1;

    public float speed = 5;

    public Rigidbody2D rb;

    public Vector2 targetPoint;
    public Vector2 movementDirection;

    public Pathing pathingController;
    
    private void Start()
    {
        currentHP = maxHP;
        MoveToNextPoint();
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector2 currentDirectionToTarget = targetPoint - new Vector2(currentPosition.x, currentPosition.y);
        if (Vector2.Dot(currentDirectionToTarget, movementDirection) < 0)
        {
            //Past Target, get new target
            MoveToNextPoint();
        }
    }

    public void MoveToNextPoint()
    {
        targetPoint = pathingController.GetNextTargetPoint(currentSegment, lineT);

        if (targetPoint == Vector2.zero)
        {
            Destroy(gameObject);
            Debug.Log("Reached the end");
            return;
        }
        
        Vector3 currentPosition = transform.position;
        movementDirection = (targetPoint - new Vector2(currentPosition.x, currentPosition.y)).normalized;

        rb.velocity = movementDirection * speed;

        currentSegment++;
    }

    public void TakeDamage(int argDamage)
    {
        currentHP -= argDamage;

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
