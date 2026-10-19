using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    private const string DialogueInputName = "Input"; // Change this constant when a proper input is decided
    
    [Header("UI Panels")]
    [SerializeField] private GameObject _dialogueTextComponent;
    [SerializeField] private GameObject _characterNameComponent;
    
    [Header("Dialogue Selection")]
    [SerializeField] private DialogueDataScriptableObject dialogueDataScriptableObject;

    [Header("Character Animations")] 
    [SerializeField] private CharacterAnimationCollection _characterAnimations = new();

    private Animator _characterAnimator;
    private OldPlayerController _oldPlayerController;
    private Text _dialogueTextTextComponent;
    private Text _characterNameTextComponent; 
    [SerializeField] private float _textWriteSpeedCap = 0.5f;
    private int _currentDialogueCharacterIndex;
    private int _currentDialogueNodeIndex;
    private bool _isDialogueActive;
    private bool _isNotWritingText;
    
    private void Awake()
    {
        if (!SceneHelper.TryFindFirstObjectByTypeInScene<OldPlayerController>(out _oldPlayerController))
            throw new CouldNotFindGameObjectException("Could not find a player controller in the scene.");

        if (!TryGetComponent<Animator>(out _characterAnimator))
            _characterAnimator = this.AddComponent<Animator>();

        if (!((bool)_dialogueTextComponent && (bool)_characterNameComponent &&
            (bool)dialogueDataScriptableObject && _characterAnimations != null))
            throw new MissingComponentException("Missing one of the following components. " +
                                                "_dialogueTextComponent, _choicesComponent, _dialogueData");

        if (!_dialogueTextComponent.TryGetComponent<Text>(out _dialogueTextTextComponent))
            throw new MissingComponentException("_dialogueTextComponent is missing a Text component.");

        if (!_characterNameComponent.TryGetComponent<Text>(out _characterNameTextComponent))
            throw new MissingComponentException("_characterNameComponent is missing a Text component.");
    }
    
    private void Update()
    {
        if (_isDialogueActive)
            UpdateDialogue();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetButtonDown("Jump"))
            StartDialogue();
    }

    private void StartDialogue()
    {
        ActivateUI();
        
        _characterNameTextComponent.text = dialogueDataScriptableObject.CharacterName;
        
        _oldPlayerController.SetInputEnabled(false);
        
        EnableCharacterTalkingAnimation();

        _isDialogueActive = true;
    }

    private void EndDialogue()
    {
       DeactivateUI();

       _characterNameTextComponent.text = "";
       _dialogueTextTextComponent.text = "";
       
       _oldPlayerController.SetInputEnabled(true);
       
       DisableCharacterTalkingAnimation();

       _isDialogueActive = false;
    }
    
    private void ActivateUI()
    {
       ActivateUIComponent(_dialogueTextComponent);
       ActivateUIComponent(_characterNameComponent); 
        
       Canvas.ForceUpdateCanvases();
    }

    private void DeactivateUI()
    {
        DeactivateUIComponent(_dialogueTextComponent);
        DeactivateUIComponent(_characterNameComponent);

        Canvas.ForceUpdateCanvases();
    }
    
    private void WriteDialogueText()
    {
        if (Time.deltaTime < _textWriteSpeedCap)
            return;

        _isNotWritingText = false;
        
        DialogueNode currentDialogueNode = dialogueDataScriptableObject.DialogueNodes[_currentDialogueNodeIndex];

        if (Input.GetButtonDown(DialogueInputName))
            _dialogueTextTextComponent.text = currentDialogueNode.DialogueText;
        else
            _dialogueTextTextComponent.text += currentDialogueNode.DialogueText[_currentDialogueCharacterIndex];

        if (_dialogueTextTextComponent.text == currentDialogueNode.DialogueText)
            _isNotWritingText = true;
        else
            _currentDialogueCharacterIndex++;
    }

    private void UpdateDialogue()
    {
        if (_isNotWritingText && Input.GetButtonDown(DialogueInputName))
        {
            _currentDialogueCharacterIndex = 0; // We set the character index so that we start from the beginning with the next line of dialogue
            _currentDialogueNodeIndex++;
        }
        
        if (_currentDialogueNodeIndex == (dialogueDataScriptableObject.DialogueNodes.Count - 1) &&  // We need to remove 1 from count to get the last index of the list
            Input.GetButtonDown(DialogueInputName))
            EndDialogue();
        
        WriteDialogueText();
    }
    
    private void ActivateUIComponent(GameObject uiComponent)
    {
        if (!uiComponent.TryGetComponent<CanvasGroup>(out var canvasGroup))
            throw new MissingComponentException("uiElement is missing a CanvasGroup component.");
        
        uiComponent.SetActive(true);
        
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void DeactivateUIComponent(GameObject uiComponent)
    {
        if (!uiComponent.TryGetComponent<CanvasGroup>(out var canvasGroup))
            throw new MissingComponentException("uiElement is missing a CanvasGroup component.");
        
        uiComponent.SetActive(false);
        
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    private void DisableCharacterTalkingAnimation()
    {
        EnableAnimation(AnimationType.Idle);
        DisableAnimation(AnimationType.Talking);
    }

    private void EnableCharacterTalkingAnimation()
    {
        DisableAnimation(AnimationType.Idle);
        EnableAnimation(AnimationType.Talking);
    }

    private void DisableAnimation(AnimationType animationType) =>
        SetAnimation(animationType, false);
    
    private void EnableAnimation(AnimationType animationType) =>
        SetAnimation(animationType, true);

    private void SetAnimation(AnimationType animationType, bool animationState)
    {
        if (!_characterAnimations.TryGet(animationType, out string animationName))
            throw new AnimationNotDefinedException($"Animation of type {Enum.GetName(typeof(AnimationType),animationType)} is not defined.");
        
        _characterAnimator.SetBool(animationName, animationState);
    }
}