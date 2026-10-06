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
        _currentLevel++;
        return _internalDictionary[_currentLevel];
    }
}