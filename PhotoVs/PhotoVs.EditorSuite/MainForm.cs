using System.Windows.Forms;
using PhotoVs.EditorSuite.Panels;
using PhotoVs.EditorSuite.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace PhotoVs.EditorSuite
{
    public partial class MainForm : Form
    {
        private Timer _autosaveTimer;
        private DockPanel _dockPanel;

        private readonly ProjectExplorer _projectExplorer;

        public MainForm()
        {
            InitializeComponent();
            IsMdiContainer = true;
            Text = "Photeus Editor";

            CreateDockPanel();
            CreateMenu();

            _projectExplorer = new ProjectExplorer(_dockPanel)
            {
                HideOnClose = true,
                CloseButton = false,
                CloseButtonVisible = false,
                DockAreas = DockAreas.DockLeft | DockAreas.DockRight
            };
            _projectExplorer.Show(_dockPanel, DockState.DockLeft);

            CreateAutoSaveTimer();
        }

        private void CreateAutoSaveTimer()
        {
            _autosaveTimer = new Timer
            {
                Interval = 60 * 1000
            };
            _autosaveTimer.Tick += (sender, args) => _projectExplorer?.SaveProject(true);
            _autosaveTimer.Start();
        }

        private void CreateDockPanel()
        {
            Controls.Add(_dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill,
                Theme = new VS2015BlueTheme()
            });
        }

        private void CreateMenu()
        {
            MenuStrip menu;
            Controls.Add(menu = new MenuStrip
            {
                Dock = DockStyle.Top
            });

            ToolStripMenuItem file, edit, project;

            menu.Items.Add(file = new ToolStripMenuItem("File"));

            ToolStripMenuItem newProject, loadProject, saveProject, exit, autosave;

            file.DropDownItems.Add(newProject = new ToolStripMenuItem("New Project")
            {
                ShortcutKeys = Keys.Control | Keys.Shift | Keys.N,
                Image = Resources.NewFile_16x
            });
            newProject.Click += (sender, args) => _projectExplorer?.NewProject();

            file.DropDownItems.Add(saveProject = new ToolStripMenuItem("Save Project As")
            {
                ShortcutKeys = Keys.Control | Keys.Shift | Keys.S,
                Image = Resources.SaveAs_16x
            });
            saveProject.Click += (sender, args) => _projectExplorer?.SaveProjectAs();

            file.DropDownItems.Add(loadProject = new ToolStripMenuItem("Load Project")
            {
                ShortcutKeys = Keys.Control | Keys.Shift | Keys.O
            });
            loadProject.Click += (sender, args) => _projectExplorer.LoadProject();

            file.DropDownItems.Add(new ToolStripSeparator());

            file.DropDownItems.Add(autosave = new ToolStripMenuItem("Autosave"));
            autosave.Click += (sender, args) =>
            {
                if (_autosaveTimer.Enabled)
                    _autosaveTimer.Stop();
                else
                    _autosaveTimer.Start();
            };
            file.Click += (sender, args) => { autosave.Checked = _autosaveTimer.Enabled; };

            file.DropDownItems.Add(new ToolStripSeparator());

            file.DropDownItems.Add(exit = new ToolStripMenuItem("Exit")
            {
                ShortcutKeys = Keys.Alt | Keys.F4,
                Image = Resources.Close_red_16x
            });

            menu.Items.Add(edit = new ToolStripMenuItem("Edit"));

            ToolStripMenuItem undo, redo, cut, copy, paste;

            edit.DropDownItems.Add(undo = new ToolStripMenuItem("Undo")
            {
                ShortcutKeys = Keys.Control | Keys.Z,
                Image = Resources.Undo_16x
            });

            edit.DropDownItems.Add(redo = new ToolStripMenuItem("Redo")
            {
                ShortcutKeys = Keys.Control | Keys.Y,
                Image = Resources.Redo_16x
            });

            edit.DropDownItems.Add(new ToolStripSeparator());

            edit.DropDownItems.Add(cut = new ToolStripMenuItem("Cut")
            {
                ShortcutKeys = Keys.Control | Keys.X,
                Image = Resources.Cut_16x
            });

            edit.DropDownItems.Add(copy = new ToolStripMenuItem("Copy")
            {
                ShortcutKeys = Keys.Control | Keys.C,
                Image = Resources.Copy_16x
            });

            edit.DropDownItems.Add(paste = new ToolStripMenuItem("Paste")
            {
                ShortcutKeys = Keys.Control | Keys.V,
                Image = Resources.Paste_16x
            });


            menu.Items.Add(project = new ToolStripMenuItem("Project"));

            ToolStripMenuItem connectToGame, build;

            project.DropDownItems.Add(connectToGame = new ToolStripMenuItem("Connect to Game")
            {
                ShortcutKeys = Keys.F5,
                Image = Resources.Run_16x
            });

            project.DropDownItems.Add(build = new ToolStripMenuItem("Build")
            {
                ShortcutKeys = Keys.F6,
                Image = Resources.BuildSolution_16x
            });
        }
    }
}