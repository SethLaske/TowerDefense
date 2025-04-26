using System;

[Serializable]
public class UpdateTimer
{
    private readonly float timePerAction;
    public float timeRemaining { get; private set; }

    private Action onTimerFinished;

    private bool continuousTimer;   // A continuous timer can be called multiple times per DoUpdate call. If False, the timer will reset immediately after calling the action

    public float timerCompletionPercent => (timePerAction - timeRemaining) / timePerAction;
    
    public UpdateTimer(float argTimePerAction, Action argOnTimerFinished, bool argContinuousTimer = true)
    {
        timePerAction = argTimePerAction;
        timeRemaining = timePerAction;

        onTimerFinished = argOnTimerFinished;

        continuousTimer = argContinuousTimer;
    }

    public void DoUpdate(float argDelta)
    {
        timeRemaining -= argDelta;
        while (timeRemaining <= 0)
        {
            //Allows the time to remain consistent regardless of frame rate and happen multiple times per Update if needed
            timeRemaining += timePerAction;

            if (continuousTimer == false)
            {
                timeRemaining = timePerAction;
                //return;
            }

            //Calling this last to allow the action to reset the timer
            onTimerFinished.Invoke();
        }
    }

    public void ResetTimer()
    {
        timeRemaining = timePerAction;
    }

    // Might be useful if something should happen immediately the first time, and after that be evenly spaced
    public void PrimeTimer()
    {
        timeRemaining = 0.01f;
    }
}
