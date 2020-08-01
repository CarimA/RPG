using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PhotoVs.EditorSuite.Panels
{
    public partial class ProjectExplorer : DockContent
    {
        private readonly DockPanel _dockPanel;
        private TreeView _tvExplorer;
        private TreeView _tvFilterExplorer;
        public Project Project;

        public ProjectExplorer(DockPanel dockPanel)
        {
            InitializeComponent();
            _dockPanel = dockPanel;

            NewProject();

            txtSearch.Enter += (sender, args) =>
            {
                if (txtSearch.ForeColor == Color.Black)
                    return;
                txtSearch.Text = string.Empty;
                txtSearch.ForeColor = Color.Black;
            };

            txtSearch.Leave += (sender, args) =>
            {
                if (txtSearch.Text.Trim() == string.Empty)
                {
                    txtSearch.ForeColor = Color.Gray;
                    txtSearch.Text = "Search...";
                }
            };

            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = "Search...";
        }

        private void ResetExplorer()
        {
            if (_tvExplorer != null)
                pnlContainer.Controls.Remove(_tvExplorer);

            if (_tvFilterExplorer != null)
                pnlContainer.Controls.Remove(_tvFilterExplorer);

            _tvExplorer = new TreeView();
            _tvExplorer.Dock = DockStyle.Fill;

            _tvFilterExplorer = new TreeView();
            _tvFilterExplorer.Dock = DockStyle.Fill;
            _tvFilterExplorer.Visible = false;
            //_tvFilterExplorer.BringToFront();

            _tvExplorer.ItemHeight = 20;
            _tvFilterExplorer.ItemHeight = 20;

            pnlContainer.Controls.Add(_tvExplorer);
            pnlContainer.Controls.Add(_tvFilterExplorer);
        }

        public void NewProject()
        {
            Project = new Project(string.Empty);
            ParseTree();
        }

        public void SaveProject(bool autoSave = false)
        {
            Project?.Save(false, autoSave);
        }

        public void SaveProjectAs()
        {
            Project?.Save(true);
        }

        public void LoadProject()
        {
            var result = Project.Load();
            if (result != null)
            {
                Project = null;
                Project = result;
                ParseTree();
            }
        }

        public void ParseTree()
        {
            CloseDocuments();
            ResetExplorer();
            Project.AssignTree(_dockPanel, _tvExplorer, _tvFilterExplorer, bar, bar2);
        }

        private void CloseDocuments()
        {
            foreach (var document in _dockPanel.Documents.ToList())
                ((DockContent) document).Close();
        }

        private void newStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void newScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.AddScript(_tvExplorer);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            _tvExplorer.SelectedNode.BeginEdit();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Project.Duplicate(_tvExplorer.SelectedNode);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Project.Delete(_tvExplorer.SelectedNode);
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.AddFolder(_tvExplorer.SelectedNode);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Project.OpenContainingFolder();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text == string.Empty || txtSearch.ForeColor == Color.Gray)
            {
                _tvFilterExplorer.Visible = false;
                _tvExplorer.Visible = true;
                _tvFilterExplorer.Dock = DockStyle.None;
                _tvExplorer.Dock = DockStyle.Fill;
                return;
            }

            _tvFilterExplorer.ImageList = _tvExplorer.ImageList;

            _tvFilterExplorer.Nodes.Clear();
            _tvFilterExplorer.Visible = true;
            _tvExplorer.Visible = false;
            _tvExplorer.Dock = DockStyle.None;
            _tvFilterExplorer.Dock = DockStyle.Fill;

            foreach (var node in _tvExplorer.Nodes)
            {
                if (node is Project.DataTreeNode child)
                    Filter(child, txtSearch.Text);
            }

            if (_tvFilterExplorer.Nodes.Count == 0)
            {
                _tvFilterExplorer.Nodes.Add("No items found.");
            }
        }

        private void Filter(Project.DataTreeNode node, string search)
        {
            try
            {
                if (node.Data != null)
                {
                    if (Regex.IsMatch(node.Text, search, RegexOptions.IgnoreCase))
                    {
                        _tvFilterExplorer.Nodes.Add(new Project.DataTreeNode(node.Text, node.Data, node.IsMovable,
                            node.IsEditable)
                        {
                            ImageIndex = node.ImageIndex,
                            StateImageIndex = node.StateImageIndex,
                            SelectedImageIndex = node.SelectedImageIndex
                        });
                    }
                }

                foreach (var child in node.Nodes)
                    Filter((Project.DataTreeNode) child, search);
            }
            catch
            {
            }
        }

        private void newEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.AddEvent(_tvExplorer);
        }
    }
}