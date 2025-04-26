using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public UIManager uiManager = null;
    public InputManager inputManager = null;
    public TowerManager towerManager = null;
    public EnemyManager enemyManager = null;

    public PlayerController playerController = null;
    //public PathController pathController = null;
    public Transform gameplayPosition;
    
    [Space]
    [SerializeField]
    private LevelConfigSO _levelConfig = null;
    [HideInInspector]
    public LevelConfigSO levelConfig = null;
    
    public GameLogic gameLogic;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        levelConfig = Instantiate(_levelConfig);
    }

    // Start should only ever be called by GameManager
    private void Start()
    {
        DoFirstUpdate();
    }

    private void DoFirstUpdate()
    {
        gameLogic = new GameLogic();
        
        playerController.Initialize(levelConfig);
        
        inputManager.DoFirstUpdate();
        
        uiManager.DoFirstUpdate();
        
        towerManager.DoFirstUpdate();
        enemyManager.DoFirstUpdate();

        uiManager.SetUIState(UIManager.UIState.Map);
    }

    // Update should only ever be called by GameManager
    private void Update()
    {
        DoUpdate(Time.deltaTime);
    }

    private void DoUpdate(float argDelta)
    {
        inputManager.DoUpdate(argDelta);
        
        uiManager.DoUpdate(argDelta);
        
        
        // Gameplay elements can change speed, other features will not
        float modifiedDelta = argDelta * gameLogic.GetCurrentGameSpeedMultiplier();
        gameLogic.DoUpdate(modifiedDelta);
        
        towerManager.DoUpdate(modifiedDelta);   //Towers update outside of playing to allow for troop movements before the game
        if (gameLogic.currentGameState != GameLogic.GameState.Playing)
        {
            return;
        }
        enemyManager.DoUpdate(modifiedDelta);
    }

    public void SelectLevel()
    {
        gameLogic.SetupLevel(levelConfig);
        uiManager.SetUIState(UIManager.UIState.Gameplay);
    }
}


/*  Basic copy-paste for manager

    public static ___ instance = null;

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

    }

     */