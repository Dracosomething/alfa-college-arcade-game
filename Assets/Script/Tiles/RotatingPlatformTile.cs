using UnityEngine;

public class RotatingPlatformTile : MonoBehaviour
{
    private const float TargetAngle = 180f;
    private const float NoRotation = 0f;
    private const float FullRotation = 360f;
    
    [SerializeField] private bool _isRotatingClockwise;
    [SerializeField] private bool _isAutoRotating;
    [SerializeField] private float _rotationSpeed;
    private bool _isRotating;

    private void Update()
    {
        Rotate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(Constants.PlayerGameObjectName))
            return;

        StartRotating();
    }
    
    private void StartRotating() =>
        _isRotating = true;
    
    private void StopRotating() =>
        _isRotating = false;

    private void Rotate()
    {
        if (_isRotating || _rotationSpeed <= NoRotation)
        {
            StopRotating();
            return;
        }

        float startZAngle = transform.eulerAngles.z;
        float rotatedDegrees = NoRotation;
        var direction = _isRotatingClockwise ? RotationDirection.Clockwise : RotationDirection.CounterClockwise;

        while (rotatedDegrees < TargetAngle - Constants.FloatingPointErrorOffset || _isAutoRotating)
        {
            float rotationStep = _rotationSpeed * Time.deltaTime;
            float remainingRotation = TargetAngle - rotatedDegrees;
            // We want to make sure that we won't go over TargetAngle.
            var nextStep = Mathf.Min(rotationStep, remainingRotation);
            
            transform.Rotate(xAngle: NoRotation, yAngle: NoRotation, zAngle: nextStep);

            if (!_isAutoRotating)
                rotatedDegrees += nextStep;
        }
        
        // We do these calculations to have the platform snap back to it's original rotation.
        float finalZAngle = startZAngle + ((int)direction * TargetAngle);
        // The final modulus should make sure that negative numbers become positive.
        finalZAngle = (finalZAngle % FullRotation + FullRotation) % FullRotation;
        Vector3 currentEulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentEulerAngles.x, currentEulerAngles.y, finalZAngle);

        StopRotating();
    }
}
