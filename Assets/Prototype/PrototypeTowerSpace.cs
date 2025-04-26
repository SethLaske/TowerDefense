using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Prototpye
{
    public class PrototypeTowerSpace : MonoBehaviour
    {
        public GameObject tower;

        public bool isTowerEnabled;

        public float radius;

        public TempEnemy targettedEnemy = null;

        private void Awake()
        {
            tower.SetActive(false);
            isTowerEnabled = false;
        }

        private void Update()
        {
            if (isTowerEnabled == false)
            {
                return;
            }

            TargetEnemy();
        }

        public void Selected()
        {
            isTowerEnabled = (isTowerEnabled == false);
            tower.SetActive(isTowerEnabled);

        }

        public void TargetEnemy()
        {
            int furthestSection = -1;

            targettedEnemy = null;
            // Get all colliders within radius
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (Collider2D col in colliders)
            {
                TempEnemy component = col.GetComponent<TempEnemy>();
                if (component != null && component.currentSegment > furthestSection)
                {
                    targettedEnemy = component;
                    furthestSection = targettedEnemy.currentSegment;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (isTowerEnabled == false)
            {
                return;
            }

            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, radius);
            if (targettedEnemy != null)
            {
                Gizmos.DrawLine(transform.position, targettedEnemy.transform.position);
            }
        }
    }
}
