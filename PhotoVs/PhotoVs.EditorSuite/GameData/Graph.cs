using System.Collections.Generic;
using PhotoVs.EditorSuite.GameData.Events;

namespace PhotoVs.EditorSuite.GameData
{
    public class Graph
    {
        public List<Node> Nodes { get; }

        public Graph()
        {
            Nodes = new List<Node>();
        }
    }
}
