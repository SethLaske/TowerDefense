using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfigSO")]
public class LevelConfigSO : ScriptableObject
{
    public int startingLives = 10;
    public int startingGold = 1000;

    public GameMap gameMap = null;
    
    //Tower upgrades, depending on how they are assigned

    public EnemySpawnsSO waveData = null;
}
