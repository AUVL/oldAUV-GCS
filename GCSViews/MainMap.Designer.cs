namespace AUV_GCS.GCSViews
{
    partial class MainMap
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMap));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.drawPolygonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPologonPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPolygonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gMapControl1 = new MissionPlanner.Controls.myGMAP();
            this.coords1 = new MissionPlanner.Controls.Coords();
            this.Label1 = new System.Windows.Forms.Label();
            this.TXT_homelat = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.TXT_homelng = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TXT_homealt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.LinkLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawPolygonToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(156, 26);
            // 
            // drawPolygonToolStripMenuItem
            // 
            this.drawPolygonToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPologonPointToolStripMenuItem,
            this.clearPolygonToolStripMenuItem});
            this.drawPolygonToolStripMenuItem.Name = "drawPolygonToolStripMenuItem";
            this.drawPolygonToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.drawPolygonToolStripMenuItem.Text = "Draw Polygon";
            // 
            // addPologonPointToolStripMenuItem
            // 
            this.addPologonPointToolStripMenuItem.Name = "addPologonPointToolStripMenuItem";
            this.addPologonPointToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.addPologonPointToolStripMenuItem.Text = "Add Pologon Point";
            // 
            // clearPolygonToolStripMenuItem
            // 
            this.clearPolygonToolStripMenuItem.Name = "clearPolygonToolStripMenuItem";
            this.clearPolygonToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.clearPolygonToolStripMenuItem.Text = "Clear Polygon";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gMapControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.TXT_homealt);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.TXT_homelng);
            this.splitContainer1.Panel2.Controls.Add(this.Label2);
            this.splitContainer1.Panel2.Controls.Add(this.TXT_homelat);
            this.splitContainer1.Panel2.Controls.Add(this.Label1);
            this.splitContainer1.Panel2.Controls.Add(this.coords1);
            this.splitContainer1.Size = new System.Drawing.Size(642, 460);
            this.splitContainer1.SplitterDistance = 412;
            this.splitContainer1.TabIndex = 2;
            // 
            // gMapControl1
            // 
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Gray;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(0, 0);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 24;
            this.gMapControl1.MinZoom = 0;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedArea = ((GMap.NET.RectLatLng)(resources.GetObject("gMapControl1.SelectedArea")));
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(642, 412);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 0D;
            // 
            // coords1
            // 
            this.coords1.Alt = 0D;
            this.coords1.AltSource = "";
            this.coords1.AltUnit = "m";
            this.coords1.Lat = 0D;
            this.coords1.Lng = 0D;
            this.coords1.Location = new System.Drawing.Point(3, 19);
            this.coords1.Name = "coords1";
            this.coords1.Size = new System.Drawing.Size(200, 21);
            this.coords1.TabIndex = 0;
            this.coords1.Vertical = false;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(292, 19);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(20, 12);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Lat";
            // 
            // TXT_homelat
            // 
            this.TXT_homelat.Location = new System.Drawing.Point(318, 16);
            this.TXT_homelat.Name = "TXT_homelat";
            this.TXT_homelat.Size = new System.Drawing.Size(65, 22);
            this.TXT_homelat.TabIndex = 2;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label2.Location = new System.Drawing.Point(389, 19);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(30, 12);
            this.Label2.TabIndex = 5;
            this.Label2.Text = "Long";
            // 
            // TXT_homelng
            // 
            this.TXT_homelng.Location = new System.Drawing.Point(425, 16);
            this.TXT_homelng.Name = "TXT_homelng";
            this.TXT_homelng.Size = new System.Drawing.Size(65, 22);
            this.TXT_homelng.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(496, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "Alt (abs)";
            // 
            // TXT_homealt
            // 
            this.TXT_homealt.Location = new System.Drawing.Point(547, 16);
            this.TXT_homealt.Name = "TXT_homealt";
            this.TXT_homealt.Size = new System.Drawing.Size(65, 22);
            this.TXT_homealt.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(209, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 17;
            this.label4.TabStop = true;
            this.label4.Text = "Home Location";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(618, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainMap
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainMap";
            this.Size = new System.Drawing.Size(642, 460);
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem drawPolygonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPologonPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearPolygonToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        public MissionPlanner.Controls.myGMAP gMapControl1;
        private MissionPlanner.Controls.Coords coords1;
        private System.Windows.Forms.LinkLabel label4;
        private System.Windows.Forms.TextBox TXT_homealt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TXT_homelng;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.TextBox TXT_homelat;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Button button1;
    }
}
