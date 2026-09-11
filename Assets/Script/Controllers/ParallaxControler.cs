using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxController : MonoBehaviour
{
    [Header("Wobble Pattern Settings")]
    public bool UseWobbleX = false;
    public bool UseWobbleY = false;
    public float WobbleOscillationSpeedX = 1f;
    public float WobbleOscillationSpeedY = 1f;
    public float WobbleXDirectionAmplitude = 2f;
    public float WobbleYDirectionAmplitude = 2f;
    
    [Header("Auto Scroll Settings")]
    public bool UseAutoScrollX = false;
    public float AutoScrollSpeedX = 0f;
    
    private float _textureLength;
    private float _textureHeight;
    private float _startposX;
    private float _startposY;
    private float _autoScrollOffset = 0f;
    private float _wobbleTime = 0f;
    public float ParallaxEffectX;
    public float ParallaxEffectY;
    public GameObject Camera;

    private void Start()
    {
        var parallaxControllerSpriteRenderer = GetComponent<SpriteRenderer>();
        
        _startposX = transform.position.x;
        _startposY = transform.position.y;
        _textureLength = parallaxControllerSpriteRenderer.bounds.size.x;
        _textureHeight = parallaxControllerSpriteRenderer.bounds.size.y;
    }

    private void Update()
    {
        Vector3 cameraPosition = Camera.transform.position;
        Vector3 currentParallaxPosition = transform.position;
        
        RelativeParallaxPosition relativePosition = new(
            cameraPosition.x, ParallaxEffectX,
            cameraPosition.y, ParallaxEffectY);

        Vector2 distanceVector = new(
             (cameraPosition.y * ParallaxEffectY), 
             (cameraPosition.x * ParallaxEffectX));
        
        if (UseWobbleX)
        {
            UpdateWobbleTimeUsingOscillationSpeed(WobbleOscillationSpeedX);
            
            float wobbleOffsetX = Mathf.Sin(_wobbleTime) * WobbleXDirectionAmplitude;
            
            currentParallaxPosition.x = CalculateNewPositionForCoordinate(_startposX, distanceVector.x, 
                wobbleOffsetX);
        }
        else if (UseAutoScrollX)
        {
            _autoScrollOffset += AutoScrollSpeedX * Time.deltaTime;

            currentParallaxPosition.x = CalculateNewPositionForCoordinate(_startposX, distanceVector.x,
                _autoScrollOffset);
        }
        else
            currentParallaxPosition.x = CalculateNewPositionForCoordinate(_startposX, distanceVector.x);
        
        ScrollBackgroundOnAxis(Axis2D.X, relativePosition);
        
        if (UseWobbleY)
        {   // We only need to update the wobble time if we haven't already done it with the x wobble.
            if (!UseWobbleX)
               UpdateWobbleTimeUsingOscillationSpeed(WobbleOscillationSpeedY); 
            
            float wobbleOffsetY = Mathf.Sin(_wobbleTime * WobbleOscillationSpeedY / WobbleOscillationSpeedX) * WobbleYDirectionAmplitude;

            currentParallaxPosition.y = CalculateNewPositionForCoordinate(_startposY, distanceVector.y,
                wobbleOffsetY);
        }
        else
            currentParallaxPosition.y = CalculateNewPositionForCoordinate(_startposY, distanceVector.y);
        
        ScrollBackgroundOnAxis(Axis2D.Y, relativePosition);
        
        transform.position = currentParallaxPosition;
    }

    private void ScrollBackgroundOnAxis(Axis2D axis, RelativeParallaxPosition relativeParallaxPosition)
    {
        switch (axis)
        {
            case Axis2D.X:
                _startposX = GetNewStartPosValueOnAxis(_startposX, _textureLength, relativeParallaxPosition.X);
                break;
            case Axis2D.Y:
                _startposY = GetNewStartPosValueOnAxis(_startposY, _textureHeight, relativeParallaxPosition.Y);
                break;
        }
    }

    private float GetNewStartPosValueOnAxis(float originalStartPosOnAxis, float lengthOnAxis, float relativePositionOnAxis)
    {
        if (relativePositionOnAxis > originalStartPosOnAxis + lengthOnAxis)
            return originalStartPosOnAxis + lengthOnAxis;
        if (relativePositionOnAxis < originalStartPosOnAxis - lengthOnAxis)
            return originalStartPosOnAxis - lengthOnAxis;

        return 0f;
    }

    private float CalculateNewPositionForCoordinate(float startPoseCoordinate, float distanceCoordinate,
        float offset = 0) =>
        _startposY + distanceCoordinate + offset;
    
    private void UpdateWobbleTimeUsingOscillationSpeed(float oscillationSpeed) =>
        _wobbleTime += oscillationSpeed * Time.deltaTime;
}   