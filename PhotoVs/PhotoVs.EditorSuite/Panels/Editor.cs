using WeifenLuo.WinFormsUI.Docking;

namespace PhotoVs.EditorSuite.Panels
{
    public abstract class Editor<T> : DockContent
    {
        public Project Project { get; set; }
        public T Instance { get; set;  }

        protected Editor()
        {
        }
    }
}