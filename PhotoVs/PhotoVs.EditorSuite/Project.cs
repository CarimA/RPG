using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DamienG.Security.Cryptography;
using Newtonsoft.Json;
using PhotoVs.EditorSuite.Events;
using PhotoVs.EditorSuite.Panels;
using WeifenLuo.WinFormsUI.Docking;

namespace PhotoVs.EditorSuite
{
    public class Project
    {
        // the backing project is hidden so that invalidation can be abstracted
        private DataTreeNode _rootNode;
        private string _projectLocation;
        private bool _hasMadeChanges;

        private Control _bar;
        DataTreeNode hovered = null;
        private Control _bar2;

        private DockPanel _dockPanel;
        private Dictionary<object, DockContent> _openPanels;

        public Project(string path)
        {
            _projectLocation = path;
            _hasMadeChanges = true;

            _rootNode = new DataTreeNode("Project", null, false, false);

            var properties = new DataTreeNode("Properties", new GameProperties(), false, false);
            _rootNode.Nodes.Add(properties);

            _openPanels = new Dictionary<object, DockContent>();
        }

        private static string Crc32(string input)
        {
            var crc32 = new Crc32();
            var hash = crc32.ComputeHash(Encoding.UTF8.GetBytes(input));

            return hash.Aggregate(
                string.Empty, 
                (current, b) => current + b.ToString("x2").ToLowerInvariant());
        }

        public void Save(bool saveAs = false, bool autoSave = false)
        {
            // do not save if there's no need to
            if (!autoSave && (!saveAs && !_hasMadeChanges))
                return;

            if (autoSave && _projectLocation.Trim() == string.Empty)
                return;

            if (saveAs || _projectLocation.Trim() == string.Empty)
            {
                var sfd = new SaveFileDialog
                {
                    Filter = "Photeus Project|*.pvp", 
                    Title = "Save Project"
                };

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                _projectLocation = sfd.FileName;
            }

            var json = JsonConvert.SerializeObject(_rootNode, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            // check if crc doesn't match
            if (Crc32(json) == Crc32(File.ReadAllText(_projectLocation)))
                return;

            var count = 0;
            while (File.Exists(_projectLocation + $".{count}.bak"))
            {
                count++;
            }
            File.Copy(_projectLocation, _projectLocation + $".{count}.bak");
            File.WriteAllText(_projectLocation, json);
            _hasMadeChanges = false;
        }

        public static Project Load()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "Photeus Project|*.pvp",
                Title = "Open Project"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return null;

            var fileLocation = ofd.FileName;
            var json = File.ReadAllText(fileLocation);
            var obj = JsonConvert.DeserializeObject<DataTreeNode>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var result = new Project(fileLocation)
            {
                _rootNode = obj,
                _hasMadeChanges = false
            };

            return result;
        }

        public void Build()
        {
            // here is where we check references to data: if something was deleted and no longer exists
            // it should cancel and warn.
            // build by cross-referencing from events: do not add anything that goes unused
        }

        public void AssignTree(DockPanel panel, TreeView treeView, TreeView filter, Control bar, Control bar2)
        {
            _dockPanel = panel;
            _bar = bar;
            _bar2 = bar2;

            MoveBar(-10000, 0);

            _dockPanel.ActivePaneChanged += (sender, args) =>
            {
                Save(false, true);
            };

            treeView.Nodes.Clear();

            treeView.LabelEdit = true;

            treeView.BeforeLabelEdit += (sender, args) =>
            {
                var node = args.Node;

                if (node == null)
                    return;

                if (node is DataTreeNode dataNode)
                {
                    if (!dataNode.IsEditable)
                    {
                        args.CancelEdit = true;
                        return;
                    }
                }
            };

            treeView.AfterLabelEdit += (sender, args) =>
            {
                if (args.Label == null || args.Label.Trim() == string.Empty)
                {
                    args.CancelEdit = true;
                    return;
                }

                var node = args.Node;

                if (node == null)
                    return;

                if (node is DataTreeNode dataNode)
                {
                    if (!dataNode.IsEditable)
                    {
                        args.CancelEdit = true;
                        return;
                    }

                    dataNode.Text = args.Label;

                    if (dataNode.Data != null)
                    {
                        if (_openPanels.ContainsKey(dataNode.Data))
                        {
                            _openPanels[dataNode.Data].Text = dataNode.Text;
                        }
                    }

                    Save(false, true);
                }
            };

            treeView.DoubleClick += (sender, args) =>
            {
                var node = treeView.SelectedNode;

                if (node == null)
                    return;

                if (node is DataTreeNode dataNode)
                {
                    Open(dataNode);
                }
            };

            filter.DoubleClick += (sender, args) =>
            {
                var node = filter.SelectedNode;

                if (node == null)
                    return;

                if (node is DataTreeNode dataNode)
                {
                    Open(dataNode);
                }
            };

            treeView.AllowDrop = true;

            treeView.ItemDrag += (sender, args) =>
            {
                if (((DataTreeNode)args.Item).IsMovable)
                    treeView.DoDragDrop(args.Item, DragDropEffects.Move);
            };

            treeView.DragEnter += (sender, args) =>
            {
                args.Effect = DragDropEffects.Move;
            };

            treeView.DragOver += (sender, args) =>
            {
                var target = treeView.PointToClient(new Point(args.X, args.Y));
                var targetNode = (DataTreeNode)treeView.GetNodeAt(target);
                var draggedNode = (DataTreeNode)args.Data.GetData(typeof(DataTreeNode));
                var buffer = 6;

                if (targetNode == null)
                    return;

                var targetNodePosition = targetNode.Bounds;

                if (hovered != null && hovered != targetNode)
                {
                    hovered.BackColor = Color.White;
                    hovered.ForeColor = Color.Black;
                }

                if (targetNode.Parent == null
                    || (targetNode.Parent != null && targetNode.Data != null && targetNode.Data.GetType() == typeof(GameProperties)))
                {
                    MoveBar(-10000, 0);
                    return;
                }

                targetNode.BackColor = SystemColors.Highlight;
                targetNode.ForeColor = SystemColors.HighlightText;

                if (target.Y < targetNodePosition.Top + buffer)
                {
                    // move above
                    MoveBar(targetNodePosition.Top, targetNodePosition.Left);
                }
                else if (target.Y > targetNodePosition.Bottom - buffer)
                {
                    // move below
                    MoveBar(targetNodePosition.Bottom, targetNodePosition.Left);
                }
                else
                {
                    if (CanDropInto(targetNode, draggedNode))
                        MoveBar(targetNodePosition.Top, targetNodePosition.Left, true, treeView.ItemHeight);
                    else 
                        MoveBar(-10000, 0);
                }
                
                hovered = targetNode;
            };

            treeView.DragDrop += (sender, args) =>
            {
                var target = treeView.PointToClient(new Point(args.X, args.Y));
                var targetNode = (DataTreeNode) treeView.GetNodeAt(target);
                var draggedNode = (DataTreeNode) args.Data.GetData(typeof(DataTreeNode));

                if (targetNode == null)
                    return;

                var targetNodePosition = targetNode.Bounds;
                var buffer = 6;

                MoveBar(-10000, 0);

                if (hovered != null)
                {
                    hovered.BackColor = Color.White;
                    hovered.ForeColor = Color.Black;
                }

                if (targetNode != null)
                {
                    targetNode.BackColor = Color.White;
                    targetNode.ForeColor = Color.Black;
                }

                if (draggedNode != null)
                {
                    draggedNode.BackColor = Color.White;
                    draggedNode.ForeColor = Color.Black;
                }

                if (!(!draggedNode.Equals(targetNode) && targetNode != null))
                    return;

                if (!draggedNode.IsMovable)
                    return;

                if (target.Y < targetNodePosition.Top + buffer)
                {
                    // do not allow moving above properties
                    if (targetNode.Parent != null && targetNode.Data.GetType() == typeof(GameProperties))
                        return;

                    // move above
                    if (targetNode.Parent != null)
                    {
                        draggedNode.Remove();
                        targetNode.Parent.Nodes.Insert(targetNode.Index, draggedNode);
                    }
                }
                else if (target.Y > targetNodePosition.Bottom - buffer)
                {
                    // move below
                    if (targetNode.Parent != null)
                    {
                        draggedNode.Remove();
                        targetNode.Parent.Nodes.Insert(targetNode.Index + 1, draggedNode);
                    }
                }
                else
                {
                    if (!CanDropInto(targetNode, draggedNode))
                        return;

                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                    targetNode.Expand();
                }

                Save(false, true);
            };

            var imageList = new ImageList();
            imageList.Images.Add(Properties.Resources.SourceConrolFolder_16x);
            imageList.Images.Add(Properties.Resources.FolderClosed_16x);
            imageList.Images.Add(Properties.Resources.FolderOpened_16x);
            imageList.Images.Add(Properties.Resources.Settings_16x);
            imageList.Images.Add(Properties.Resources.Script_16x);
            imageList.Images.Add(Properties.Resources.Event_16x);

            treeView.ImageList = imageList;

            treeView.AfterExpand += UpdateIcon;
            treeView.AfterCollapse += UpdateIcon;

            treeView.Nodes.Add(_rootNode);
            _rootNode.Expand();

            SetIcons(_rootNode);
        }

        private void Open(DataTreeNode dataNode)
        {
            if (dataNode?.Data == null)
            {
                //dataNode.Toggle();
                return;
            }

            var data = dataNode.Data;

            if (_openPanels.ContainsKey(data))
            {
                _openPanels[data].Show();
                return;
            }

            if (data.GetType() == typeof(Script))
            {
                var scriptEditor = new ScriptEditor(this, (Script)data)
                {
                    HideOnClose = false,
                    CloseButton = true,
                    CloseButtonVisible = true,
                    DockAreas = DockAreas.Document,
                    Text = dataNode.Text
                };

                // todo: save on closing/tab changing
                scriptEditor.Closed += (sender, args) =>
                {
                    Save(false, true);
                    _openPanels.Remove(data);
                };

                scriptEditor.Show(_dockPanel, DockState.Document);
                _openPanels.Add(data, scriptEditor);
            }

            if (data.GetType() == typeof(Graph))
            {
                var eventEditor = new EventEditor(this, (Graph)data)
                {
                    HideOnClose = false,
                    CloseButton = true,
                    CloseButtonVisible = true,
                    DockAreas = DockAreas.Document,
                    Text = dataNode.Text
                };

                eventEditor.Closed += (sender, args) =>
                {
                    Save(false, true);
                    _openPanels.Remove(data);
                };

                eventEditor.Show(_dockPanel, DockState.Document);
                _openPanels.Add(data, eventEditor);
            }
        }

        private bool CanDropInto(DataTreeNode targetNode, DataTreeNode draggedNode)
        {
            // this node is not a folder
            if (targetNode.Data != null)
                return false;

            var canDrop = true;
            var parentNode = targetNode;
            while (canDrop && (parentNode != null))
            {
                // this node is a descendent
                canDrop = !Object.ReferenceEquals(draggedNode, parentNode);
                parentNode = (DataTreeNode)parentNode.Parent;
            }

            if (!canDrop)
                return false;

            return true;
        }

        private void MoveBar(int top, int left, bool overlapping = false, int itemHeight = 20)
        {
            _bar.BringToFront();
            _bar2.BringToFront();

            _bar.Size = new Size(_bar.Parent.Width, 1);
            _bar2.Size = new Size(_bar.Parent.Width, 1);
            _bar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _bar2.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            if (overlapping)
            {
                _bar.Location = new Point(left, top + 1);
                _bar2.Location = new Point(left, top + itemHeight + 1);
            }
            else
            {
                _bar.Location = new Point(left, top + 1);
                _bar2.Location = new Point(left, top + 1);
            }
        }

        private void UpdateIcon(object sender, TreeViewEventArgs e)
        {
            var data = e.Node;

            if (data == null)
                return;

            if (!(data is DataTreeNode dataNode))
                return;

            SetIcon(dataNode);
        }

        private void SetIcons(DataTreeNode node)
        {
            SetIcon(node);
            foreach (var child in node.Nodes)
            {
                SetIcons((DataTreeNode)child);
            }
        }

        private void SetIcon(DataTreeNode node)
        {
            if (node == null)
                return;

            var isExpanded = node.IsExpanded;

            if (node.Data == null)
            {
                if (node.Data == _rootNode)
                    SetIcon(node, 0);
                else
                    SetIcon(node, isExpanded ? 2 : 1);
            }
            else if (node.Data.GetType() == typeof(GameProperties))
            {
                SetIcon(node, 3);
            }
            else if (node.Data.GetType() == typeof(Script))
            {
                SetIcon(node, 4);
            }
            else if (node.Data.GetType() == typeof(Graph))
            {
                SetIcon(node, 5);
            }
        }

        private void SetIcon(DataTreeNode node, int index)
        {
            node.ImageIndex = index;
            node.StateImageIndex = index;
            node.SelectedImageIndex = index;
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class DataTreeNode : TreeNode
        {
            [JsonProperty]
            public List<DataTreeNode> Children
            {
                get
                {
                    var list = new List<DataTreeNode>();
                    foreach (var node in base.Nodes)
                        list.Add((DataTreeNode)node);

                    return list;
                }
                set
                {
                    foreach (var node in value)
                    {
                        base.Nodes.Add(node);
                    }
                }
            }

            [JsonProperty] public new string Text
            {
                get => base.Text;
                set => base.Text = value;
            }
            [JsonProperty] public object Data { get; set; }
            [JsonProperty] public bool IsMovable { get; set; }
            [JsonProperty] public bool IsEditable { get; set; }
            
            public DataTreeNode(string text, object data, bool isMovable = true, bool isEditable = true) : base(text)
            {
                Data = data;
                IsMovable = isMovable;
                IsEditable = isEditable;
            }
        }

        private void AddObject<T>(TreeNode node, string name) where T : new()
        {
            if (node == null)
                return;

            if (!(node is DataTreeNode dataNode))
                return;

            // folders will always have null data
            if (dataNode.Data != null)
            {
                // go one level above and add as a sibling
                AddObject<T>(node.Parent, name);
                return;
            }

            var text = $"New {name}";

            var newNode = new DataTreeNode(text, new T());
            dataNode.Nodes.Add(newNode);

            dataNode.Expand();

            Save(false, true);
            SetIcon(newNode);

            newNode.BeginEdit();
        }

        public void AddScript(TreeView treeView)
        {
            AddObject<Script>(treeView.SelectedNode, "Script");
        }

        public void AddEvent(TreeView treeView)
        {
            AddObject<Graph>(treeView.SelectedNode, "Event");
        }

        public void Duplicate(TreeNode node)
        {
            if (node == null)
                return;

            if (!(node is DataTreeNode dataNode))
                return;

            if (!dataNode.IsEditable)
                return;

            if (!dataNode.IsMovable)
                return;

            // to duplicate easily, we'll just serialize and deserialize the data
            var json = JsonConvert.SerializeObject(dataNode.Data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var copy = JsonConvert.DeserializeObject(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var copyNode = new DataTreeNode(dataNode.Text + " - Copy", copy);
            node.Parent.Nodes.Add(copyNode);

            dataNode.Expand();

            Save(false, true);
            SetIcon(copyNode);

            copyNode.BeginEdit();
        }

        public void Delete(TreeNode node)
        {
            if (node == null)
                return;

            if (!(node is DataTreeNode dataNode))
                return;

            if (!dataNode.IsEditable)
                return;

            if (!dataNode.IsMovable)
                return;

            // todo: [IMPORTANT] CHECK IF THE USER WANTS IT GONE FIRST

            if (dataNode.Data != null)
            {
                if (_openPanels.ContainsKey(dataNode.Data))
                {
                    _openPanels[dataNode.Data].Close();
                }
            }

            node.Parent.Nodes.Remove(node);
            Save(false, true);
        }

        public void AddFolder(TreeNode node)
        {
            if (node == null)
                return;

            if (!(node is DataTreeNode dataNode))
                return;

            // folders will always have null data
            if (dataNode.Data != null)
            {
                // go one level above and add as a sibling
                AddFolder(node.Parent);
                return;
            }

            var text = $"New Folder";

            var newNode = new DataTreeNode(text, null);
            dataNode.Nodes.Add(newNode);

            dataNode.Expand();

            Save(false, true);
            SetIcon(newNode);

            newNode.BeginEdit();
        }

        public void OpenContainingFolder()
        {
            if (_projectLocation.Trim() == string.Empty)
                return;

            Process.Start("explorer.exe", Path.GetDirectoryName(_projectLocation));
        }
    }
}
