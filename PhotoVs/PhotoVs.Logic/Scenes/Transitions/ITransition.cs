using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Logic.Scenes.Transitions
{
    public interface ITransition
    {
        bool IsFinished { get; }

        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        bool ShouldSwitch();
    }
}
