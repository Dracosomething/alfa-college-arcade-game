using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ElevatorEntry
{
    public int id;
    public Transform transform;
    public Collider2D collider;
}

public class ElevatorTile : MonoBehaviour
{
    
    [Header("Sidegate Objects")]
    [Tooltip("Sidegate objects that will open when player enters and close when elevator reaches destination")]
    public Animator[] sidegateAnimators = new Animator[2];
    
    [Header("Button System")]
    public Animator buttonAnimator;
    
    [Tooltip("The actual button GameObject/Collider that player must stand on")]
    public Collider2D buttonCollider;
    
    [Tooltip("Time in seconds the player must stand on button to activate elevator")]
    public float buttonPressTime = 2f;

    private Vector2 previousPosition;
    private Rigidbody2D rb;
    private Transform nextPoint;
    private bool isPlayerOnButton = false;
    private float buttonTimer = 0f;
    private bool elevatorActivated = false;
    private Transform currentPlayerTransform;
    private bool requirePlayerToLeaveButton = false; // Prevents immediate re-activation after elevator movement
    private bool waitingForPlayerToLeaveZone = false; // Waiting for player to leave elevator zone before resetting button
    private GameObject _playerGameObject;

    [Header("")]
    public List<ElevatorEntry> ElevatorEntries = new List<ElevatorEntry>();
    public int index;
    public Transform Platform;
    public float speed;
    public float waitBeforeMove = 1.5f; // seconds to wait before elevator starts moving

    private void Start()
    {
        if (ElevatorEntries.Count <= 1)
            throw new Exception("Needs at least 2 points!");

        rb = Platform.GetComponent<Rigidbody2D>();
        previousPosition = rb.position;
    }
    
    private void Update()
    {
        CheckPlayerOnButton();
        
        if (waitingForPlayerToLeaveZone)
            CheckPlayerInElevatorZone();
    }
    
    private void CheckPlayerOnButton()
    {
        if (buttonCollider == null || elevatorActivated)
		return;
        
	if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
		return;
        
        Collider2D playerCollider = playerGameObject.GetComponent<Collider2D>();
        if (playerCollider != null && buttonCollider.bounds.Intersects(playerCollider.bounds))
        {
            if (!isPlayerOnButton)
            {
                isPlayerOnButton = true;
                currentPlayerTransform = playerGameObject.transform;
                
                if (requirePlayerToLeaveButton)
                    return;
                
                int currentZoneId = index;
                StartCoroutine(ButtonActivationProcess(currentZoneId));
            }

	    return;
        }

        if (isPlayerOnButton)
        {
	    isPlayerOnButton = false;
	    buttonTimer = 0f;
	    requirePlayerToLeaveButton = false;
    	}
    }
    
    private void CheckPlayerInElevatorZone()
    {
	if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
		return;
        
        Collider2D currentZoneCollider = ElevatorEntries[index].collider;
        if (currentZoneCollider == null)
		return;

        Collider2D playerCollider = playerGameObject.GetComponent<Collider2D>();

        if (playerCollider != null && !currentZoneCollider.bounds.Intersects(playerCollider.bounds))
        {
            // Debug.Log("Player left elevator zone - resetting button");
            if (buttonAnimator != null)
            {
                buttonAnimator.SetBool("ButtonUp", true);
                buttonAnimator.SetBool("ButtonDown", false);
            }
            
            waitingForPlayerToLeaveZone = false;
            requirePlayerToLeaveButton = true;
            
            StartCoroutine(ResetButtonUpAnimation());
        }
    }

    /// <summary>
    /// Legacy method - now button detection is handled in Update()
    /// Keep this for compatibility but it no longer triggers elevator movement
    /// </summary>
    public void OnPlayerEnteredZone(int zoneId, Transform playerTransform)
    {
        // Debug.Log($"Player entered elevator zone {zoneId} - use button to activate elevator");
    }
    
    /// <summary>
    /// Legacy method - now handled in Update()
    /// </summary>
    public void OnPlayerExitedZone(int zoneId, Transform playerTransform)
    {
        // Debug.Log($"Player exited elevator zone {zoneId}");
    }
    
    /// <summary>
    /// Handle button activation process - player must stand on button for specified time
    /// </summary>
    private IEnumerator ButtonActivationProcess(int zoneId)
    {
        buttonTimer = 0f;
        
        while (isPlayerOnButton && buttonTimer < buttonPressTime && !elevatorActivated)
        {
            buttonTimer += Time.deltaTime;
            
            yield return null;
        }
        
        if (isPlayerOnButton && buttonTimer >= buttonPressTime && !elevatorActivated)
        {
            elevatorActivated = true;
            
            if (buttonAnimator != null)
                buttonAnimator.SetBool("ButtonDown", true);
            
            StartCoroutine(GoToNextPoint(zoneId));
        }
    }

    public IEnumerator GoToNextPoint(int zoneId)
    {
        if (zoneId < 0 || zoneId >= ElevatorEntries.Count)
        {
            Debug.LogWarning($"Invalid zoneId: {zoneId}");

            yield break;
        }

        int targetIndex;

        if (index == zoneId)
        {
            targetIndex = (index + 1) % ElevatorEntries.Count;

            OpenSidegates();

            yield return new WaitForSeconds(waitBeforeMove);
        }
        else
        {
            targetIndex = zoneId;
            
            OpenSidegates();
        }

        nextPoint = ElevatorEntries[targetIndex].transform;

        while (Vector2.Distance(Platform.position, nextPoint.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(Platform.position, nextPoint.position, Time.fixedDeltaTime * speed);
            rb.MovePosition(newPosition);

            rb.linearVelocity = (newPosition - previousPosition) / Time.fixedDeltaTime;
            previousPosition = newPosition;

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(nextPoint.position);
        rb.linearVelocity = Vector2.zero;
        index = targetIndex;
        
        CloseSidegates();
        
        elevatorActivated = false;
        buttonTimer = 0f;
        waitingForPlayerToLeaveZone = true;
    }
    
    private IEnumerator ResetButtonUpAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("ButtonUp", false);
        }
    }
    
    private void OpenSidegates()
    {
        if (sidegateAnimators != null && sidegateAnimators.Length > 0)
        {
            foreach (Animator sidegateAnimator in sidegateAnimators)
            {
                if (sidegateAnimator != null)
                {
                    sidegateAnimator.SetBool("Open", true);
                    sidegateAnimator.SetBool("Close", false);
                    // Debug.Log($"Set Open=true, Close=false on sidegate: {sidegateAnimator.gameObject.name}");
                }
            }
        }
    }
    
    private void CloseSidegates()
    {
        // if (sidegateAnimators != null && sidegateAnimators.Length > 0)
        if (sidegateAnimators == null || sidegateAnimators.Length == 0)
		return;

        foreach (Animator sidegateAnimator in sidegateAnimators)
        {
	    if (sidegateAnimator != null)
	    {
	        sidegateAnimator.SetBool("Open", false);
	        sidegateAnimator.SetBool("Close", true);
	        // Debug.Log($"Set Open=false, Close=true on sidegate: {sidegateAnimator.gameObject.name}");
	    
	        StartCoroutine(ResetCloseBool(sidegateAnimator));
	    }
        }
    }
    
    /// <summary>
    /// Reset the Close bool to false after the closing animation has time to play
    /// </summary>
    private IEnumerator ResetCloseBool(Animator animator)
    {
        yield return new WaitForSeconds(0.3f); // Wait for animation to start

        if (animator != null)
            animator.SetBool("Close", false);
    }



    private void OnDrawGizmos()
    {
        if (ElevatorEntries == null || ElevatorEntries.Count < 2) return;

        Gizmos.color = Color.green;
        
	for (int i = 0; i < ElevatorEntries.Count; i++)
        {
            Vector3 current = ElevatorEntries[i].transform.position;
            Vector3 next = ElevatorEntries[(i + 1) % ElevatorEntries.Count].transform.position;
            Gizmos.DrawLine(current, next);
        }
    }
}
