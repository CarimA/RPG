using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using PhotoVs.EditorSuite.GameData;
using PhotoVs.EditorSuite.GameData.Events;
using PhotoVs.EditorSuite.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace PhotoVs.EditorSuite.Panels
{
    public partial class EventEditor : DockContent
    {
        private const int NodeWidth = 200;
        private const int GridSize = 25;
        private NodeLabel _dragEndLabel;

        private NodeLabel _dragStartLabel;

        private Control _focusedNode;
        private readonly Graph _graph;
        private readonly Dictionary<Control, Node> _nodeReference;

        private readonly Dictionary<Node, Control> _panelReference;

        private readonly Project _project;

        private bool isDragDropping;
        private int offsetX;
        private int offsetY;

        public EventEditor(Project project, Graph graph)
        {
            InitializeComponent();

            _panelReference = new Dictionary<Node, Control>();
            _nodeReference = new Dictionary<Control, Node>();

            _project = project;
            _graph = graph;

            // set double buffering on the panel
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                pnlCanvas, new object[] {true});

            pnlCanvas.AutoScrollMargin = new Size(50, 50);

            pnlCanvas.Click += (sender, args) =>
            {
                _focusedNode = null;
                Refresh();
            };

            pnlCanvas.Paint += CanvasPaint;
            pnlCanvas.AutoSizeChanged += (sender, args) => Refresh();
            pnlCanvas.Scroll += (sender, args) => Refresh();

            pnlCanvas.AllowDrop = true;

            tsbFlowControl.MouseDown += (sender, args) =>
                pnlCanvas.DoDragDrop(typeof(FlowControlNode), DragDropEffects.Move);


            pnlCanvas.DragEnter += (sender, args) =>
                args.Effect = DragDropEffects.Move;
            pnlCanvas.DragDrop += (sender, args) =>
            {
                if (args.Data.GetData(args.Data.GetFormats()[0]) is Type type)
                {
                    var loc = pnlCanvas.PointToClient(new Point(args.X, args.Y));
                    AddNode(type, Snap(loc.X), Snap(loc.Y));
                }
            };

            if (_graph.Nodes.Count == 0) AddNode<StartNode>(50, 50);

            UpdateNodes();
        }

        private void Focus(Node node)
        {
            var control = _panelReference[node];
            _focusedNode = control;
            control.BringToFront();
        }

        private void AddNode(Type type, int x, int y)
        {
            var node = (Node) Activator.CreateInstance(type);
            node.X = x;
            node.Y = y;
            node.Assign();
            _graph.Nodes.Add(node);
            UpdateNodes();

            Focus(node);
        }

        private void AddNode<T>(int x, int y) where T : Node, new()
        {
            var node = new T {X = x, Y = y};
            node.Assign();
            _graph.Nodes.Add(node);
            UpdateNodes();

            Focus(node);
        }

        private void RemoveNode(Node node)
        {
            // disconnect it first
            foreach (var input in node.Inputs)
            foreach (var output in input.ConnectedFrom)
                output.ConnectedTo = null;

            foreach (var output in node.Outputs) output.ConnectedTo?.ConnectedFrom.Remove(output);

            _graph.Nodes.Remove(node);
            UpdateNodes();
        }

        private void UpdateNodes()
        {
            var updated = false;

            foreach (var node in _graph.Nodes)
            {
                if (_panelReference.ContainsKey(node))
                    continue;

                // there is something new in the graph, so it should be added
                // to the form

                var control = CreateNode(node);
                _panelReference.Add(node, control);
                _nodeReference.Add(control, node);
                pnlCanvas.Controls.Add(control);

                updated = true;
            }

            foreach (var node in _panelReference.Keys.ToList())
            {
                if (_graph.Nodes.Contains(node))
                    continue;

                // this thing was removed from the graph, so it should be removed
                // from the form.

                var control = _panelReference[node];
                _panelReference.Remove(node);
                _nodeReference.Remove(control);
                pnlCanvas.Controls.Remove(control);

                updated = true;
            }

            if (updated)
                Refresh();
        }

        private Control CreateNode(Node node)
        {
            var titleHeight = 30;
            var titlePadding = 4;
            var titleLabelHeight = 26;

            var nodeSize = 22;
            var nodePadding = 4;

            var totalHeight = titleHeight + titlePadding;

            var panel = new Panel
            {
                Location = new Point(node.X, node.Y),
                BackColor = Color.FromArgb(228, 231, 235),
                BorderStyle = BorderStyle.FixedSingle
            };

            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                panel, new object[] {true});

            var titlePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(NodeWidth, titleHeight),
                BackColor = node.HeaderColor
            };

            var titleLabel = new Label
            {
                Text = node.Name,
                AutoSize = false,
                Size = new Size(NodeWidth, titleLabelHeight),
                Location = new Point(titleHeight / 2 - titleLabelHeight / 2, titleHeight / 2 - titleLabelHeight / 2),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(FontFamily.GenericSansSerif, titleLabelHeight / 2, FontStyle.Bold, GraphicsUnit.Pixel),
                ForeColor = Color.Black
            };

            if (node.IsRemovable)
            {
                var remove = new Panel
                {
                    BackgroundImage = Resources.Close_red_16x,
                    BackgroundImageLayout = ImageLayout.Center,
                    Size = new Size(titleHeight, titleHeight),
                    Location = new Point(NodeWidth - titleHeight, 0)
                };

                remove.Click += (sender, args) => { RemoveNode(node); };

                titlePanel.Controls.Add(remove);
            }

            void OnMouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    isDragDropping = true;
                    var tr = panel.PointToScreen(e.Location);
                    offsetX = tr.X - panel.Location.X;
                    offsetY = tr.Y - panel.Location.Y;
                    Focus(node);
                }
            }

            void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    if (isDragDropping)
                    {
                        Refresh();
                        var tr = panel.PointToScreen(e.Location);
                        panel.Location = new Point(Math.Max(0, Snap(tr.X - offsetX)),
                            Math.Max(0, Snap(tr.Y - offsetY)));

                        node.X = panel.Location.X;
                        node.Y = panel.Location.Y;
                    }
            }

            void OnMouseUp(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    isDragDropping = false;

                    node.X = panel.Location.X;
                    node.Y = panel.Location.Y;
                    _project.Save(false, true);

                    CancelDragDrop();
                }
            }

            titlePanel.MouseDown += OnMouseDown;
            titlePanel.MouseMove += OnMouseMove;
            titlePanel.MouseUp += OnMouseUp;
            titleLabel.MouseDown += OnMouseDown;
            titleLabel.MouseMove += OnMouseMove;
            titleLabel.MouseUp += OnMouseUp;

            titlePanel.Controls.Add(titleLabel);
            panel.Controls.Add(titlePanel);

            var inputHeight = totalHeight;
            var outputHeight = totalHeight;

            for (var i = 0; i < node.Inputs.Count; i++)
            {
                var input = node.Inputs[i];

                if (input.CanConnect)
                {
                    // this is a label that can be dragged and dropped to/from
                    // to connect to other nodes

                    var connector = new Panel
                    {
                        Location = new Point(0, inputHeight),
                        Size = new Size(nodeSize, nodeSize),
                        BackColor = Color.FromArgb(154, 165, 177),
                        AllowDrop = true
                    };

                    var label = new Label
                    {
                        Text = input.Name,
                        AutoSize = false,
                        Location = new Point(nodeSize, inputHeight),
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    panel.Controls.Add(connector);
                    panel.Controls.Add(label);

                    connector.MouseDown += (sender, args) =>
                    {
                        if (args.Button == MouseButtons.Right)
                        {
                            foreach (var connection in input.ConnectedFrom) connection.ConnectedTo = null;

                            input.ConnectedFrom = new List<NodeOutputLabel>();
                            Refresh();
                            _project.Autosave();
                        }
                        else
                        {
                            _dragStartLabel = input;
                            connector.DoDragDrop(connector, DragDropEffects.Move);
                        }
                    };

                    connector.DragEnter += (sender, args) => { args.Effect = DragDropEffects.Move; };

                    connector.DragDrop += (sender, args) =>
                    {
                        _dragEndLabel = input;
                        Reconcile();
                    };
                }

                inputHeight += nodeSize + nodePadding;
            }

            for (var i = 0; i < node.Outputs.Count; i++)
            {
                var output = node.Outputs[i];

                var connector = new Panel
                {
                    Location = new Point(NodeWidth - nodeSize, outputHeight),
                    Size = new Size(nodeSize, nodeSize),
                    BackColor = Color.FromArgb(154, 165, 177),
                    AllowDrop = true
                };

                var label = new Label
                {
                    Text = output.Name,
                    AutoSize = false,
                    Location = new Point(nodeSize, outputHeight),
                    Size = new Size(NodeWidth - nodeSize * 2, nodeSize),
                    TextAlign = ContentAlignment.MiddleRight
                };

                panel.Controls.Add(connector);
                panel.Controls.Add(label);

                connector.MouseDown += (sender, args) =>
                {
                    if (args.Button == MouseButtons.Right)
                    {
                        if (output.ConnectedTo == null)
                            return;

                        output.ConnectedTo.ConnectedFrom.Remove(output);
                        output.ConnectedTo = null;
                        Refresh();
                        _project.Autosave();
                    }
                    else
                    {
                        _dragStartLabel = output;
                        connector.DoDragDrop(connector, DragDropEffects.Move);
                    }
                };

                connector.DragEnter += (sender, args) => { args.Effect = DragDropEffects.Move; };

                connector.DragDrop += (sender, args) =>
                {
                    _dragEndLabel = output;
                    Reconcile();
                };

                outputHeight += nodeSize + nodePadding;
            }

            if (inputHeight > outputHeight)
                totalHeight = inputHeight;
            else
                totalHeight = outputHeight;

            panel.Size = new Size(NodeWidth, Snap(totalHeight + GridSize));

            return panel;
        }

        private void Reconcile()
        {
            // if either are null, it got cancelled midway
            if (_dragStartLabel == null || _dragEndLabel == null)
            {
                CancelDragDrop();
                return;
            }

            switch (_dragStartLabel)
            {
                case NodeInputLabel startInput:
                    switch (_dragEndLabel)
                    {
                        case NodeInputLabel _:
                            CancelDragDrop();
                            return;
                        case NodeOutputLabel endOutput when startInput.Requires != endOutput.Provides:
                            CancelDragDrop();
                            return;
                    }

                    break;
                case NodeOutputLabel startOutput:
                    switch (_dragEndLabel)
                    {
                        case NodeOutputLabel _:
                            CancelDragDrop();
                            return;
                        case NodeInputLabel endInput when startOutput.Provides != endInput.Requires:
                            CancelDragDrop();
                            return;
                    }

                    break;
            }

            // they're valid! build a connection

            if (_dragStartLabel is NodeInputLabel dragStartInput)
                dragStartInput.ConnectedFrom.Add(_dragEndLabel as NodeOutputLabel);

            if (_dragEndLabel is NodeInputLabel dragEndInput)
                dragEndInput.ConnectedFrom.Add(_dragStartLabel as NodeOutputLabel);

            if (_dragStartLabel is NodeOutputLabel dragStartOutput)
                dragStartOutput.ConnectedTo = _dragEndLabel as NodeInputLabel;

            if (_dragEndLabel is NodeOutputLabel dragEndOutput)
                dragEndOutput.ConnectedTo = _dragStartLabel as NodeInputLabel;

            Refresh();
            _project.Autosave();
        }

        private void CancelDragDrop()
        {
            _dragStartLabel = null;
            _dragEndLabel = null;
        }

        private int Snap(int input)
        {
            return (int) Math.Round((double) (input / GridSize)) * GridSize;
        }

        public void Refresh()
        {
            pnlCanvas.Invalidate();
        }

        private void CanvasPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighSpeed;

            g.TranslateTransform(pnlCanvas.AutoScrollPosition.X, pnlCanvas.AutoScrollPosition.Y);

            var grid = new Pen(Color.FromArgb(50, 63, 75), 1);
            var lines = new Pen(Color.FromArgb(154, 165, 177), 6);
            var smallBorder = new Pen(Color.FromArgb(97, 110, 124), 4);
            var border = new Pen(Color.Yellow, 8);

            g.Clear(Color.FromArgb(31, 41, 51));

            for (var x = Snap(-pnlCanvas.AutoScrollPosition.X);
                x < Snap(-pnlCanvas.AutoScrollPosition.X + pnlCanvas.Width + GridSize);
                x += GridSize)
                g.DrawLine(grid, x, -pnlCanvas.AutoScrollPosition.Y, x,
                    -pnlCanvas.AutoScrollPosition.Y + pnlCanvas.Height);

            for (var y = Snap(-pnlCanvas.AutoScrollPosition.Y);
                y < Snap(-pnlCanvas.AutoScrollPosition.Y + pnlCanvas.Height + GridSize);
                y += GridSize)
                g.DrawLine(grid, -pnlCanvas.AutoScrollPosition.X, y, -pnlCanvas.AutoScrollPosition.X + pnlCanvas.Width,
                    y);

            foreach (var kvp in _panelReference)
            {
                var control = kvp.Value;

                g.DrawRectangle(
                    ReferenceEquals(control, _focusedNode)
                        ? border
                        : smallBorder,
                    new Rectangle(
                        new Point(control.Location.X - pnlCanvas.AutoScrollPosition.X,
                            control.Location.Y - pnlCanvas.AutoScrollPosition.Y),
                        control.Size));
            }

            foreach (var kvp in _panelReference)
            {
                var node = kvp.Key;
                var control = kvp.Value;

                // draw all outputs. inputs will implicitly be drawn.
                foreach (var output in node.Outputs)
                    if (output.ConnectedTo != null)
                    {
                        var connectedNode = output.ConnectedTo.Parent;

                        var n1Index = node.Outputs.IndexOf(output);
                        var n2Index = connectedNode.Inputs.IndexOf(output.ConnectedTo);

                        var n1Control = control;
                        var n2Control = _panelReference[connectedNode];

                        var p1Pos = new Point(n1Control.Location.X - pnlCanvas.AutoScrollPosition.X,
                            n1Control.Location.Y - pnlCanvas.AutoScrollPosition.Y);
                        var n1Pos = new Point(NodeWidth - 15, 34 + n1Index * 26);
                        var p2Pos = new Point(n2Control.Location.X - pnlCanvas.AutoScrollPosition.X,
                            n2Control.Location.Y - pnlCanvas.AutoScrollPosition.Y);
                        var n2Pos = new Point(15, 34 + n2Index * 26);

                        g.DrawBezier(lines,
                            new Point(
                                p1Pos.X + n1Pos.X + 10,
                                p1Pos.Y + n1Pos.Y + 10),
                            new Point(
                                p1Pos.X + n1Pos.X + 10 + 60,
                                p1Pos.Y + n1Pos.Y + 10),
                            new Point(
                                p2Pos.X + n2Pos.X - 10 - 60,
                                p2Pos.Y + n2Pos.Y + 10),
                            new Point(
                                p2Pos.X + n2Pos.X - 10,
                                p2Pos.Y + n2Pos.Y + 10)
                        );
                    }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var center = GetCenter();
            AddNode<FlowControlNode>(center.X, center.Y);
        }

        private Point GetCenter()
        {
            var x = -pnlCanvas.AutoScrollPosition.X + pnlCanvas.PreferredSize.Width / 2;
            var y = -pnlCanvas.AutoScrollPosition.Y + pnlCanvas.PreferredSize.Height / 2;
            return new Point(x, y);
        }
    }
}