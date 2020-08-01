using System;
using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData.Events
{
    public class NodeInputLabel : NodeLabel
    {
        public NodeInputLabel()
        {
            ConnectedFrom = new List<NodeOutputLabel>();
        }

        public Type Requires { get; set; }
        public List<NodeOutputLabel> ConnectedFrom { get; set; }
        public bool CanConnect { get; set; }
    }
}