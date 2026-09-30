using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitLevelController : MonoBehaviour
{
    [Header("Exit Script Settings")]
    [SerializeField] private float _exitDelayInSeconds = 2.0f;
    [SerializeField] private InputActionReference _cancelActionReference;
    
    // [Header("Required Assets")]

    private float _timeRemaining;
    private GameObject _guiIndicator;
    private TextMeshProUGUI _indicatorText;

    private void Awake()
    {
        _indicatorText = GameObject.Find("ExitLevelText").GetComponent<TextMeshProUGUI>();
        _guiIndicator = GameObject.Find("ExitLevelIndicator");
        _guiIndicator.SetActive(false);
        
        _timeRemaining = _exitDelayInSeconds;
        _cancelActionReference.action.Enable();
    }

    private void Update()
    {
        if (_cancelActionReference.action.ReadValue<float>().Equals(1))
        {
            _guiIndicator.SetActive(true);
            _timeRemaining -= Time.deltaTime;
            _indicatorText.text = "Exiting level in: " + _timeRemaining.ToString("F1");
            if (_timeRemaining <= 0)
                ExitToMenu();
        }
        else if(_timeRemaining < 2.0f)
        {
            _guiIndicator.SetActive(false);
            _timeRemaining = 2.0f;
        }
    }

    private void ExitToMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
