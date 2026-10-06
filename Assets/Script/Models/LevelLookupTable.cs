using System;
using System.Collections.Generic;

public class LevelLookupTable
{
    /// <summary>
    /// Key is a Level entry, Value is the name of the level scene
    /// </summary>
    private Dictionary<Level, string> _internalDictionary = new()
    {
        { Level.Tutorial,         "TutorialLevelScene" },
        { Level.FrontEnd,         "FrontEndLevelScene" },
        { Level.BackEnd,          "BackEndLevelScene" },
        { Level.GameDevelopment,  "GameDevelopmentLevelScene" },
        { Level.SensorTechnology, "SensorTechnologyLevelScene" },
        { Level.FinalBoss,        "FinalBossLevelScene" },
    };
    private Level _currentLevel = Level.Tutorial;

    public string GetLevel(Level requestedLevel)
    {
        _currentLevel = requestedLevel;
        return _internalDictionary[requestedLevel];
    }

    public string NextLevel()
    {
        if (!IsCurrentLevelFinalLevel())
            return Constants.TitleScreenSceneName;
        
        MoveCurrentLevelToNextLevel();
        
        return _internalDictionary[_currentLevel];
    }

    private bool IsCurrentLevelFinalLevel()
    {
        var levelEnumLength = Enum.GetValues(typeof(Enum)).Length;
        // We can parse _currentLevel to an integer since enum's are integers behind the scene.
        return (int)_currentLevel >= levelEnumLength;
    }

    private void MoveCurrentLevelToNextLevel() =>
        // We use the '++' operator here to get the next level in linear order,
        // this can be done since enum's are integers behind the scenes.
        _currentLevel++;
}