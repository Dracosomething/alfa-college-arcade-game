using UnityEngine;

public class DashBarAnimation : MonoBehaviour
{
    [Component] private SpriteRenderer _dashBarImage;

    private void Awake() =>
        _dashBarImage.enabled = false;

    public void AnimateCooldownBar(float duration)
    {
        float decreaseRate = transform.localScale.x / duration;
    }

    public void HideCooldownBar() =>
        _dashBarImage.enabled = false;

    public void ShowCooldownBar() =>
        _dashBarImage.enabled = true;
}
