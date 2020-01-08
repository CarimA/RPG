using PhotoVs.Models.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic.UI
{
    public class CUIActionEvent : IComponent
    {
        // todo: do we need IGameObject/ISystem senders?
        public Action OnAction;
    }

    public class CUICheck : IComponent
    {
        public bool Checked;
    }

    public class CUIObject : IComponent
    {
        public UIObject Object;
    }

    public class CUIObjectUp : CUIObject { }
    public class CUIObjectDown : CUIObject { }
    public class CUIObjectLeft : CUIObject { }
    public class CUIObjectRight : CUIObject { }
}
