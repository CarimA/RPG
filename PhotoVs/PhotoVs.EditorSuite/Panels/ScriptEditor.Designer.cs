namespace PhotoVs.EditorSuite.Panels
{
    partial class ScriptEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.codeEdit = new ScintillaNET.Scintilla();
            this.save = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // codeEdit
            // 
            this.codeEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEdit.Location = new System.Drawing.Point(0, 0);
            this.codeEdit.Name = "codeEdit";
            this.codeEdit.Size = new System.Drawing.Size(800, 450);
            this.codeEdit.TabIndex = 0;
            this.codeEdit.TabWidth = 2;
            this.codeEdit.UseTabs = true;
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.codeEdit);
            this.Name = "ScriptEditor";
            this.Text = "ScriptEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla codeEdit;
        private System.Windows.Forms.Timer save;
    }
}