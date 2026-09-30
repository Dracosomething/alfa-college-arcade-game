using System.Collections;
using UnityEngine;

public class FallingPlatformTile : MonoBehaviour
{
    [SerializeField] private float _fallDelay;
    [SerializeField] private float _resetDelay;
    private Vector3 initialPosition;
    private bool hasFallen = false;
    private Rigidbody2D _rigidbody;
    private Animation _animation;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        _collider = GetComponent<Collider2D>();
        _animation = GetComponent<Animation>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasFallen)
	    return;

        hasFallen = true;

        StartCoroutine(MakeFall());
        Invoke("ResetTile", _resetDelay);
    }

    private IEnumerator MakeFall()
    {
        yield return new WaitForSeconds(_fallDelay);

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (_animation != null) _animation.Play();

        if (_collider != null)
	    _collider.enabled = false;
    }

    private void ResetTile()
    {
        hasFallen = false;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        transform.position = initialPosition;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _animation.Stop();
        _spriteRenderer.enabled = true;
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        _collider.enabled = true;
    }
}
