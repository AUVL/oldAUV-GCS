namespace AUV_GCS.FunctionTabControl
{
    partial class WayPointData
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Commands = new MissionPlanner.Controls.MyDataGridView();
            this.Command = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Lat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Up = new System.Windows.Forms.DataGridViewImageColumn();
            this.Down = new System.Windows.Forms.DataGridViewImageColumn();
            this.Dist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Group = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LBL_defalutalt = new System.Windows.Forms.Label();
            this.TXT_DefaultAlt = new System.Windows.Forms.TextBox();
            this.LBL_WPRad = new System.Windows.Forms.Label();
            this.TXT_WPRad = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Commands)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.TXT_WPRad);
            this.panel1.Controls.Add(this.LBL_WPRad);
            this.panel1.Controls.Add(this.TXT_DefaultAlt);
            this.panel1.Controls.Add(this.LBL_defalutalt);
            this.panel1.Controls.Add(this.Commands);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(571, 318);
            this.panel1.TabIndex = 0;
            // 
            // Commands
            // 
            this.Commands.AllowUserToAddRows = false;
            this.Commands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Commands.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            this.Commands.ColumnHeadersHeight = 30;
            this.Commands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Command,
            this.Lat,
            this.Lon,
            this.Alt,
            this.Delete,
            this.Up,
            this.Down,
            this.Dist,
            this.Group});
            this.Commands.Location = new System.Drawing.Point(6, 67);
            this.Commands.Name = "Commands";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.Commands.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.Commands.RowHeadersWidth = 50;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.Commands.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.Commands.RowTemplate.Height = 24;
            this.Commands.Size = new System.Drawing.Size(565, 150);
            this.Commands.TabIndex = 0;
            // 
            // Command
            // 
            this.Command.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            this.Command.DefaultCellStyle = dataGridViewCellStyle1;
            this.Command.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Command.HeaderText = "Command";
            this.Command.MinimumWidth = 60;
            this.Command.Name = "Command";
            this.Command.Width = 60;
            // 
            // Lat
            // 
            this.Lat.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Lat.HeaderText = "Lat";
            this.Lat.MinimumWidth = 60;
            this.Lat.Name = "Lat";
            this.Lat.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Lat.Width = 60;
            // 
            // Lon
            // 
            this.Lon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Lon.HeaderText = "Lon";
            this.Lon.MinimumWidth = 60;
            this.Lon.Name = "Lon";
            this.Lon.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Lon.Width = 60;
            // 
            // Alt
            // 
            this.Alt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Alt.HeaderText = "Alt";
            this.Alt.MinimumWidth = 60;
            this.Alt.Name = "Alt";
            this.Alt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Alt.Width = 60;
            // 
            // Delete
            // 
            this.Delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Delete.HeaderText = "Delete";
            this.Delete.MinimumWidth = 50;
            this.Delete.Name = "Delete";
            this.Delete.Text = "X";
            this.Delete.ToolTipText = "Delete the row";
            this.Delete.Width = 50;
            // 
            // Up
            // 
            this.Up.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Up.HeaderText = "Up";
            this.Up.Image = global::AUV_GCS.Properties.Resources.up;
            this.Up.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch;
            this.Up.MinimumWidth = 30;
            this.Up.Name = "Up";
            this.Up.ToolTipText = "Move the row UP";
            this.Up.Width = 30;
            // 
            // Down
            // 
            this.Down.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Down.HeaderText = "Down";
            this.Down.Image = global::AUV_GCS.Properties.Resources.down;
            this.Down.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch;
            this.Down.MinimumWidth = 40;
            this.Down.Name = "Down";
            this.Down.ToolTipText = "Move the row down";
            this.Down.Width = 40;
            // 
            // Dist
            // 
            this.Dist.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Dist.HeaderText = "Dist";
            this.Dist.MinimumWidth = 30;
            this.Dist.Name = "Dist";
            this.Dist.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Dist.Width = 30;
            // 
            // Group
            // 
            this.Group.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Group.HeaderText = "Group";
            this.Group.MinimumWidth = 50;
            this.Group.Name = "Group";
            this.Group.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Group.Width = 50;
            // 
            // LBL_defalutalt
            // 
            this.LBL_defalutalt.AutoSize = true;
            this.LBL_defalutalt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LBL_defalutalt.Location = new System.Drawing.Point(67, 9);
            this.LBL_defalutalt.Name = "LBL_defalutalt";
            this.LBL_defalutalt.Size = new System.Drawing.Size(56, 12);
            this.LBL_defalutalt.TabIndex = 20;
            this.LBL_defalutalt.Text = "Default Alt";
            // 
            // TXT_DefaultAlt
            // 
            this.TXT_DefaultAlt.Location = new System.Drawing.Point(69, 24);
            this.TXT_DefaultAlt.Name = "TXT_DefaultAlt";
            this.TXT_DefaultAlt.Size = new System.Drawing.Size(40, 22);
            this.TXT_DefaultAlt.TabIndex = 21;
            this.TXT_DefaultAlt.Text = "100";
            // 
            // LBL_WPRad
            // 
            this.LBL_WPRad.AutoSize = true;
            this.LBL_WPRad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LBL_WPRad.Location = new System.Drawing.Point(4, 9);
            this.LBL_WPRad.Name = "LBL_WPRad";
            this.LBL_WPRad.Size = new System.Drawing.Size(57, 12);
            this.LBL_WPRad.TabIndex = 22;
            this.LBL_WPRad.Text = "WP Radius";
            // 
            // TXT_WPRad
            // 
            this.TXT_WPRad.Location = new System.Drawing.Point(15, 24);
            this.TXT_WPRad.Name = "TXT_WPRad";
            this.TXT_WPRad.Size = new System.Drawing.Size(36, 22);
            this.TXT_WPRad.TabIndex = 23;
            this.TXT_WPRad.Text = "30";
            // 
            // WayPointData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "WayPointData";
            this.Size = new System.Drawing.Size(571, 318);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Commands)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.DataGridViewImageColumn Up;
        private System.Windows.Forms.DataGridViewImageColumn Down;
        private System.Windows.Forms.DataGridViewTextBoxColumn Group;
        public MissionPlanner.Controls.MyDataGridView Commands;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.TextBox TXT_DefaultAlt;
        private System.Windows.Forms.Label LBL_defalutalt;
        public System.Windows.Forms.DataGridViewTextBoxColumn Lat;
        public System.Windows.Forms.DataGridViewTextBoxColumn Lon;
        public System.Windows.Forms.DataGridViewTextBoxColumn Alt;
        public System.Windows.Forms.DataGridViewComboBoxColumn Command;
        public System.Windows.Forms.TextBox TXT_WPRad;
        private System.Windows.Forms.Label LBL_WPRad;
        public System.Windows.Forms.DataGridViewTextBoxColumn Dist;
    }
}
