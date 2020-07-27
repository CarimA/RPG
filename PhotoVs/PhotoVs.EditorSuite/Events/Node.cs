using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.EditorSuite.Events
{
    public sealed class StartNode : Node
    {
        public override string Name => "Start";

        public StartNode() : base()
        {
            Outputs.Add(new NodeOutputFunctionLabel());
        }
    }

    public abstract class Node
    {
        public virtual string Name { get; }
        public int X { get; set; }
        public int Y { get; set; }

        public List<NodeInputLabel> Inputs { get; }
        public List<NodeOutputLabel> Outputs { get; }

        protected Node()
        {
            Inputs = new List<NodeInputLabel>();
            Outputs = new List<NodeOutputLabel>();
        }
    }

    public class NodeOutputDataLabel : NodeOutputLabel { }
    public class NodeOutputFunctionLabel : NodeOutputLabel { }

    public class NodeInputDataLabel : NodeInputLabel { }
    public class NodeInputFunctionLabel : NodeInputLabel { }

    public abstract class NodeInputLabel
    {
        public List<Node> ConnectedFrom { get; set; }

        protected NodeInputLabel()
        {
            ConnectedFrom = new List<Node>();
        }
    }

    public abstract class NodeOutputLabel
    {
        public Node ConnectedTo { get; set; }
    }
}
