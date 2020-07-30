using System;

namespace PhotoVs.EditorSuite.GameData.Events
{
    public abstract class NodeLabel
    {
        public Node Parent { get; set; }
        public string Name { get; set; }
        public bool Removable { get; set; }
        public Type DataType { get; set; }
        public object Data { get; set; }
    }
}