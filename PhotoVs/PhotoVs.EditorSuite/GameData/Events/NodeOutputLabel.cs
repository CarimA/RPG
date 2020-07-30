using System;

namespace PhotoVs.EditorSuite.GameData.Events
{
    public class NodeOutputLabel : NodeLabel
    {
        public Type Provides { get; set; }
        public NodeInputLabel ConnectedTo { get; set; }
    }
}