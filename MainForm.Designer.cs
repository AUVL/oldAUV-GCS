
namespace AUV_GCS
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.conect_button = new System.Windows.Forms.ToolStripButton();
            this.toolStripConnectionControl = new MissionPlanner.Controls.ToolStripConnectionControl();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.BackgroundImage = global::AUV_GCS.Properties.Resources.bgdark;
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.conect_button,
            this.toolStripConnectionControl});
            resources.ApplyResources(this.MainMenu, "MainMenu");
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Stretch = false;
            // 
            // conect_button
            // 
            this.conect_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.conect_button.ForeColor = System.Drawing.Color.White;
            this.conect_button.Image = global::AUV_GCS.Properties.Resources.light_connect_icon;
            resources.ApplyResources(this.conect_button, "conect_button");
            this.conect_button.Name = "conect_button";
            this.conect_button.Click += new System.EventHandler(this.conect_button_Click);
            // 
            // toolStripConnectionControl
            // 
            this.toolStripConnectionControl.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripConnectionControl.BackgroundImage = global::AUV_GCS.Properties.Resources.bgdark;
            this.toolStripConnectionControl.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripConnectionControl.Name = "toolStripConnectionControl";
            resources.ApplyResources(this.toolStripConnectionControl, "toolStripConnectionControl");
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainForm";
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MenuStrip MainMenu;
        public System.Windows.Forms.ToolStripButton conect_button;
        private MissionPlanner.Controls.ToolStripConnectionControl toolStripConnectionControl;
    }
}