// In AlbionDataHandlers/Entities/InterpolatableEntity.cs
namespace AlbionDataHandlers.Entities
{
    /// <summary>
    /// Base class for smooth "chasing" interpolation. The entity's current
    /// position constantly chases its target position every frame.
    /// </summary>
    public abstract class InterpolatableEntity
    {
        // The final, smoothed position that is rendered on screen.
        public float CurrentLerpedX { get; protected set; }
        public float CurrentLerpedY { get; protected set; }

        // The destination position received from network updates.
        protected float TargetX;
        protected float TargetY;

        private bool _isInitialized = false;
        private const float LERP_SPEED = 7.5f;

        public void SetTargetPosition(float x, float y)
        {
            if (!_isInitialized)
            {
                CurrentLerpedX = x;
                CurrentLerpedY = y;
                _isInitialized = true;
            }

            TargetX = x;
            TargetY = y;
        }

        public void Interpolate(float deltaTimeSeconds)
        {
            float factor = LERP_SPEED * deltaTimeSeconds;

            CurrentLerpedX = Lerp(CurrentLerpedX, TargetX, factor);
            CurrentLerpedY = Lerp(CurrentLerpedY, TargetY, factor);
        }

        private static float Lerp(float start, float end, float amount)
        {
            // Use our own custom Clamp method instead of Math.Clamp
            amount = Clamp(amount, 0.0f, 1.0f);
            return start + (end - start) * amount;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// This is a polyfill for Math.Clamp for older .NET versions.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum allowable value.</param>
        /// <param name="max">The maximum allowable value.</param>
        /// <returns>The clamped value.</returns>
        private static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}