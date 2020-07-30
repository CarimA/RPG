using System.Collections.Generic;
using System.Drawing;

namespace PhotoVs.EditorSuite.GameData.Events
{
    public abstract class Node
    {
        public abstract string Name { get; }
        public int X { get; set; }
        public int Y { get; set; }

        public virtual List<NodeInputLabel> Inputs { get; }
        public virtual List<NodeOutputLabel> Outputs { get; }

        public abstract Color HeaderColor { get; }
        public virtual bool IsRemovable { get; } = true;
        public abstract bool CanAddInputs { get; }
        public abstract bool CanAddOutputs { get; }

        protected Node()
        {
            Inputs = new List<NodeInputLabel>();
            Outputs = new List<NodeOutputLabel>();
        }

        public virtual void AddInput()
        {
        }

        public virtual void AddOutput()
        {
        }

        public virtual void Assign()
        {

        }
    }
}
