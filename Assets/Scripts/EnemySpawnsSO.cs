using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemySpawns", menuName = "EnemySpawnsSO")]
public class EnemySpawnsSO : ScriptableObject
{
     public List<WaveData> allWaves;
}

[Serializable]
public class WaveData
{
     public float timeTillNextWave = 10;
     public List<WaveEnemies> enemies;

     public List<WaveEnemies> GetEnemiesForPath(int argPathIndex)
     {
          List<WaveEnemies> enemiesAtPath = new List<WaveEnemies>();

          foreach (WaveEnemies waveEnemy in enemies)
          {
               if (waveEnemy.pathIndex == argPathIndex)
               {
                    enemiesAtPath.Add(waveEnemy);
               }
          }

          return enemiesAtPath;
     }
}

[Serializable]
public class WaveEnemies
{
     public EnemyBaseController enemyType = null;
     public int pathIndex = 0;
     public float timeSinceWaveStartToSpawn = 0;
     public int count;

     //Used if single spawn
     [Range(0, 1)] 
     public float pathOffset = 0;

     //Used if multi spawn
     public int spawnsWide = 0;
     //public int spawnsDeep = 0;
     public float timeBetweenSpawnLines = 0;
}

[Serializable]
public class EnemyLine
{
     public EnemyBaseController enemyType = null;
     public List<float> pathOffsets;
     public float gameTimeToSpawn;
     public int pathControllerIndex;

     public EnemyLine(EnemyBaseController argEnemyType, List<float> argPathOffsets, float argSpawnTime, int argPathControllerIndex)
     {
          enemyType = argEnemyType;
          pathOffsets = new List<float>(argPathOffsets);
          gameTimeToSpawn = argSpawnTime;
          pathControllerIndex = argPathControllerIndex;
     }
     
     public EnemyLine(EnemyBaseController argEnemyType, float argPathOffset, float argSpawnTime, int argPathControllerIndex)
     {
          enemyType = argEnemyType;
          pathOffsets = new List<float> { argPathOffset };
          gameTimeToSpawn = argSpawnTime;
          pathControllerIndex = argPathControllerIndex;
     }
}