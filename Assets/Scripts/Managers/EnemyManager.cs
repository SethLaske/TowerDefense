using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance = null;

    private Transform enemyParent = null;
    
    private List<EnemyBaseController> activeEnemies = new List<EnemyBaseController>();

    private List<EnemyBaseController> toBeAddedEnemies = new List<EnemyBaseController>();
    private List<EnemyBaseController> toBeRemovedEnemies = new List<EnemyBaseController>();

    //private EnemySpawnsSO enemySpawns => GameManager.instance.levelConfig.waveData;
    private List<EnemyLine> scheduledSpawns = new List<EnemyLine>();
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void DoFirstUpdate()
    {
        
    }
    
    public void DoUpdate(float argDelta)
    {
        TrySpawnEnemies();
        
        if (toBeAddedEnemies.Count > 0)
        {
            foreach (EnemyBaseController enemyToAdd in toBeAddedEnemies)
            {
                if (activeEnemies.Contains(enemyToAdd) == false)
                {
                    activeEnemies.Add(enemyToAdd);
                }
            }
            
            toBeAddedEnemies.Clear();
        }
        
        foreach (EnemyBaseController enemy in activeEnemies)
        {
            enemy.DoUpdate(argDelta);
        }

        if (toBeRemovedEnemies.Count > 0)
        {
            foreach (EnemyBaseController enemyToRemove in toBeRemovedEnemies)
            {
                if (activeEnemies.Contains(enemyToRemove))
                {
                    activeEnemies.Remove(enemyToRemove);
                }
            }
            
            toBeRemovedEnemies.Clear();
        }
    }

    public void SetEnemyParentObject(Transform argEnemyParent)
    {
        enemyParent = argEnemyParent;
    }

    private void TrySpawnEnemies()
    {
        float currentTime = GameManager.instance.gameLogic.timeSinceGameStarted;

        for (int i = scheduledSpawns.Count - 1; i >= 0; i--)
        {
            EnemyLine enemy = scheduledSpawns[i];
            if (currentTime < enemy.gameTimeToSpawn)
            {
                continue;
            }

            foreach (float pathOffset in enemy.pathOffsets)
            {
                SpawnEnemy(enemy.pathControllerIndex, enemy.enemyType, pathOffset);
            }
            
            scheduledSpawns.RemoveAt(i);
        }
    }

    private void SpawnEnemy(int argEnemyPathIndex, EnemyBaseController argEnemyType, float argPathOffset)
    {
        if (argEnemyPathIndex >= GameManager.instance.gameLogic.pathControllers.Count)
        {
            Debug.LogError($"[EnemyBaseController] - PathController does not exist at index {argEnemyPathIndex}");
            return;
        }
        
        EnemyBaseController newEnemy = Instantiate(argEnemyType, enemyParent);
        
        newEnemy.pathOffset = argPathOffset;
        newEnemy.SetPathController(argEnemyPathIndex);
        newEnemy.transform.position = GameManager.instance.gameLogic.pathControllers[argEnemyPathIndex].GetNextTargetPoint(0, argPathOffset);
        
        toBeAddedEnemies.Add(newEnemy);
    }

    public void NewWaveStarted(WaveData argWaveData)
    {
        PopulateEnemyLines(argWaveData);
    }

    private void PopulateEnemyLines(WaveData argWaveData)
    {
        foreach (WaveEnemies waveEnemy in argWaveData.enemies)
        {
            // Invalid data
            if (waveEnemy.count <= 0 || waveEnemy.enemyType == null)
            {
                continue;
            }

            //Spawning individual enemy
            if (waveEnemy.count == 1)
            {
                scheduledSpawns.Add(new EnemyLine(waveEnemy.enemyType, waveEnemy.pathOffset, GameManager.instance.gameLogic.timeSinceGameStarted + waveEnemy.timeSinceWaveStartToSpawn, waveEnemy.pathIndex));
                continue;
            }

            //Spawning multiple enemies
            int numberOfEnemies = waveEnemy.count;
            float timeToSpawnLine = GameManager.instance.gameLogic.timeSinceGameStarted + waveEnemy.timeSinceWaveStartToSpawn;
            
            while (numberOfEnemies > 0)
            {
                
                if (waveEnemy.spawnsWide > 1)
                {
                    //Spawn a row of enemies spread across the path
                    int enemiesToSpawnInRow = Mathf.Min(waveEnemy.spawnsWide, numberOfEnemies);
                    List<float> pathOffsets = new List<float>();
                    for (int i = 0; i < enemiesToSpawnInRow; i++)
                    {
                        pathOffsets.Add((float)i/(enemiesToSpawnInRow - 1));
                    }
                    scheduledSpawns.Add(new EnemyLine(waveEnemy.enemyType, pathOffsets, timeToSpawnLine, waveEnemy.pathIndex));
                    
                    numberOfEnemies -= enemiesToSpawnInRow;
                }
                else
                {
                    //Spawn a single enemy at the set offset
                    scheduledSpawns.Add(new EnemyLine(waveEnemy.enemyType, waveEnemy.pathOffset, timeToSpawnLine, waveEnemy.pathIndex));
                    
                    numberOfEnemies --;
                }

                timeToSpawnLine += waveEnemy.timeBetweenSpawnLines;
            }
            
        }
    }

    public void RemoveEnemy(EnemyBaseController argEnemy)
    {
        toBeRemovedEnemies.Add(argEnemy);
    }
}
