using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
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
