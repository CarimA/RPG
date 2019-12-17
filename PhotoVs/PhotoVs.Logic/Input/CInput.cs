using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Input
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