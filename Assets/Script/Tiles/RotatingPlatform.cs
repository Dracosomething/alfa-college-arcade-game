using System.Collections;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    private const float _targetAngle = 180f;
    [SerializeField] private bool _isRotatingClockwise;
    [SerializeField] private bool _isAutoRotating;
    [SerializeField] private float _rotationSpeed;
    private bool _isRotating;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
	
        if (!_isRotating)
            StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        if (_isRotating)
            yield break;

        if (_rotationSpeed <= 0f)
            yield break;

        _isRotating = true;

        float startZAngle = transform.eulerAngles.z;
        float rotatedAmount = 0f;
        float direction = _isRotatingClockwise ? -1f : 1f;

        while (rotatedAmount < _targetAngle - 0.0001f)
        {
            float step = _rotationSpeed * Time.deltaTime;
            float remainingAngle = _targetAngle - rotatedAmount;
            float biggestPossibleStepWithoutGoingOver = Mathf.Min(step, remainingAngle);

            while (_isAutoRotating)
	    {
                transform.Rotate(0f, 0f, direction * biggestPossibleStepWithoutGoingOver);
                yield return null;
            }

            transform.Rotate(0f, 0f, direction * biggestPossibleStepWithoutGoingOver);
            rotatedAmount += biggestPossibleStepWithoutGoingOver;

            yield return null;
        }

        float finalZAngle = startZAngle + direction * _targetAngle;
        finalZAngle = (finalZAngle % 360f + 360f) % 360f;
        Vector3 currentEulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentEulerAngles.x, currentEulerAngles.y, finalZAngle);

        _isRotating = false;
    }
}
