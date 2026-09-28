using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseUI;

    private void Awake()
    {
        if ((bool)_pauseUI)
            throw new SerializeFieldNotSetException("The _pauseUI field has not been set in the editor.");
    }

    private void Start() =>
        _pauseUI.SetActive(false);

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
            _pauseUI.SetActive(false);
        }
        else
        {
            // actives the pause ui
            _pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
