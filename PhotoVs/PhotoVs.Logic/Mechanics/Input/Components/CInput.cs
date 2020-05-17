using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.Input.Components
{
    public class CInput : IComponent
    {
        public GameInput Input;

        public CInput(GameInput input)
        {
            Input = input;
        }
    }
}