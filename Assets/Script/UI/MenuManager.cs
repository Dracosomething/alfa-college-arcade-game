using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _cancelInputActionReference;
    [SerializeField] private GameObject pauseUI;
    private void Start()
    {
        pauseUI.SetActive(false);
    }

    private void Update()
    {
        if (_cancelInputActionReference.action.triggered)
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
