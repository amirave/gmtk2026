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
    public GameObject wonLevelScreen;
    public List<RuleButton> winChoices;

    public GameObject lostLevelScreen;
    public LoseButton lostRestartButton;
    public LoseButton lostQuitButton;

    private int _successes;

    public async Task<RuleButton> WinLevel()
    {
        Debug.Log($"Round {levelNumber} successful");

        wonLevelScreen.SetActive(true);

        var taskList = winChoices.Select(choice => choice.WaitForClick()).ToList();
        var choice = await Task.WhenAny(taskList);
        var result = await choice;

        var buttonIndex = taskList.FindIndex(item => item == choice);

        wonLevelScreen.SetActive(false);

        return winChoices[buttonIndex];
    }

    public async Task<LoseButton> LostLevel()
    {
        Debug.Log($"Round #{levelNumber} failed");

        lostLevelScreen.SetActive(true);

        var buttonList = new List<LoseButton> { lostRestartButton, lostQuitButton };
        var taskList = buttonList.Select(button => button.WaitForClick()).ToList();

        var choice = await Task.WhenAny(taskList);
        var result = await choice;

        var buttonIndex = taskList.FindIndex(item => item == choice);

        lostLevelScreen.SetActive(false);
        
        return buttonList[buttonIndex];
    }

    public async Task<Tuple<bool, RuleButton>> Success()
    {
        _successes++;
        Debug.Log($"Success {_successes}/{successPerLevel} of Round #{levelNumber}");
            
        if (_successes < successPerLevel) return new Tuple<bool, RuleButton>(false, null);

        _successes = 0;
        var previousLevel = levelNumber;
        levelNumber = Math.Min(levelNumber + 1, maxLevelNumber);

        var button = await WinLevel();
        return new Tuple<bool, RuleButton>(previousLevel < levelNumber, button);
    }
        
    public async Task<Tuple<bool, LoseButton>> Fail()
    {
        _successes = 0;
        
        var previousLevel = levelNumber;
        levelNumber = Math.Max(levelNumber - 1, 1);

        var button = await LostLevel();
        return new Tuple<bool, LoseButton>(previousLevel > levelNumber, button);
    }
}