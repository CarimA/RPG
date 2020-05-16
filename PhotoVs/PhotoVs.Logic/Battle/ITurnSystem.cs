using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;

namespace PhotoVs.Logic.Battle
{
    public interface ITurnSystem : ISystem
    {
        void BeforeTurn(IGameObjectCollection allGameObjects);
        void AfterTurn(IGameObjectCollection allGameObjects);
        void Update(GameTime gameTime, IGameObjectCollection allGameObjects);
        void Draw(GameTime gameTime, IGameObjectCollection allGameObjects);
    }
}
