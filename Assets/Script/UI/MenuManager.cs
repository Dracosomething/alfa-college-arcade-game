using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseUI;
    private void Start()
    {
        if (pauseUI == null)
        {
            Debug.LogError("Pause UI is not assigned in the inspector.");
            return;
        }
        pauseUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseApplication();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void PauseApplication()
    {

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pauseUI.SetActive(false);
        }
        else
        {
            // actives the pause ui
            pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
