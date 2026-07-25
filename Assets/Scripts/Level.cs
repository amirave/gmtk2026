using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Level
{
    public int levelNumber = 1;
    public int maxLevelNumber;
    public int successPerLevel;

    private int _successes;

    public bool Success()
    {
        _successes++;
        Debug.Log($"Success {_successes}/{successPerLevel} of Round #{levelNumber}");
            
        if (_successes < successPerLevel) return false;

        _successes = 0;
        var previousLevel = levelNumber;
        levelNumber = Math.Min(levelNumber + 1, maxLevelNumber);

        return true;
    }
        
    public bool Fail()
    {
        _successes = 0;
        
        var previousLevel = levelNumber;
        levelNumber = 1;//Math.Max(levelNumber - 1, 1);

        return true;
    }
}