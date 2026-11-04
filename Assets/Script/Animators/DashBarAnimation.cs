using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DashBarAnimation : MonoBehaviour
{
    const float MinimumClampedValue = 0f;

    [Component] private SpriteRenderer _dashBarImage;
    private PlayerMovementController _playerMovementController;
    private float _dashBarLocalSizeXAxis;
    private float _dashBarCooldownTimeInSeconds = 1;
    private float _dashBarDurationTimeInSeconds = 0.3f;

    private void Start() 
    {
        // No TryGetComponentInParent exists, manual null check required.
        _playerMovementController = transform.GetComponentInParent<PlayerMovementController>();

        if (_playerMovementController.IsUnityNull())
            throw new MissingComponentException($"PlayerMovementController component not found in parent of DashBar");

        _dashBarLocalSizeXAxis = transform.localScale.x;
        _dashBarCooldownTimeInSeconds = _playerMovementController.DashCooldown;
        _dashBarDurationTimeInSeconds = _playerMovementController.DashDuration;
    }

    public void IncreaseDashBar(float timeElapsedInSeconds)
    {
        float timeElapsedToLocalScaleRatio = _dashBarLocalSizeXAxis / _dashBarCooldownTimeInSeconds * timeElapsedInSeconds;
        float clampedTimeElapsed = Mathf.Clamp(timeElapsedToLocalScaleRatio, MinimumClampedValue, _dashBarLocalSizeXAxis);

        transform.localScale = new Vector3(clampedTimeElapsed, transform.localScale.y, transform.localScale.z);
    }
    
    public void DecreaseDashBar(float timeElapsedInSeconds)
    {
        float timeElapsedToLocalScaleRatio = _dashBarLocalSizeXAxis - (_dashBarLocalSizeXAxis / _dashBarDurationTimeInSeconds * timeElapsedInSeconds);
        float clampedTimeElapsed = Mathf.Clamp(timeElapsedToLocalScaleRatio, MinimumClampedValue, _dashBarLocalSizeXAxis);

        transform.localScale = new Vector3(clampedTimeElapsed, transform.localScale.y, transform.localScale.z);
    }

    public void HideCooldownBar() =>
        _dashBarImage.enabled = false;

    public void ShowCooldownBar() =>
        _dashBarImage.enabled = true;
    private void Update()
    {
        if (transform.localScale.x.Equals(_dashBarLocalSizeXAxis))
        {
            _dashBarImage.color = Color.green;
            return;
        }

        _dashBarImage.color = Color.red;
    }
}