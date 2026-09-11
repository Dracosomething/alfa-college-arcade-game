public struct RelativeParallaxPosition
{
    private const int ParallaxResetValue = 1;

    private float x;
    private float y;
    private float parallaxEffectX;
    private float parallaxEffectY;
    
    public float X => CalculateRelativePositionOfGivenCoordinate(x, parallaxEffectX);

    public float Y => CalculateRelativePositionOfGivenCoordinate(y, parallaxEffectY);

    public RelativeParallaxPosition(float x, float parallaxEffectX, float y, float parallaxEffectY)
    {
        this.x = x;
        this.y = y;
        this.parallaxEffectX = parallaxEffectX;
        this.parallaxEffectY = parallaxEffectY;
    }
    
    private float CalculateRelativePositionOfGivenCoordinate(float coordinate, float parallaxEffectOfCoordinate) =>
        coordinate * (ParallaxResetValue - parallaxEffectOfCoordinate);
}