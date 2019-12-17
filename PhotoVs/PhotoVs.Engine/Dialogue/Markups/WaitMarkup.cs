namespace PhotoVs.Engine.Dialogue.Markups
{
    public class WaitMarkup : IMarkup
    {
        public float WaitTime;

        public WaitMarkup() : this(0.5f)
        {
                
        }

        public WaitMarkup(float waitTime)
        {
            WaitTime = waitTime;
        }
    }
}
