using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameStatsController : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI livesText;
    
    [SerializeField] 
    private TextMeshProUGUI goldText;
    
    [SerializeField] 
    private TextMeshProUGUI wavesText;

    public void Initalize()
    {
        
    }

    public void SetGoldText(int argCurrentGold)
    {
        goldText.text = argCurrentGold.ToString();
    }
    
    public void SetLivesText(int argCurrentLives)
    {
        livesText.text = argCurrentLives.ToString();
    }

    public void SetWaveText(int argCurrentWave, int argMaxWave)
    {
        string waveString = $"Wave {argCurrentWave}/";

        if (argMaxWave > 0)
        {
            waveString += argMaxWave.ToString();
        }
        else if(argMaxWave == 0){
            waveString += "??";
        }
        else
        {
            waveString += "Infinite";
        }

        wavesText.text = waveString;
    }
}
