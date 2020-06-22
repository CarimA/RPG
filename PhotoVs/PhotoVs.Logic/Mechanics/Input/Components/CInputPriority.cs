using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.Input.Components
{
    public class CInputPriority : IComponent
    {
        public InputPriority InputPriority { get; set; }

        public CInputPriority()
        {
            InputPriority = InputPriority.GamePad;
        }
    }
}
