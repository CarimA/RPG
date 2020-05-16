using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;

namespace PhotoVs.Logic.Battles.TurnOrder
{
    public class TurnLoop
    {
        /*private int _turn;
        private List<ITurnActor> _turnActors;
        private Queue<ITurnActor> _turnOrder;

        public void Update(GameTime gameTime)
        {
            foreach (var actor in _turnActors)
                actor.Update(gameTime, actor == _turnOrder.Peek());

            if (_turnOrder.Count == 0)
            {
                _turn++;
                return;
            }

            var currentActor = _turnOrder.Peek();

            if (currentActor.HasFinished)
            {
                currentActor.AfterAction();
                _turnOrder.Dequeue();
                currentActor = _turnOrder.Peek();
                currentActor.BeforeAction();
            }

            currentActor.DuringAction(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            //foreach (var actor in _turnActors)
            //    actor.Draw(gameTime, actor == _currentActor);
        }

        private void BeforeAction(IGameObject gameObject)
        {

        }

        private void Action(IGameObject gameObject)
        {

        }*/
    }
}
