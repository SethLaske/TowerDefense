using System.Collections;
using System.Collections.Generic;
using Prototpye;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            GameObject clickedObject = GetClickedGameObject();
            if (clickedObject != null)
            {
                Debug.Log("Clicked on: " + clickedObject.name);

                PrototypeTowerSpace tower = clickedObject.GetComponent<PrototypeTowerSpace>();

                if (tower != null)
                {
                    tower.Selected();
                }
            }
        }
    }

    GameObject GetClickedGameObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convert screen to world coordinates
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition); // Check for colliders at the position

        return hitCollider != null ? hitCollider.gameObject : null;
    }
}
