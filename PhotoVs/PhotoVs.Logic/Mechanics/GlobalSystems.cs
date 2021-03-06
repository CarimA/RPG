﻿using PhotoVs.Engine.ECS;

namespace PhotoVs.Logic.Mechanics
{
    public class GlobalSystems
    {
        public GlobalSystems(Stage stage, Camera camera, Input input, Animation animation)
        {
            stage.RegisterGlobalSystem(camera.UpdateTransform, int.MinValue);

            stage.RegisterGlobalSystem(input.ResetInputSchemesIfDisconnected, int.MinValue);
            stage.RegisterGlobalSystem(input.SetLastState, int.MinValue);
            stage.RegisterGlobalSystem(input.CheckPriority, int.MinValue);
            stage.RegisterGlobalSystem(input.ProcessKeyboards, int.MinValue + 1);
            stage.RegisterGlobalSystem(input.ProcessControllers, int.MinValue + 2);
            stage.RegisterGlobalSystem(input.RaiseEvents, int.MinValue + 3);

            stage.RegisterGlobalSystem(animation.UpdateDirection, int.MinValue);
            stage.RegisterGlobalSystem(animation.UpdateAnimations, int.MinValue + 1);
        }
    }
}
