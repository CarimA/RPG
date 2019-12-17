using PhotoVs.ECS.Components;

namespace PhotoVs.CommonGameLogic.Input
{
    public class CInput : IComponent
    {
        public GameInput.Input Input;

        public CInput(GameInput.Input input)
        {
            Input = input;
        }
    }
}
