using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Logic.Transforms;
using PhotoVs.Logic.WorldZoning;
using PhotoVs.Models;
using PhotoVs.Models.ECS;
using PhotoVs.Utils;

namespace PhotoVs.Logic.Collision
{
    public class SCollisionResolution : IUpdateableSystem
    {
        private readonly Events _gameEvents;
        private readonly SMapBoundaryGeneration _mapBoundary;

        public SCollisionResolution(Events gameEvents, SMapBoundaryGeneration mapBoundary)
        {
            _gameEvents = gameEvents;
            _mapBoundary = mapBoundary;
        }

        public int Priority { get; set; } = -1;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CCollisionBound), typeof(CPosition) };

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            var stationaryList = new GameObjectCollection(); //_mapBoundary.GetCollisions());
            var movingList = new GameObjectCollection();

            stationaryList.AddRange(_mapBoundary.GetCollisions());

            foreach (var entity in entities)
                if (entity.Components.Has<CSolid>())
                    stationaryList.Add(entity);
                else
                    movingList.Add(entity);

            foreach (var moving in movingList)
                Move(moving, stationaryList, gameTime);
            movingList.ForEach(ProcessVelocityIntents);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void Move(IGameObject moving, GameObjectCollection stationaryEntities, GameTime gameTime)
        {
            var minimumTranslations = new List<Vector2>();
            var positionA = moving.Components.Get<CPosition>();
            var collisionBoundA = moving.Components.Get<CCollisionBound>();

            Vector2 baseVelocity, velocity;
            var compA = new RectangleF(positionA.Position.X + collisionBoundA.InflatedBounds.Left,
                positionA.Position.Y + collisionBoundA.InflatedBounds.Top,
                collisionBoundA.InflatedBounds.Width,
                collisionBoundA.InflatedBounds.Height);

            // next, check if A is actually moving and possesses a velocity
            if (moving.Components.TryGet(out CVelocity velocityA) && velocityA.Velocity != Vector2.Zero)
            {
                velocity = velocityA.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                baseVelocity = velocity;
            }
            else
            {
                return;
            }

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

                var result = CollisionResult.Simulate(positionA.Position, positionB.Position, collisionBoundA.Bounds,
                    collisionBoundB.Bounds, collisionBoundA.Points, collisionBoundB.Points, collisionBoundA.Edges,
                    collisionBoundB.Edges, collisionBoundA.Center, collisionBoundB.Center, velocity);

                if (result.WillIntersect)
                    minimumTranslations.Add(result.MinimumTranslation);

                if (!result.AreIntersecting)
                    continue;

                _gameEvents.RaiseOnCollision(moving, stationary);
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

            velocityA.VelocityIntent.Add(velocity - baseVelocity); // * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private static void ProcessVelocityIntents(IGameObject entity)
        {
            if (!entity.Components.TryGet(out CVelocity velocity) ||
                !entity.Components.TryGet(out CPosition position))
                return;

            if (velocity.VelocityIntent.Count == 0)
                return;

            position.Position += new Vector2(velocity.VelocityIntent.Average(x => x.X),
                velocity.VelocityIntent.Average(y => y.Y));

            velocity.VelocityIntent.Clear();
        }
    }
}