using System;

namespace PhotoVs.Logic.Scenes.SceneComponents
{
    public class MenuItem
    {
        public string Text;
        public Action OnClick;

        public MenuItem(string text, Action onClick)
        {
            Text = text;
            OnClick = onClick;
        }
    }
}
