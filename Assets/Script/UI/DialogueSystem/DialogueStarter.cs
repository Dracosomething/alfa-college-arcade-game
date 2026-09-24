using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStarter : MonoBehaviour
{
    [Header("Dialogue Manager")]
    [SerializeField] private DialogueManager _dialogueManager;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject _dialogueContainerComponent;
    [SerializeField] private GameObject _choiceComponent;
    [SerializeField] private GameObject _exitButtonComponent;
    
    [Header("Dialogue Selection")]
    [SerializeField] private DialogueData _dialogueData;

    [Header("Character Animations")] 
    [SerializeField] private CharacterAnimationCollection _characterAnimations = new();
    
    private Animator _characterAnimator;
    private Button _exitButtonButtonComponent;
    private OldPlayerController _oldPlayerController;
    
    private void Awake()
    {
        if (!SceneHelper.TryFindFirstObjectByTypeInScene<OldPlayerController>(out _oldPlayerController))
            throw new CouldNotFindGameObjectException("Could not find a player controller in the scene.");

        if (!TryGetComponent<Animator>(out _characterAnimator))
            _characterAnimator = this.AddComponent<Animator>();

        if (!((bool)_dialogueContainerComponent && (bool)_choiceComponent && (bool)_exitButtonComponent &&
            (bool)_dialogueData && (bool)_dialogueManager && _characterAnimations != null))
            throw new MissingComponentException("Missing one of the following components. " +
                                                "_dialogueContainerComponent, _choicesComponent, _exitButtenComponent, _dialogueData" +
                                                ", _dialogueManager.");

        if (!_exitButtonComponent.TryGetComponent<Button>(out _exitButtonButtonComponent))
            throw new MissingComponentException("_exitButtonComponent is missing a Button component.");
    }
    
    private void Update()
    {
        // Re-enable player input when dialogue ends
        CheckDialogueState();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetButtonDown("Jump") && !_dialogueManager.IsDialogueActive)
            StartDialogue();
    }

    private void CheckDialogueState()
    {
        if (!_dialogueManager.IsDialogueActive)
        {
            DisableCharacterTalkingAnimation();
                
            _oldPlayerController.SetInputEnabled(true);
        }
    }

    private void StartDialogue()
    {
        ActivateUI();
        
        _oldPlayerController.SetInputEnabled(false);
        
        EnableCharacterTalkingAnimation();
        
        _dialogueManager.StartDialogue(_dialogueData.dialogueNodes);
    }

    private void ActivateUI()
    {
       ActivateUIComponent(_dialogueContainerComponent);
       ActivateUIComponent(_choiceComponent);
       ActivateUIComponent(_exitButtonComponent);

       _exitButtonButtonComponent.interactable = true;
        
        // Force UI refresh
        Canvas.ForceUpdateCanvases();
    }

    private void ActivateUIComponent(GameObject uiComponent)
    {
        if (!uiComponent.TryGetComponent<CanvasGroup>(out var canvasGroup))
            throw new MissingComponentException("uiElement is missing a CanvasGroup component.");
        
        uiComponent.SetActive(true);
        
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
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
            throw new AnimationNotDefinedException($"Animation of type {Enum.GetName<AnimationType>(animationType)} is not defined.");
        
        _characterAnimator.SetBool(animationName, animationState);
    }
}