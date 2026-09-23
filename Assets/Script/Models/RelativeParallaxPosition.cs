public struct RelativeParallaxPosition
{
    private const int ParallaxResetValue = 1;

    private float _x;
    private float _y;
    private float _parallaxEffectX;
    private float _parallaxEffectY;
    
    public float X => CalculateRelativePositionOfGivenCoordinate(_x, _parallaxEffectX);

    public float Y => CalculateRelativePositionOfGivenCoordinate(_y, _parallaxEffectY);

    public RelativeParallaxPosition(float x, float parallaxEffectX, float y, float parallaxEffectY)
    {
        this._x = x;
        this._y = y;
        this._parallaxEffectX = parallaxEffectX;
        this._parallaxEffectY = parallaxEffectY;
    }
    
    private float CalculateRelativePositionOfGivenCoordinate(float coordinate, float parallaxEffectOfCoordinate) =>
        coordinate * (ParallaxResetValue - parallaxEffectOfCoordinate);
}