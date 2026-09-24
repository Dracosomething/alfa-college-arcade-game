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
    [SerializeField] private Animator[] _sideGateAnimators = new Animator[2];
    
    [Header("Button")]
    [SerializeField] private Animator _buttonAnimator;

    [Tooltip("The actual button GameObject/Collider that player must stand on")]
    [SerializeField] private Collider2D _buttonCollider;

    [Tooltip("Time in seconds the player must stand on button to activate elevator")]
    [SerializeField] private float _requiredButtonPressTime = 2f;
    
    [Header("")]
    private Vector2 _previousPosition;
    private Rigidbody2D _rigidBody;
    private bool _isPlayerOnButton = false;
    private bool _isElevatorActivated = false;
    private bool _isPlayerRequiredToLeaveButton = false;
    private bool _isWaitingForPlayerToLeaveZone = false;
    private GameObject _playerGameObject;
    [SerializeField] private List<ElevatorEntry> _elevatorEntries = new List<ElevatorEntry>();
    [SerializeField] private int _elevatorEntriesIndex;
    [SerializeField] private Transform _platformTransform;
    [SerializeField] private float _speed;
    [SerializeField] private float _startMovingDelay = 1.5f;

    private void Start()
    {
        if (_elevatorEntries.Count <= 1)
            throw new Exception("Elevator Entries need at least 2 points.");

        _rigidBody = _platformTransform.GetComponent<Rigidbody2D>();
        _previousPosition = _rigidBody.position;
    }
    
    private void Update()
    {
        if (IsPlayerOnButton())
            StartCoroutine(ButtonActivationProcess(_elevatorEntriesIndex));
        
        if (_isWaitingForPlayerToLeaveZone)
            CheckPlayerInElevatorZone();
    }
    
    private bool IsPlayerOnButton()
    {
        if (_buttonCollider == null || _isElevatorActivated)
            return false;
        
	if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
            return false;
        
        Collider2D playerCollider = playerGameObject.GetComponent<Collider2D>();

        if (playerCollider != null && _buttonCollider.bounds.Intersects(playerCollider.bounds))
        {
            if (!_isPlayerOnButton)
            {
                _isPlayerOnButton = true;
                
                if (_isPlayerRequiredToLeaveButton)
                    return false;

		return true;
            }

	    return false;
        }

        if (_isPlayerOnButton)
        {
	    _isPlayerOnButton = false;
	    _isPlayerRequiredToLeaveButton = false;
    	}

	return false;
    }
    
    private void CheckPlayerInElevatorZone()
    {
	if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
		return;
        
        Collider2D currentZoneCollider = _elevatorEntries[_elevatorEntriesIndex].Collider;

        if (currentZoneCollider == null)
		return;

        Collider2D playerCollider = playerGameObject.GetComponent<Collider2D>();

        if (playerCollider != null && !currentZoneCollider.bounds.Intersects(playerCollider.bounds))
        {
            if (_buttonAnimator != null)
            {
                _buttonAnimator.SetBool("ButtonUp", true);
                _buttonAnimator.SetBool("ButtonDown", false);
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
        
        while (_isPlayerOnButton && buttonTimer < _requiredButtonPressTime && !_isElevatorActivated)
        {
            buttonTimer += Time.deltaTime;
            
            yield return null;
        }
        
        if (_isPlayerOnButton && buttonTimer >= _requiredButtonPressTime && !_isElevatorActivated)
        {
            _isElevatorActivated = true;
            
            if (_buttonAnimator != null)
                _buttonAnimator.SetBool("ButtonDown", true);
            
            StartCoroutine(GoToNextPoint(zoneId));
        }
    }

    private IEnumerator GoToNextPoint(int zoneId)
    {
        if (zoneId < 0 || zoneId >= _elevatorEntries.Count)
        {
            Debug.LogWarning($"Invalid zoneId: {zoneId}");

            yield break;
        }

        int targetIndex;

        if (_elevatorEntriesIndex == zoneId)
        {
            targetIndex = (_elevatorEntriesIndex + 1) % _elevatorEntries.Count;

            OpenSideGates();

            yield return new WaitForSeconds(_startMovingDelay);
        }
        else
        {
            targetIndex = zoneId;
            
            OpenSideGates();
        }

        Transform nextPoint = _elevatorEntries[targetIndex].Transform;

        while (Vector2.Distance(_platformTransform.position, nextPoint.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(_platformTransform.position, nextPoint.position, Time.fixedDeltaTime * _speed);
            _rigidBody.MovePosition(newPosition);

            _rigidBody.linearVelocity = (newPosition - _previousPosition) / Time.fixedDeltaTime;
            _previousPosition = newPosition;

            yield return new WaitForFixedUpdate();
        }

        _rigidBody.MovePosition(nextPoint.position);
        _rigidBody.linearVelocity = Vector2.zero;
        _elevatorEntriesIndex = targetIndex;
        
        CloseSideGates();
        
        _isElevatorActivated = false;
        _isWaitingForPlayerToLeaveZone = true;
    }
    
    private IEnumerator ResetButtonUpAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (_buttonAnimator != null)
            _buttonAnimator.SetBool("ButtonUp", false);
    }
    
    private void OpenSideGates()
    {
        if (_sideGateAnimators == null || _sideGateAnimators.Length == 0)
		return;

        foreach (Animator sideGateAnimator in _sideGateAnimators)
        {
            if (sideGateAnimator != null)
		    continue;

            sideGateAnimator.SetBool("Open", true);
            sideGateAnimator.SetBool("Close", false);
        }
    }
    
    private void CloseSideGates()
    {
        if (_sideGateAnimators == null || _sideGateAnimators.Length == 0)
		return;

        foreach (Animator sideGateAnimator in _sideGateAnimators)
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
        if (_elevatorEntries == null || _elevatorEntries.Count < 2) return;

        Gizmos.color = Color.green;
        
	for (int i = 0; i < _elevatorEntries.Count; i++)
        {
            Vector3 current = _elevatorEntries[i].Transform.position;
            Vector3 next = _elevatorEntries[(i + 1) % _elevatorEntries.Count].Transform.position;
            Gizmos.DrawLine(current, next);
        }
    }
}
