using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic
{
    // References
    private LevelConfigSO currentLevelConfig = null;
    private GameMap currentGameMap = null;
    
    public List<PathController> pathControllers = null;
    
    // Timing
    public float timeSinceGameStarted = 0;
    public int currentWaveIndex = 0;
    public float timeToStartNextWaveAt = 0;
    
    public enum GameState
    {
        NotActive,
        Paused,
        Playing
    }

    public GameState currentGameState = GameState.NotActive;
    
    public enum GameSpeed
    {
        //Paused,
        Half,
        Regular,
        Double
    }
    private GameSpeed currentGameSpeed = GameSpeed.Regular;
    
    public void DoUpdate(float argDelta)
    {
        if (currentGameState != GameState.Playing)
        {
            return;
        }

        timeSinceGameStarted += argDelta;

        if (timeSinceGameStarted > timeToStartNextWaveAt)
        {
            StartNextWave();
        }
    }
    
    public void OnGameSpeedChanged()
    {
        currentGameSpeed = Util.GetNextEnum(currentGameSpeed);
    }
    
    public float GetCurrentGameSpeedMultiplier()
    {
        switch (currentGameSpeed)
        {
            /*case GameSpeed.Paused:
                return 0;*/
            case GameSpeed.Half:
                return .5f;
            case GameSpeed.Regular:
                return 1;
            case GameSpeed.Double:
                return 2;
            default:
                return 1;
        }
    }
    
    public void SetupLevel(LevelConfigSO argLevelConfig)
    {
        currentLevelConfig = argLevelConfig;

        if (currentGameMap != null)
        {
            Object.Destroy(currentGameMap);
        }

        currentGameMap = Object.Instantiate(currentLevelConfig.gameMap, GameManager.instance.gameplayPosition);
        
        TowerManager.instance.SetTowerPositions(currentGameMap.towerPositions);
        GameManager.instance.playerController.Initialize(currentLevelConfig);
        EnemyManager.instance.SetEnemyParentObject(currentGameMap.enemyParent);
        pathControllers = currentGameMap.pathControllers;

        foreach (PathController pathController in pathControllers)
        {
            pathController.Initialize();
        }

        currentGameState = GameState.Paused;
    }

    public void StartLevel()
    {
        timeToStartNextWaveAt = currentLevelConfig.waveData.allWaves[currentWaveIndex].timeTillNextWave;
        UIManager.instance.gameStatsController.SetWaveText(currentWaveIndex + 1, currentLevelConfig.waveData.allWaves.Count);
        //Debug.Log($"Time to start first wave at: {timeToStartNextWaveAt}");

        currentGameState = GameState.Playing;
        
        EnemyManager.instance.NewWaveStarted(currentLevelConfig.waveData.allWaves[currentWaveIndex]);
        UIManager.instance.gameStatsController.SetWaveText(currentWaveIndex + 1, currentLevelConfig.waveData.allWaves.Count);
    }
    
    public void StartNextWave()
    {
        currentWaveIndex++;
        
        if (currentWaveIndex > currentLevelConfig.waveData.allWaves.Count)
        {
            GameWon();
            return;
        }
        timeToStartNextWaveAt = timeSinceGameStarted + currentLevelConfig.waveData.allWaves[currentWaveIndex].timeTillNextWave;
        //Debug.Log($"Time to start {currentWaveIndex} wave at: {timeToStartNextWaveAt}");
        
        EnemyManager.instance.NewWaveStarted(currentLevelConfig.waveData.allWaves[currentWaveIndex]);
        UIManager.instance.gameStatsController.SetWaveText(currentWaveIndex + 1, currentLevelConfig.waveData.allWaves.Count);
    }
    
    public void GameLost()
    {
        Debug.Log("Game Lost");

        currentGameState = GameState.Paused;
        //currentGameSpeed = GameSpeed.Paused;
        UIManager.instance.gameSpeedButton.SetText($"X {GetCurrentGameSpeedMultiplier()}");
    }

    public void GameWon()
    {
        Debug.Log("Game Won");
        
        currentGameState = GameState.Paused;
    }

    public bool IsPointOnPath(Vector2 argPoint)
    {
        foreach (PathController pathController in pathControllers)
        {
            if (pathController.IsPointOnPath(argPoint))
            {
                return true;
            }
        }

        return false;
    }

    public Vector2 GetClosestPathPointToPoint(Vector2 argPoint)
    {
        Vector2 foundPoint = Vector2.zero;

        foreach (PathController pathController in pathControllers)
        {
            foundPoint = pathController.FindClosestPathPointToPoint(argPoint, foundPoint);
        }

        return foundPoint;
    }
}
