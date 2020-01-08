using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Models.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic.UI
{
    public class UIObject : GameObject
    {
        public UIObjectCollection Children { get; private set; }
        public UIObject ActiveChild { get; private set; }

        public UIObject()
        {
            Children = new UIObjectCollection();
        }
    }
}
