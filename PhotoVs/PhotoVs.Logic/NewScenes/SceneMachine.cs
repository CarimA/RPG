using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.StateMachine;
using PhotoVs.Logic.Mechanics.Input.Components;

namespace PhotoVs.Logic.NewScenes
{
    public class SceneMachine : StackStateMachine<Scene>, IHasUpdate, IHasDraw
    {
        private readonly IGameState _gameState;

        private readonly CInputState _inputState;
        // todo: move before/after update/draw into methods from interface calls

        private readonly IRenderer _renderer;

        public SceneMachine(IGameState gameState, IRenderer renderer)
        {
            _gameState = gameState;
            _inputState = _gameState.Player.Components.Get<CInputState>();
            _renderer = renderer;
        }

        public int DrawPriority { get; set; } = 0;
        public bool DrawEnabled { get; set; } = true;

        public void Draw(GameTime gameTime)
        {
            Process<IDrawableSystem>(
                BeforeDraw,
                Draw,
                AfterDraw,
                SceneDraw,
                gameTime);
        }

        public int UpdatePriority { get; set; } = 0;
        public bool UpdateEnabled { get; set; } = true;

        public void Update(GameTime gameTime)
        {
            Process<IUpdateableSystem>(
                BeforeUpdate,
                Update,
                AfterUpdate,
                SceneUpdate,
                gameTime);
        }

        private void Process<T>(
            Action<IEnumerable<T>, GameTime> beforeDo,
            Action<IEnumerable<T>, GameObjectList, GameTime> nowDo,
            Action<IEnumerable<T>, GameTime> afterDo,
            Action<Scene, GameTime> sceneAction,
            GameTime gameTime) where T : ISystem
        {
            var scenes = CurrentScenes();
            var work = _gameState.Systems.ToList();

            foreach (var scene in scenes) work.AddRange(scene.Systems);

            work.Sort(SortByPriority);

            var relevantWork = work.OfType<T>();

            beforeDo(relevantWork, gameTime);
            nowDo(relevantWork, _gameState.GameObjects, gameTime);
            afterDo(relevantWork, gameTime);

            foreach (var scene in scenes) sceneAction(scene, gameTime);
        }

        private void BeforeUpdate(IEnumerable<IUpdateableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems)
                system.BeforeUpdate(gameTime);
        }

        private void Update(IEnumerable<IUpdateableSystem> systems, GameObjectList entities, GameTime gameTime)
        {
            foreach (var system in systems)
                system.Update(gameTime,
                    system.Requires.Length == 0
                        ? entities
                        : entities.All(system.Requires));
        }

        private void AfterUpdate(IEnumerable<IUpdateableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems)
                system.AfterUpdate(gameTime);
        }

        private void SceneUpdate(Scene scene, GameTime gameTime)
        {
            scene.ProcessInput(gameTime, _inputState);
            scene.Update(gameTime);
        }

        private void BeforeDraw(IEnumerable<IDrawableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems)
                system.BeforeDraw(gameTime);
        }

        private void Draw(IEnumerable<IDrawableSystem> systems, GameObjectList entities, GameTime gameTime)
        {
            foreach (var system in systems)
                system.Draw(gameTime,
                    system.Requires.Length == 0
                        ? entities
                        : entities.All(system.Requires));
        }

        private void AfterDraw(IEnumerable<IDrawableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems)
                system.AfterDraw(gameTime);
        }

        private void SceneDraw(Scene scene, GameTime gameTime)
        {
            scene.Draw(gameTime, _renderer.GetUIOrigin());
        }

        private bool IsActive(ISystem system)
        {
            return system.Active;
        }

        public IEnumerable<Scene> CurrentScenes()
        {
            var takeWhile = States.TakeWhile(IsStateNotBlocking);
            var take = States.Take(takeWhile.Count() + 1);
            var reverse = take.Reverse();

            return reverse;
            //return States.Take(States.TakeWhile(IsStateNotBlocking).Count() + 1).Reverse();
        }

        private int SortByPriority(ISystem a, ISystem b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        private bool IsStateNotBlocking(Scene state)
        {
            return !state.IsBlocking;
        }
    }
}