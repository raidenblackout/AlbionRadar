namespace AlbionDataHandlers.Entities;

public class InterpolatableEntity
{
    public float FromX { get; set; }
    public float FromY { get; set; }
    public float ToX { get; set; }
    public float ToY { get; set; }

    public float CurrentLerpedX { get; private set; }
    public float CurrentLerpedY { get; private set; }

    /// <summary>
    /// Calculates the interpolated position for the current frame using Lerp.
    /// </summary>
    public void Interpolate(float t)
    {
        CurrentLerpedX = FromX + (ToX - FromX) * t;
        CurrentLerpedY = FromY + (ToY - FromY) * t;
    }
}
