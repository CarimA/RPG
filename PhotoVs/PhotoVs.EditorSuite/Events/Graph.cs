using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.EditorSuite.Events
{
    public class Graph
    {
        // instead of doing it here, create a dictionary of <node, control>
        // in the actual editor and handle stuff there...?
        public List<Node> Nodes { get; }

        public Graph()
        {
            Nodes = new List<Node>();
            Nodes.Add(new StartNode()
            {
                X = 50,
                Y = 50
            });
        }
    }
}
