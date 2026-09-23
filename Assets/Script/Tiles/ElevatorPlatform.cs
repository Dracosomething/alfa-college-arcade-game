using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    
    [Header("Side Gates")]
    [Tooltip("Sidegate objects that will open when the player enters and close when elevator reaches destination.")]
    [SerializeField] private Animator[] SideGateAnimators = new Animator[2];
    
    [Header("Button")]
    [SerializeField] private Animator ButtonAnimator;

    [Tooltip("The actual button GameObject/Collider that player must stand on")]
    [SerializeField] private Collider2D ButtonCollider;

    [Tooltip("Time in seconds the player must stand on button to activate elevator")]
    [SerializeField] private float ButtonPressTime = 2f;
    
    [Header("")]
    private Vector2 _previousPosition;
    private Rigidbody2D _rigidBody;
    private bool _isPlayerOnButton = false;
    private bool _isElevatorActivated = false;
    private bool _isPlayerRequiredToLeaveButton = false;
    private bool _isWaitingForPlayerToLeaveZone = false;
    private GameObject _playerGameObject;
    [SerializeField] private List<ElevatorEntry> ElevatorEntries = new List<ElevatorEntry>();
    [SerializeField] private int ElevatorEntriesIndex;
    [SerializeField] private Transform PlatformTransform;
    [SerializeField] private float Speed;
    [SerializeField] private float StartMovingDelay = 1.5f;

    private void Start()
    {
        if (ElevatorEntries.Count <= 1)
            throw new Exception("Needs at least 2 points!");

        _rigidBody = PlatformTransform.GetComponent<Rigidbody2D>();
        _previousPosition = _rigidBody.position;
    }
    
    private void Update()
    {
        CheckPlayerOnButton();
        
        if (_isWaitingForPlayerToLeaveZone)
            CheckPlayerInElevatorZone();
    }
    
    private void CheckPlayerOnButton()
    {
        if (ButtonCollider == null || _isElevatorActivated)
            return;
        
	if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
            return;
        
        Collider2D playerCollider = playerGameObject.GetComponent<Collider2D>();

        if (playerCollider != null && ButtonCollider.bounds.Intersects(playerCollider.bounds))
        {
            if (!_isPlayerOnButton)
            {
                _isPlayerOnButton = true;
                
                if (_isPlayerRequiredToLeaveButton)
                    return;
                
                int currentZoneId = ElevatorEntriesIndex;
                StartCoroutine(ButtonActivationProcess(currentZoneId));
            }

	    return;
        }

        if (_isPlayerOnButton)
        {
	    _isPlayerOnButton = false;
	    _isPlayerRequiredToLeaveButton = false;
    	}
    }
    
    private void CheckPlayerInElevatorZone()
    {
	if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
		return;
        
        Collider2D currentZoneCollider = ElevatorEntries[ElevatorEntriesIndex].Collider;

        if (currentZoneCollider == null)
		return;

        Collider2D playerCollider = playerGameObject.GetComponent<Collider2D>();

        if (playerCollider != null && !currentZoneCollider.bounds.Intersects(playerCollider.bounds))
        {
            if (ButtonAnimator != null)
            {
                ButtonAnimator.SetBool("ButtonUp", true);
                ButtonAnimator.SetBool("ButtonDown", false);
            }
            
            _isWaitingForPlayerToLeaveZone = false;
            _isPlayerRequiredToLeaveButton = true;
            
            StartCoroutine(ResetButtonUpAnimation());
        }
    }

    // left by the last group:
    /// Legacy method - now button detection is handled in Update()
    /// Keep this for compatibility but it no longer triggers elevator movement
    public void OnPlayerEnteredZone(int zoneId, Transform playerTransform)
    {
    }

    //  Is this function fully removable?
    public void OnPlayerExitedZone(int zoneId, Transform playerTransform)
    {
    }
    
    private IEnumerator ButtonActivationProcess(int zoneId)
    {
        float buttonTimer = 0f;
        
        while (_isPlayerOnButton && buttonTimer < ButtonPressTime && !_isElevatorActivated)
        {
            buttonTimer += Time.deltaTime;
            
            yield return null;
        }
        
        if (_isPlayerOnButton && buttonTimer >= ButtonPressTime && !_isElevatorActivated)
        {
            _isElevatorActivated = true;
            
            if (ButtonAnimator != null)
                ButtonAnimator.SetBool("ButtonDown", true);
            
            StartCoroutine(GoToNextPoint(zoneId));
        }
    }

    private IEnumerator GoToNextPoint(int zoneId)
    {
        if (zoneId < 0 || zoneId >= ElevatorEntries.Count)
        {
            Debug.LogWarning($"Invalid zoneId: {zoneId}");

            yield break;
        }

        int targetIndex;

        if (ElevatorEntriesIndex == zoneId)
        {
            targetIndex = (ElevatorEntriesIndex + 1) % ElevatorEntries.Count;

            OpenSideGates();

            yield return new WaitForSeconds(StartMovingDelay);
        }
        else
        {
            targetIndex = zoneId;
            
            OpenSideGates();
        }

        Transform nextPoint = ElevatorEntries[targetIndex].Transform;

        while (Vector2.Distance(PlatformTransform.position, nextPoint.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(PlatformTransform.position, nextPoint.position, Time.fixedDeltaTime * Speed);
            _rigidBody.MovePosition(newPosition);

            _rigidBody.linearVelocity = (newPosition - _previousPosition) / Time.fixedDeltaTime;
            _previousPosition = newPosition;

            yield return new WaitForFixedUpdate();
        }

        _rigidBody.MovePosition(nextPoint.position);
        _rigidBody.linearVelocity = Vector2.zero;
        ElevatorEntriesIndex = targetIndex;
        
        CloseSideGates();
        
        _isElevatorActivated = false;
        _isWaitingForPlayerToLeaveZone = true;
    }
    
    private IEnumerator ResetButtonUpAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (ButtonAnimator != null)
            ButtonAnimator.SetBool("ButtonUp", false);
    }
    
    private void OpenSideGates()
    {
        if (SideGateAnimators == null || SideGateAnimators.Length == 0)
		return;

        foreach (Animator sideGateAnimator in SideGateAnimators)
        {
            if (sideGateAnimator != null)
		    continue;

            sideGateAnimator.SetBool("Open", true);
            sideGateAnimator.SetBool("Close", false);
        }
    }
    
    private void CloseSideGates()
    {
        if (SideGateAnimators == null || SideGateAnimators.Length == 0)
		return;

        foreach (Animator sideGateAnimator in SideGateAnimators)
        {
	    if (sideGateAnimator != null)
		    continue;

	    sideGateAnimator.SetBool("Open", false);
	    sideGateAnimator.SetBool("Close", true);
	    
	    StartCoroutine(ResetCloseBool(sideGateAnimator));
        }
    }
    
    private IEnumerator ResetCloseBool(Animator animator)
    {
        yield return new WaitForSeconds(0.3f);

        if (animator != null)
            animator.SetBool("Close", false);
    }



    private void OnDrawGizmos()
    {
        if (ElevatorEntries == null || ElevatorEntries.Count < 2) return;

        Gizmos.color = Color.green;
        
	for (int i = 0; i < ElevatorEntries.Count; i++)
        {
            Vector3 current = ElevatorEntries[i].Transform.position;
            Vector3 next = ElevatorEntries[(i + 1) % ElevatorEntries.Count].Transform.position;
            Gizmos.DrawLine(current, next);
        }
    }
}
