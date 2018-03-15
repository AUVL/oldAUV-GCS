using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace AUV_GCS.GCSViews
{
    public partial class MainWorkspace : MyUserControl
    {
        //public  FunctionTabControl.WayPointData WayPointData;
        public MainWorkspace()
        {
            
            InitializeComponent();
            //tabPage1.BackColor=  Color.FromArgb(0x26, 0x27, 0x28);
           // WayPointData = new FunctionTabControl.WayPointData();
            WayPoint_Tab.Controls.Add(new FunctionTabControl.WayPointData());
           // WayPoint_Tab.Controls.Add(WayPointData);
           // WayPointData.Dock = DockStyle.Fill;
            
            

        }
    }
}
