using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(Constants.TutorialLevelSceneName);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
