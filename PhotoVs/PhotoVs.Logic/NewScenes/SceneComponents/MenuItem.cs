using System;

namespace PhotoVs.Logic.NewScenes.SceneComponents
{
    public class MenuItem
    {
        public Action OnClick;
        public string Text;

        public MenuItem(string text, Action onClick)
        {
            Text = text;
            OnClick = onClick;
        }
    }
}