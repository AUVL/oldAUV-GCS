namespace AUV_GCS.GCSViews
{
    partial class MainWorkspace
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.Function_tabControl = new System.Windows.Forms.TabControl();
            this.WayPoint_Tab = new System.Windows.Forms.TabPage();
            this.AutoGuide_Tab = new System.Windows.Forms.TabPage();
            this.panel1.SuspendLayout();
            this.Function_tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.Function_tabControl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(509, 634);
            this.panel1.TabIndex = 0;
            // 
            // Function_tabControl
            // 
            this.Function_tabControl.Controls.Add(this.WayPoint_Tab);
            this.Function_tabControl.Controls.Add(this.AutoGuide_Tab);
            this.Function_tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Function_tabControl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Function_tabControl.Location = new System.Drawing.Point(0, 0);
            this.Function_tabControl.Name = "Function_tabControl";
            this.Function_tabControl.SelectedIndex = 0;
            this.Function_tabControl.Size = new System.Drawing.Size(509, 634);
            this.Function_tabControl.TabIndex = 0;
            // 
            // WayPoint_Tab
            // 
            this.WayPoint_Tab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(39)))), ((int)(((byte)(40)))));
            this.WayPoint_Tab.Location = new System.Drawing.Point(4, 22);
            this.WayPoint_Tab.Name = "WayPoint_Tab";
            this.WayPoint_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.WayPoint_Tab.Size = new System.Drawing.Size(501, 608);
            this.WayPoint_Tab.TabIndex = 0;
            this.WayPoint_Tab.Text = "航點資料";
            // 
            // AutoGuide_Tab
            // 
            this.AutoGuide_Tab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(39)))), ((int)(((byte)(40)))));
            this.AutoGuide_Tab.Location = new System.Drawing.Point(4, 22);
            this.AutoGuide_Tab.Name = "AutoGuide_Tab";
            this.AutoGuide_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.AutoGuide_Tab.Size = new System.Drawing.Size(501, 608);
            this.AutoGuide_Tab.TabIndex = 1;
            this.AutoGuide_Tab.Text = "多機控制";
            // 
            // MainWorkspace
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(39)))), ((int)(((byte)(40)))));
            this.Controls.Add(this.panel1);
            this.Name = "MainWorkspace";
            this.Size = new System.Drawing.Size(509, 634);
            this.panel1.ResumeLayout(false);
            this.Function_tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl Function_tabControl;
        private System.Windows.Forms.TabPage WayPoint_Tab;
        private System.Windows.Forms.TabPage AutoGuide_Tab;
    }
}
