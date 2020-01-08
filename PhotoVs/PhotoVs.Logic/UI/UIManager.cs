using Microsoft.Xna.Framework;
using PhotoVs.Models.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic.UI
{
    public class UIManager
    {
        public UIObject Root { get; private set; }

        private readonly UIObjectCollection _rootCollection;
        private readonly ISystemCollection _UISystems;

        public UIManager(ISystemCollection uiSystems)
        {
            Root = new UIObject();
            _UISystems = uiSystems;
            _rootCollection.Add(Root);
        }

        public void Update(GameTime gameTime)
        {
            var systems = _UISystems
                .Where(system => system.Active)
                .OfType<IUpdateableSystem>();

            foreach (var system in systems)
            {
                system.BeforeUpdate(gameTime);
            }

            Update(systems, _rootCollection, gameTime);

            foreach (var system in systems)
            {
                system.AfterUpdate(gameTime);
            }
        }

        private void Update(IEnumerable<IUpdateableSystem> systems, UIObjectCollection uiObjects, GameTime gameTime)
        {
            foreach (var system in systems)
            {
                system.Update(gameTime, system.Requires.Length == 0
                    ? uiObjects
                    : uiObjects.All(system.Requires));
            }

            foreach (var uiObject in uiObjects)
            {
                var ui = uiObject as UIObject;
                if (ui.Children.Count > 0)
                {
                    Update(systems, ui.Children, gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            var systems = _UISystems
                .Where(system => system.Active)
                .OfType<IDrawableSystem>();

            foreach (var system in systems)
            {
                system.BeforeDraw(gameTime);
            }

            Draw(systems, _rootCollection, gameTime);

            foreach (var system in systems)
            {
                system.AfterDraw(gameTime);
            }
        }

        private void Draw(IEnumerable<IDrawableSystem> systems, UIObjectCollection uiObjects, GameTime gameTime)
        {
            foreach (var system in systems)
            {
                system.Draw(gameTime, system.Requires.Length == 0
                    ? uiObjects
                    : uiObjects.All(system.Requires));
            }

            foreach (var uiObject in uiObjects)
            {
                var ui = uiObject as UIObject;
                if (ui.Children.Count > 0)
                {
                    Draw(systems, ui.Children, gameTime);
                }
            }
        }
    }
}
