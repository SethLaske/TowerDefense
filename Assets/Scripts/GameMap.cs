using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    [Header("Hook ups")] 
    public SpriteRenderer background = null;

    public List<PathController> pathControllers = null;
    
    public List<Transform> towerPositions = null;

    public Transform enemyParent = null;
}
