using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Additional UI elements to hide during dialogue (e.g., health bar fill, UI panels)")]
    [SerializeField] private GameObject[] _uiElementsToHide;
    
    [Header("Typewriter Settings")]
    [Tooltip("Speed of the typewriter effect (characters per second)")]
    [SerializeField] private float _typewriterSpeed = 30f;
    [Tooltip("Allow clicking to skip typewriter animation")]
    [SerializeField] private bool _allowSkip = true;
    
    [Header("Dialogue State")]
    [field: SerializeField] public bool IsDialogueActive { get; private set; } = false;
    
    [SerializeField] private Text _dialogueText;
    [SerializeField] private Text _characterNameText;
    [SerializeField] private GameObject _choicesContainer;
    [SerializeField] private GameObject _dialogueTextContainer;
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private Button _choiceButtonPrefab;
    private List<DialogueNode> _dialogueNodes;
    private Coroutine _typewriterCoroutine;
    private bool _skipRequested = false;
    private bool _isTyping = false;
    private string _currentFullText = "";
    private int _currentNodeIndex = 0;

    private void Awake()
    {
        if (!((bool)_exitButton && (bool)_dialogueTextContainer))
            throw new SerializeFieldNotSetException("One if the following fields has not been set." +
                                                    "_exitButton, _dialogueTextContainer, _typeWriterCoroutine.");
    }

    public void StartDialogue(List<DialogueNode> nodes)
    {
        _dialogueNodes = nodes;
        _currentNodeIndex = 0;
        
        // Activate UI panels and ensure they're interactable
        if (_dialogueTextContainer != null)
        {
            _dialogueTextContainer.SetActive(true);
            
            // Ensure Canvas Group doesn't block interactions
            var canvasGroup = _dialogueTextContainer.TryGetComponent<CanvasGroup>(out var cg) ? cg : _dialogueTextContainer.AddComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
            }
            else
            {
                Debug.LogWarning("DialogueManager: DialogueTextContainer does not have a CanvasGroup component. Adding one.");
            }
        }
        
        if (_exitButton != null)
        {
            _exitButton.SetActive(true);
            
            // Ensure Canvas Group doesn't block interactions
            var canvasGroup = _exitButton.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
            }
            
            // Ensure button component is interactable
            var button = _exitButton.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }
        else
        {
            Debug.LogWarning("DialogueManager: xButton reference is not assigned. Please assign it in the inspector.");
        }

        DisplayNode();
    }

    private void Update()
    {
        // Allow skipping typewriter effect with mouse click or space/enter
        if (_allowSkip && _isTyping && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            _skipRequested = true;
        }
    }
    
    private void DisplayNode()
    {
        if (_currentNodeIndex < 0 || _currentNodeIndex >= _dialogueNodes.Count) return;
        
        DialogueNode nodeScriptableObject = _dialogueNodes[_currentNodeIndex];
        _characterNameText.text = nodeScriptableObject.characterName;
        
        // Stop any existing typewriter coroutine
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
        }
        
        // Clear choices until typing is complete
        foreach (Transform child in _choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        // Start typewriter effect
        _currentFullText = nodeScriptableObject.dialogueText;
        _skipRequested = false;
        _typewriterCoroutine = StartCoroutine(TypewriterEffect(nodeScriptableObject));
    }
    
    private IEnumerator TypewriterEffect(DialogueNode nodeScriptableObject)
    {
        _isTyping = true;
        _dialogueText.text = "";
        
        for (int i = 0; i <= _currentFullText.Length; i++)
        {
            if (_skipRequested)
            {
                _dialogueText.text = _currentFullText;
                break;
            }
            
            _dialogueText.text = _currentFullText.Substring(0, i);
            yield return new WaitForSeconds(1f / _typewriterSpeed);
        }
        
        _isTyping = false;
        _skipRequested = false;
        
        // Now create the choice buttons after typing is complete
        if (nodeScriptableObject.choices.Count > 0)
        {
            // Activate choices container and ensure it's interactable
            _choicesContainer.SetActive(true);
            
            var canvasGroup = _choicesContainer.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
            }
            
            foreach (var choice in nodeScriptableObject.choices)
            {
                Button choiceButton = Instantiate(_choiceButtonPrefab, _choicesContainer.transform);
                choiceButton.GetComponentInChildren<Text>().text = choice.choiceText;
                choiceButton.onClick.AddListener(() => SelectChoice(choice.nextNodeIndex));
                
                // Ensure choice button is interactable
                choiceButton.interactable = true;
            }
        }

        if (nodeScriptableObject.choices.Count == 0)
        {
            StartCoroutine(HideDialogueAfterDelay(5f));
        }
    }

    public void SelectChoice(int nextNodeIndex)
    {
        if (nextNodeIndex >= 0 && nextNodeIndex < _dialogueNodes.Count)
        {
            _currentNodeIndex = nextNodeIndex;
            DisplayNode();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        
        // Stop typewriter coroutine if running
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
        }
        
        _isTyping = false;
        _dialogueText.text = "";
        _characterNameText.text = "";
        _choicesContainer.SetActive(false);

        _dialogueTextContainer.SetActive(false);
        
        _exitButton.SetActive(false);
    }

    private IEnumerator HideDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndDialogue();
    }
}