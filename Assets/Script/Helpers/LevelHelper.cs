using UnityEngine.SceneManagement;

public static class LevelHelper
{
    public static void LoadLevelNonLinear(Level levelRequested)
    {
        var levelSceneName = Constants.LevelLookupTable.GetLevel(levelRequested);
        SceneManager.LoadScene(levelSceneName);
    }

    public static void LoadLevelLinear()
    {
        var levelSceneName = Constants.LevelLookupTable.NextLevel();
        SceneManager.LoadScene(levelSceneName);
    }
}