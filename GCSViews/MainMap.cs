using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.IO;

namespace AUV_GCS.GCSViews
{
    public partial class MainMap : MyUserControl
    {
        public static myGMAP mymap;
        public MainMap()
        {
            InitializeComponent();
            // mymap = gMapControl1;
            gMapControl1.MapProvider = GoogleSatelliteMapProvider.Instance;
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 3;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CacheLocation = Settings.GetDataDirectory() +
                                          "gmapcache" + Path.DirectorySeparatorChar;
            /**=gMapControl1.Position = new PointLatLng(Settings.Instance.GetDouble("maplast_lat"),
                        Settings.Instance.GetDouble("maplast_lng"));*/


        }
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            
            /*this.gMapControl1.MapProvider = OpenStreet4UMapProvider.Instance; // 设置地图源
            GMaps.Instance.Mode = AccessMode.ServerAndCache; // GMap工作模式
            this.gMapControl1.SetPositionByKeywords("北京"); // 地图中心位置*/

        }

        private void MainMap_Load(object sender, EventArgs e)
        {
                
        }
    }
}
