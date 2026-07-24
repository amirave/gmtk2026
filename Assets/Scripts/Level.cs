using System;
using UnityEngine;

public delegate void OnLevelChange();

[Serializable]
public class Level
{
    public int levelNumber = 1;
    public int maxLevelNumber;
    public int successPerLevel;
    public OnLevelChange OnLevelSuccess { get; set; }
    public OnLevelChange OnLevelFail { get; set; }
    public OnLevelChange OnLevelChange { get; set; }

    private int _successes;

    public void Success()
    {
        _successes++;
        Debug.Log($"Success {_successes}/{successPerLevel} of Round #{levelNumber}");
            
        if (_successes < successPerLevel) return;
        _successes = 0;
        
        var previousLevel = levelNumber;
        levelNumber = Math.Min(levelNumber + 1, maxLevelNumber);

        if (previousLevel != levelNumber)
        {
            OnLevelFail?.Invoke();
            OnLevelChange?.Invoke();
        }
        
        Debug.Log($"Round {levelNumber} successful");
    }
        
    public void Fail()
    {
        _successes = 0;
        
        var previousLevel = levelNumber;
        levelNumber = Math.Max(levelNumber - 1, 1);

        if (previousLevel != levelNumber)
        {
            OnLevelFail?.Invoke();
            OnLevelChange?.Invoke();
        }

        Debug.Log($"Round #{levelNumber} failed");
    }
}