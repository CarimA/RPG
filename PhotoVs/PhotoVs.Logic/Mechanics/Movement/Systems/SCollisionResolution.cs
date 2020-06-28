using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Events;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Mechanics.World.Components;
using PhotoVs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.Movement.Systems
{
    public class SCollisionResolution : IUpdateableSystem
    {
        private Overworld _overworld;
        private SCamera _camera;
        private readonly Engine.Events.EventQueue<GameEvents> _gameEvents;

        public SCollisionResolution(Overworld overworld, SCamera camera, Engine.Events.EventQueue<GameEvents> gameEvents)
        {
            _overworld = overworld;
            _camera = camera;
            _gameEvents = gameEvents;
        }

        public int Priority { get; set; } = -1;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CCollisionBound), typeof(CPosition) };

        public void BeforeUpdate(GameTime gameTime)
        {

        }

        public void Update(GameTime gameTime, GameObjectList entities)
        {
            var stationaryList = _overworld.GetMap().GetCollisions(_camera);
            var extraStationaryList = new GameObjectList();
            var movingList = new GameObjectList();

            foreach (var entity in entities)
                if (entity.Components.Has<CSolid>())
                    extraStationaryList.Add(entity);
                else
                    if (entity.Components.Has<CInputState>())
                        movingList.Add(entity);

            extraStationaryList.AddRange(stationaryList);

            foreach (var moving in movingList)
                Move(moving, extraStationaryList, gameTime);

            movingList.ForEach(ProcessVelocityIntents);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void Move(GameObject moving, GameObjectList stationaryEntities, GameTime gameTime)
        {
            var input = moving.Components.Get<CInputState>();
            var minimumTranslations = new List<Vector2>();
            var position = moving.Components.Get<CPosition>();
            var collisionBound = moving.Components.Get<CCollisionBound>();

            var velocity = input.LeftAxis;
            var isRunning = input.ActionDown(InputActions.Run);

            if (moving is Player player)
            {
                velocity *= player.CurrentSpeed(isRunning);
                if (!player.CanMove && (player.Components.Has<CKeyboard>() || player.Components.Has<CController>()))
                    velocity *= 0;
            }
            else
            {
                velocity *= isRunning ? 200 : 100;
            }

            velocity *= gameTime.GetElapsedSeconds();

            // check that A is actually moving
            if (velocity == Vector2.Zero)
                return;

            var compA = new RectangleF(position.Position.X + collisionBound.InflatedBounds.Left,
                position.Position.Y + collisionBound.InflatedBounds.Top,
                collisionBound.InflatedBounds.Width,
                collisionBound.InflatedBounds.Height);

            foreach (var stationary in stationaryEntities)
            {
                var positionB = stationary.Components.Get<CPosition>();
                var collisionBoundB = stationary.Components.Get<CCollisionBound>();

                var compB = new RectangleF(positionB.Position.X + collisionBoundB.InflatedBounds.Left,
                    positionB.Position.Y + collisionBoundB.InflatedBounds.Top,
                    collisionBoundB.InflatedBounds.Width,
                    collisionBoundB.InflatedBounds.Height);

                // first, check if they're even near each other
                if (!compA.Intersects(compB))
                    continue;

                var result = CollisionResult.Simulate(moving, stationary, velocity);

                if (result.WillIntersect)
                    minimumTranslations.Add(result.MinimumTranslation);

                if (!result.AreIntersecting)
                    continue;

                _gameEvents.Notify(GameEvents.Collision, new InteractEventArgs(this, moving, stationary));
            }

            if (minimumTranslations.Count > 0)
            {
                if (minimumTranslations.Count == 1)
                {
                    velocity += minimumTranslations.First();
                }
                else
                {
                    var average = CollisionResult.GetFurthestFromOrigin(minimumTranslations);
                    velocity += average;
                }
            }

            position.VelocityIntent.Add(velocity); // * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private static void ProcessVelocityIntents(GameObject entity)
        {
            if (!entity.Components.TryGet(out CPosition position))
                return;

            if (position.VelocityIntent.Count == 0)
                return;

            position.Position += new Vector2(position.VelocityIntent.Average(x => x.X),
                position.VelocityIntent.Average(y => y.Y));

            position.VelocityIntent.Clear();
        }
    }
}