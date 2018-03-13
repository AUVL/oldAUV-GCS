
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
            this.connect_button = new System.Windows.Forms.ToolStripButton();
            this.toolStripConnectionControl = new MissionPlanner.Controls.ToolStripConnectionControl();
            this.Main_splitContainer = new System.Windows.Forms.SplitContainer();
            this.Map_splitContainer = new System.Windows.Forms.SplitContainer();
            this.MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Main_splitContainer)).BeginInit();
            this.Main_splitContainer.Panel2.SuspendLayout();
            this.Main_splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Map_splitContainer)).BeginInit();
            this.Map_splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.BackgroundImage = global::AUV_GCS.Properties.Resources.bgdark;
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connect_button,
            this.toolStripConnectionControl});
            resources.ApplyResources(this.MainMenu, "MainMenu");
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Stretch = false;
            // 
            // connect_button
            // 
            this.connect_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.connect_button.ForeColor = System.Drawing.Color.White;
            this.connect_button.Image = global::AUV_GCS.Properties.Resources.light_connect_icon;
            resources.ApplyResources(this.connect_button, "connect_button");
            this.connect_button.Name = "connect_button";
            this.connect_button.Click += new System.EventHandler(this.conect_button_Click);
            // 
            // toolStripConnectionControl
            // 
            this.toolStripConnectionControl.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripConnectionControl.BackgroundImage = global::AUV_GCS.Properties.Resources.bgdark;
            this.toolStripConnectionControl.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripConnectionControl.Name = "toolStripConnectionControl";
            resources.ApplyResources(this.toolStripConnectionControl, "toolStripConnectionControl");
            // 
            // Main_splitContainer
            // 
            resources.ApplyResources(this.Main_splitContainer, "Main_splitContainer");
            this.Main_splitContainer.Name = "Main_splitContainer";
            // 
            // Main_splitContainer.Panel2
            // 
            this.Main_splitContainer.Panel2.Controls.Add(this.Map_splitContainer);
            // 
            // Map_splitContainer
            // 
            resources.ApplyResources(this.Map_splitContainer, "Map_splitContainer");
            this.Map_splitContainer.Name = "Map_splitContainer";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.Main_splitContainer);
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainForm";
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.Main_splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Main_splitContainer)).EndInit();
            this.Main_splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Map_splitContainer)).EndInit();
            this.Map_splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MenuStrip MainMenu;
        public System.Windows.Forms.ToolStripButton connect_button;
        private MissionPlanner.Controls.ToolStripConnectionControl toolStripConnectionControl;
        private System.Windows.Forms.SplitContainer Main_splitContainer;
        private System.Windows.Forms.SplitContainer Map_splitContainer;
    }
}