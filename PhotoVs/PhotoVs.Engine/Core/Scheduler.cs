using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public class Scheduler : IScheduler
    {
        private readonly IKernel _kernel;
        private readonly List<IHasAfterDraw> _afterDraws;
        private readonly List<IHasAfterUpdate> _afterUpdates;
        private readonly List<IHasBeforeDraw> _beforeDraws;
        private readonly List<IHasBeforeUpdate> _beforeUpdates;
        private readonly List<IHasDraw> _draws;
        private readonly List<IStartup> _startups;
        private readonly List<IHasUpdate> _updates;

        public Scheduler(IKernel kernel)
        {
            _kernel = kernel;
            _startups = new List<IStartup>();
            _beforeUpdates = new List<IHasBeforeUpdate>();
            _updates = new List<IHasUpdate>();
            _afterUpdates = new List<IHasAfterUpdate>();
            _beforeDraws = new List<IHasBeforeDraw>();
            _draws = new List<IHasDraw>();
            _afterDraws = new List<IHasAfterDraw>();

            kernel.OnBind += OnKernelBind;
            kernel.OnConstruct += Sort;
        }

        public void Start()
        {
            foreach (var item in _startups)
                item.Start(_kernel.Bindings);
        }

        public void BeforeUpdate(GameTime gameTime)
        {
            foreach (var item in _beforeUpdates)
                if (item.BeforeUpdateEnabled)
                    item.BeforeUpdate(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in _updates)
                if (item.UpdateEnabled)
                    item.Update(gameTime);
        }

        public void AfterUpdate(GameTime gameTime)
        {
            foreach (var item in _afterUpdates)
                if (item.AfterUpdateEnabled)
                    item.AfterUpdate(gameTime);
        }

        public void BeforeDraw(GameTime gameTime)
        {
            foreach (var item in _beforeDraws)
                if (item.BeforeDrawEnabled)
                    item.BeforeDraw(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var item in _draws)
                if (item.DrawEnabled)
                    item.Draw(gameTime);
        }

        public void AfterDraw(GameTime gameTime)
        {
            foreach (var item in _afterDraws)
                if (item.AfterDrawEnabled)
                    item.AfterDraw(gameTime);
        }

        private void Sort()
        {
            _beforeUpdates.Sort((a, b) => a.BeforeUpdatePriority.CompareTo(b.BeforeUpdatePriority));
            _updates.Sort((a, b) => a.UpdatePriority.CompareTo(b.UpdatePriority));
            _afterUpdates.Sort((a, b) => a.AfterUpdatePriority.CompareTo(b.AfterUpdatePriority));
            _beforeDraws.Sort((a, b) => a.BeforeDrawPriority.CompareTo(b.BeforeDrawPriority));
            _draws.Sort((a, b) => a.DrawPriority.CompareTo(b.DrawPriority));
            _afterDraws.Sort((a, b) => a.AfterDrawPriority.CompareTo(b.AfterDrawPriority));
        }

        private void OnKernelBind(object obj)
        {
            if (obj is IHasBeforeUpdate beforeUpdate)
                _beforeUpdates.Add(beforeUpdate);

            if (obj is IHasUpdate update)
                _updates.Add(update);

            if (obj is IHasAfterUpdate afterUpdate)
                _afterUpdates.Add(afterUpdate);

            if (obj is IHasBeforeDraw beforeDraw)
                _beforeDraws.Add(beforeDraw);

            if (obj is IHasDraw draw)
                _draws.Add(draw);

            if (obj is IHasAfterDraw afterDraw)
                _afterDraws.Add(afterDraw);

            if (obj is IStartup startup)
                _startups.Add(startup);
        }
    }
}