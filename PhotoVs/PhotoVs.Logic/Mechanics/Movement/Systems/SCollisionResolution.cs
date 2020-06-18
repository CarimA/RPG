using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Events;
using PhotoVs.Logic.Events;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Mechanics.World.Components;
using PhotoVs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using PhotoVs.Engine.Events.EventArgs;

namespace PhotoVs.Logic.Mechanics.Movement.Systems
{
    public class SCollisionResolution : IUpdateableSystem
    {
        private Overworld _overworld;
        private SCamera _camera;
        private readonly EventQueue _gameEvents;

        public SCollisionResolution(Overworld overworld, SCamera camera, EventQueue gameEvents)
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

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            var stationaryList = _overworld.GetMap().GetCollisions(_camera);
            var extraStationaryList = new GameObjectCollection();
            var movingList = new GameObjectCollection();

            foreach (var entity in entities)
                if (entity.Components.Has<CSolid>())
                    extraStationaryList.Add(entity);
                else
                    movingList.Add(entity);

            extraStationaryList.AddRange(stationaryList);

            foreach (var moving in movingList)
                Move(moving, extraStationaryList, gameTime);
            movingList.ForEach(ProcessVelocityIntents);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void Move(IGameObject moving, IGameObjectCollection stationaryEntities, GameTime gameTime)
        {
            var minimumTranslations = new List<Vector2>();
            var positionA = moving.Components.Get<CPosition>();
            var collisionBoundA = moving.Components.Get<CCollisionBound>();

            // check that A is actually moving
            if (positionA.DeltaPosition == Vector2.Zero)
            {
                return;
            }

            var compA = new RectangleF(positionA.Position.X + collisionBoundA.InflatedBounds.Left,
                positionA.Position.Y + collisionBoundA.InflatedBounds.Top,
                collisionBoundA.InflatedBounds.Width,
                collisionBoundA.InflatedBounds.Height);

            var velocity = positionA.DeltaPosition;

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

                _gameEvents.Notify(EventType.COLLISION, new InteractEventArgs(this, moving, stationary));
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

            positionA.VelocityIntent.Add(velocity); // * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private static void ProcessVelocityIntents(IGameObject entity)
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