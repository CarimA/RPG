namespace PhotoVs.Logic.Camera
{
    public class ScreenShake
    {
        public float Duration;
        public float Intensity;
        public float TotalDuration;

        public ScreenShake(float intensity, float duration)
        {
            Intensity = intensity;
            TotalDuration = duration;
            Duration = duration;
        }

        public float GetIntensity()
        {
            return Intensity * (Duration / TotalDuration);
        }
    }
}