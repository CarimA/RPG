using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PhotoVs.EditorSuite.Events;
using WeifenLuo.WinFormsUI.Docking;

namespace PhotoVs.EditorSuite.Panels
{
    public partial class EventEditor : DockContent
    {
        private Project _project;
        private Graph _graph;

        private Dictionary<Node, Control> _panelReference;
        private Dictionary<Control, Node> _nodeReference;

        public EventEditor(Project project, Graph graph)
        {
            InitializeComponent();

            _panelReference = new Dictionary<Node, Control>();
            _nodeReference = new Dictionary<Control, Node>();

            _project = project;
            _graph = graph;

            UpdateNodes();
        }

        private void UpdateNodes()
        {
            // go through the nodes
            // check if there's anything new to be added from the graph, and add it here

            // check if there's anything removed from the graph and remove it here
            
        }
    }
}
