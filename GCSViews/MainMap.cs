using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.IO;
using log4net;
using System.Reflection;

namespace AUV_GCS.GCSViews
{
    public partial class MainMap : MyUserControl
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static myGMAP mymap;
        static public Object thisLock = new Object();
        private ComponentResourceManager rm = new ComponentResourceManager(typeof(MainMap));
        MissionPlanner.Controls.Icon.Polygon polyicon = new MissionPlanner.Controls.Icon.Polygon();
        bool polygongridmode;
        bool sethome;
        bool splinemode;
        public bool quickadd;
        int selectedrow;
        private Dictionary<string, string[]> cmdParamNames = new Dictionary<string, string[]>();

        public GMapRoute route = new GMapRoute("wp route");
        public GMapRoute homeroute = new GMapRoute("home route");

        List<int> groupmarkers = new List<int>();
        List<List<Locationwp>> history = new List<List<Locationwp>>();
        public List<PointLatLngAlt> pointlist = new List<PointLatLngAlt>(); // used to calc distance
        public List<PointLatLngAlt> fullpointlist = new List<PointLatLngAlt>();

        public AUV_GCS.FunctionTabControl.WayPointData WayPointData;
        PointLatLng MouseDownStart;
        PointLatLngAlt mouseposdisplay = new PointLatLngAlt(0, 0);

        
        //layout
        public static GMapOverlay objectsoverlay; // where the markers a drawn

        // polygons
        GMapPolygon wppolygon;
        internal GMapPolygon drawnpolygon;
        GMapPolygon geofencepolygon;

        // marker
        GMapMarker center = new GMarkerGoogle(new PointLatLng(0.0, 0.0), GMarkerGoogleType.none);
        GMapMarker currentMarker;

        // etc
        bool isMouseDown;
        bool isMouseDraging;
        bool isMouseClickOffMenu;
        GMapMarkerRallyPt CurrentRallyPt;
        GMapMarker CurrentGMapMarker;
        GMapMarkerPOI CurrentPOIMarker;
        MissionPlanner.GMapMarkerRect CurentRectMarker;
        internal PointLatLng MouseDownEnd;

        //layers
        GMapOverlay drawnpolygonsoverlay;
        GMapOverlay kmlpolygonsoverlay;
        GMapOverlay geofenceoverlay;
        static GMapOverlay rallypointoverlay;
        public static GMapOverlay polygonsoverlay; // where the track is drawn

        public MainMap()
        {
            InitializeComponent();

            // set current marker
            currentMarker = new GMarkerGoogle(gMapControl1.Position, GMarkerGoogleType.red);


            WayPointData = new AUV_GCS.FunctionTabControl.WayPointData();
            // mymap = gMapControl1;
            gMapControl1.MapProvider = GoogleSatelliteMapProvider.Instance;
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 3;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CacheLocation = Settings.GetDataDirectory() +
                                          "gmapcache" + Path.DirectorySeparatorChar;

            // map events
            gMapControl1.OnPositionChanged += gMapControl1_OnCurrentPositionChanged;
            gMapControl1.OnTileLoadStart += gMapControl1_OnTileLoadStart;
            gMapControl1.OnTileLoadComplete += gMapControl1_OnTileLoadComplete;
            gMapControl1.OnMarkerClick += gMapControl1_OnMarkerClick;
            gMapControl1.OnMapZoomChanged += gMapControl1_OnMapZoomChanged;
            gMapControl1.MouseMove += gMapControl1_MouseMove;
            gMapControl1.MouseDown += gMapControl1_MouseDown;
            gMapControl1.MouseUp += gMapControl1_MouseUp;
            gMapControl1.OnMarkerEnter += gMapControl1_OnMarkerEnter;
            gMapControl1.OnMarkerLeave += gMapControl1_OnMarkerLeave;
            /**=gMapControl1.Position = new PointLatLng(Settings.Instance.GetDouble("maplast_lat"),
                        Settings.Instance.GetDouble("maplast_lng"));*/

            updateCMDParams();
        }

        void gMapControl1_OnCurrentPositionChanged(PointLatLng point)
        {
            if (point.Lat > 90)
            {
                point.Lat = 90;
            }
            if (point.Lat < -90)
            {
                point.Lat = -90;
            }
            if (point.Lng > 180)
            {
                point.Lng = 180;
            }
            if (point.Lng < -180)
            {
                point.Lng = -180;
            }
            center.Position = point;

            coords1.Lat = point.Lat;
            coords1.Lng = point.Lng;

            // always show on planner view
            //if (MainV2.ShowAirports)
            /*{
                airportsoverlay.Clear();
                foreach (var item in Airports.getAirports(MainMap.Position))
                {
                    airportsoverlay.Markers.Add(new GMapMarkerAirport(item)
                    {
                        ToolTipText = item.Tag,
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    });
                }
            }*/
        }
        void gMapControl1_OnTileLoadStart()
        {
            /*MethodInvoker m = delegate { lbl_status.Text = "Status: loading tiles..."; };
            try
            {
                if (IsHandleCreated)
                    BeginInvoke(m);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }*/
        }
        void gMapControl1_OnTileLoadComplete(long ElapsedMilliseconds)
        {
            //MainMap.ElapsedMilliseconds = ElapsedMilliseconds;

            MethodInvoker m = delegate
            {
                //lbl_status.Text = "Status: loaded tiles";

                //panelMenu.Text = "Menu, last load in " + MainMap.ElapsedMilliseconds + "ms";

                //textBoxMemory.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00}MB of {1:0.00}MB", MainMap.Manager.MemoryCacheSize, MainMap.Manager.MemoryCacheCapacity);
            };
            try
            {
                if (!IsDisposed && IsHandleCreated)
                    BeginInvoke(m);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
         {
             int answer;
             try // when dragging item can sometimes be null
             {
                 if (item.Tag == null)
                 {
                     // home.. etc
                     return;
                 }

                 if (ModifierKeys == Keys.Control)
                 {
                     try
                     {
                         groupmarkeradd(item);

                         log.Info("add marker to group");
                     }
                     catch (Exception ex)
                     {
                         log.Error(ex);
                     }
                 }
                 if (int.TryParse(item.Tag.ToString(), out answer))
                 {
                   WayPointData.Commands.CurrentCell = WayPointData.Commands[0, answer - 1];
                 }
             }
             catch (Exception ex)
             {
                 log.Error(ex);
             }
         }
        void groupmarkeradd(GMapMarker marker)
        {
            System.Diagnostics.Debug.WriteLine("add marker " + marker.Tag.ToString());
            groupmarkers.Add(int.Parse(marker.Tag.ToString()));
            if (marker is  MissionPlanner.GMapMarkerWP)
            {
                ((MissionPlanner.GMapMarkerWP)marker).selected = true;
            }
            if (marker is MissionPlanner.GMapMarkerRect)
            {
                ((MissionPlanner.GMapMarkerWP)((MissionPlanner.GMapMarkerRect)marker).InnerMarker).selected = true;
            }
        }
        void gMapControl1_OnMapZoomChanged()
        {
            if (gMapControl1.Zoom > 0)
            {
                try
                {
                    //trackBar1.Value = (int)(gMapControl1.Zoom);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                //textBoxZoomCurrent.Text = gMapControl1.Zoom.ToString();
                center.Position = gMapControl1.Position;
            }
        }
        // move current marker with left holding
        void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            if (MouseDownStart == point)
                return;

            //  Console.WriteLine("MainMap MM " + point);

            currentMarker.Position = point;

            if (!isMouseDown)
            {
                // update mouse pos display
                SetMouseDisplay(point.Lat, point.Lng, 0);
            }

            //draging
            if (e.Button == MouseButtons.Left && isMouseDown)
            {
                isMouseDraging = true;
                if (CurrentRallyPt != null)
                {
                    PointLatLng pnew = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                    CurrentRallyPt.Position = pnew;
                }
                else if (groupmarkers.Count > 0)
                {
                    // group drag

                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    MouseDownStart = point;

                    Hashtable seen = new Hashtable();

                    foreach (var markerid in groupmarkers)
                    {
                        if (seen.ContainsKey(markerid))
                            continue;

                        seen[markerid] = 1;
                        for (int a = 0; a < objectsoverlay.Markers.Count; a++)
                        {
                            var marker = objectsoverlay.Markers[a];

                            if (marker.Tag != null && marker.Tag.ToString() == markerid.ToString())
                            {
                                var temp = new PointLatLng(marker.Position.Lat, marker.Position.Lng);
                                temp.Offset(latdif, -lngdif);
                                marker.Position = temp;
                            }
                        }
                    }
                }
                else if (CurentRectMarker != null) // left click pan
                {
                    try
                    {
                        // check if this is a grid point
                        if (CurentRectMarker.InnerMarker.Tag.ToString().Contains("grid"))
                        {
                            drawnpolygon.Points[
                                int.Parse(CurentRectMarker.InnerMarker.Tag.ToString().Replace("grid", "")) - 1] =
                                new PointLatLng(point.Lat, point.Lng);
                            gMapControl1.UpdatePolygonLocalPosition(drawnpolygon);
                            gMapControl1.Invalidate();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    PointLatLng pnew = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                    // adjust polyline point while we drag
                    try
                    {
                        if (CurrentGMapMarker != null && CurrentGMapMarker.Tag is int)
                        {
                            int? pIndex = (int?)CurentRectMarker.Tag;
                            if (pIndex.HasValue)
                            {
                                if (pIndex < wppolygon.Points.Count)
                                {
                                    wppolygon.Points[pIndex.Value] = pnew;
                                    lock (thisLock)
                                    {
                                        gMapControl1.UpdatePolygonLocalPosition(wppolygon);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    // update rect and marker pos.
                    if (currentMarker.IsVisible)
                    {
                        currentMarker.Position = pnew;
                    }
                    CurentRectMarker.Position = pnew;

                    if (CurentRectMarker.InnerMarker != null)
                    {
                        CurentRectMarker.InnerMarker.Position = pnew;
                    }
                }
                else if (CurrentPOIMarker != null)
                {
                    PointLatLng pnew = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                    CurrentPOIMarker.Position = pnew;
                }
                else if (CurrentGMapMarker != null)
                {
                    PointLatLng pnew = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                    CurrentGMapMarker.Position = pnew;
                }
                else if (ModifierKeys == Keys.Control)
                {
                    // draw selection box
                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    gMapControl1.SelectedArea = new RectLatLng(Math.Max(MouseDownStart.Lat, point.Lat),
                        Math.Min(MouseDownStart.Lng, point.Lng), Math.Abs(lngdif), Math.Abs(latdif));
                }
                else // left click pan
                {
                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    try
                    {
                        lock (thisLock)
                        {
                            if (!isMouseClickOffMenu)
                                gMapControl1.Position = new PointLatLng(center.Position.Lat + latdif,
                                    center.Position.Lng + lngdif);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
            else if (e.Button == MouseButtons.None)
            {
                isMouseDown = false;
            }
        }
        public void SetMouseDisplay(double lat, double lng, int alt)
        {
            mouseposdisplay.Lat = lat;
            mouseposdisplay.Lng = lng;
            mouseposdisplay.Alt = alt;

            //coords1.Lat = mouseposdisplay.Lat;
            //coords1.Lng = mouseposdisplay.Lng;
            var altdata = MissionPlanner.srtm.getAltitude(mouseposdisplay.Lat, mouseposdisplay.Lng, gMapControl1.Zoom);
            //coords1.Alt = altdata.alt;
            //coords1.AltSource = altdata.altsource;

            try
            {
                PointLatLng last;

                if (pointlist.Count == 0 || pointlist[pointlist.Count - 1] == null)
                    return;

                last = pointlist[pointlist.Count - 1];

                double lastdist = gMapControl1.MapProvider.Projection.GetDistance(last, currentMarker.Position);

                double lastbearing = 0;

                if (pointlist.Count > 0)
                {
                    lastbearing = gMapControl1.MapProvider.Projection.GetBearing(last, currentMarker.Position);
                }

                //lbl_prevdist.Text = rm.GetString("lbl_prevdist.Text") + ": " + FormatDistance(lastdist, true) + " AZ: " +
                                    //lastbearing.ToString("0");

                // 0 is home
                if (pointlist[0] != null)
                {
                    double homedist = gMapControl1.MapProvider.Projection.GetDistance(currentMarker.Position, pointlist[0]);

                    //lbl_homedist.Text = rm.GetString("lbl_homedist.Text") + ": " + FormatDistance(homedist, true);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        private string FormatDistance(double distInKM, bool toMeterOrFeet)
        {
            string sunits = Settings.Instance["distunits"];
            MissionPlanner.Common.distances units = MissionPlanner.Common.distances.Meters;

            if (sunits != null)
                try
                {
                    units = (MissionPlanner.Common.distances)Enum.Parse(typeof(MissionPlanner.Common.distances), sunits);
                }
                catch (Exception)
                {
                }

            switch (units)
            {
                case MissionPlanner.Common.distances.Feet:
                    return toMeterOrFeet
                        ? string.Format((distInKM * 3280.8399).ToString("0.00 ft"))
                        : string.Format((distInKM * 0.621371).ToString("0.0000 miles"));
                case MissionPlanner.Common.distances.Meters:
                default:
                    return toMeterOrFeet
                        ? string.Format((distInKM * 1000).ToString("0.00 m"))
                        : string.Format(distInKM.ToString("0.0000 km"));
            }

        }
        void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isMouseClickOffMenu)
                return;

            MouseDownStart = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            //   Console.WriteLine("MainMap MD");

            if (e.Button == MouseButtons.Left && (groupmarkers.Count > 0 || ModifierKeys == Keys.Control))
            {
                // group move
                isMouseDown = true;
                isMouseDraging = false;

                return;
            }

            if (e.Button == MouseButtons.Left && ModifierKeys != Keys.Alt && ModifierKeys != Keys.Control)
            {
                isMouseDown = true;
                isMouseDraging = false;

                if (currentMarker.IsVisible)
                {
                    currentMarker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                }
            }
        }
        void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseClickOffMenu)
            {
                isMouseClickOffMenu = false;
                return;
            }

            // check if the mouse up happend over our button
            if (polyicon.Rectangle.Contains(e.Location))
            {
                polyicon.IsSelected = !polyicon.IsSelected;

                if (e.Button == MouseButtons.Right)
                {
                    polyicon.IsSelected = false;
                    clearPolygonToolStripMenuItem_Click(this, null);

                    contextMenuStrip1.Visible = false;

                    return;
                }

                if (polyicon.IsSelected)
                {
                    polygongridmode = true;
                }
                else
                {
                    polygongridmode = false;
                }

                return;
            }

            MouseDownEnd = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            // Console.WriteLine("MainMap MU");

            if (e.Button == MouseButtons.Right) // ignore right clicks
            {
                return;
            }

            if (isMouseDown) // mouse down on some other object and dragged to here.
            {
                // drag finished, update poi db
                if (CurrentPOIMarker != null)
                {
                    POI.POIMove(CurrentPOIMarker);
                    CurrentPOIMarker = null;
                }

                if (e.Button == MouseButtons.Left)
                {
                    isMouseDown = false;
                }
                if (ModifierKeys == Keys.Control)
                {
                    // group select wps
                    GMapPolygon poly = new GMapPolygon(new List<PointLatLng>(), "temp");

                    poly.Points.Add(MouseDownStart);
                    poly.Points.Add(new PointLatLng(MouseDownStart.Lat, MouseDownEnd.Lng));
                    poly.Points.Add(MouseDownEnd);
                    poly.Points.Add(new PointLatLng(MouseDownEnd.Lat, MouseDownStart.Lng));

                    foreach (var marker in objectsoverlay.Markers)
                    {
                        if (poly.IsInside(marker.Position))
                        {
                            try
                            {
                                if (marker.Tag != null)
                                {
                                    groupmarkeradd(marker);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }

                    isMouseDraging = false;
                    return;
                }
                if (!isMouseDraging)
                {
                    if (CurentRectMarker != null)
                    {
                        // cant add WP in existing rect
                    }
                    else
                    {
                        AddWPToMap(currentMarker.Position.Lat, currentMarker.Position.Lng, 0);
                    }
                }
                else
                {
                    if (groupmarkers.Count > 0)
                    {
                        Dictionary<string, PointLatLng> dest = new Dictionary<string, PointLatLng>();

                        foreach (var markerid in groupmarkers)
                        {
                            for (int a = 0; a < objectsoverlay.Markers.Count; a++)
                            {
                                var marker = objectsoverlay.Markers[a];

                                if (marker.Tag != null && marker.Tag.ToString() == markerid.ToString())
                                {
                                    dest[marker.Tag.ToString()] = marker.Position;
                                    break;
                                }
                            }
                        }

                        foreach (KeyValuePair<string, PointLatLng> item in dest)
                        {
                            var value = item.Value;
                            quickadd = true;
                            callMeDrag(item.Key, value.Lat, value.Lng, -1);
                            quickadd = false;
                        }

                        gMapControl1.SelectedArea = RectLatLng.Empty;
                        groupmarkers.Clear();
                        // redraw to remove selection
                        writeKML();

                        CurentRectMarker = null;
                    }

                    if (CurentRectMarker != null && CurentRectMarker.InnerMarker != null)
                    {
                        if (CurentRectMarker.InnerMarker.Tag.ToString().Contains("grid"))
                        {
                            try
                            {
                                drawnpolygon.Points[
                                    int.Parse(CurentRectMarker.InnerMarker.Tag.ToString().Replace("grid", "")) - 1] =
                                    new PointLatLng(MouseDownEnd.Lat, MouseDownEnd.Lng);
                                gMapControl1.UpdatePolygonLocalPosition(drawnpolygon);
                                gMapControl1.Invalidate();
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                        else
                        {
                            callMeDrag(CurentRectMarker.InnerMarker.Tag.ToString(), currentMarker.Position.Lat,
                                currentMarker.Position.Lng, -2);
                        }
                        CurentRectMarker = null;
                    }
                }
            }

            isMouseDraging = false;
        }
        void gMapControl1_OnMarkerEnter(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (item is MissionPlanner.GMapMarkerRect)
                {
                    MissionPlanner.GMapMarkerRect rc = item as MissionPlanner.GMapMarkerRect;
                    rc.Pen.Color = Color.Red;
                    gMapControl1.Invalidate(false);

                    int answer;
                    if (item.Tag != null && rc.InnerMarker != null &&
                        int.TryParse(rc.InnerMarker.Tag.ToString(), out answer))
                    {
                        try
                        {
                            WayPointData.Commands.CurrentCell = WayPointData.Commands[0, answer - 1];
                            item.ToolTipText = "Alt: " + WayPointData.Commands[WayPointData.Alt.Index, answer - 1].Value;
                            item.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }

                    CurentRectMarker = rc;
                }
                if (item is GMapMarkerRallyPt)
                {
                    CurrentRallyPt = item as GMapMarkerRallyPt;
                }
                if (item is GMapMarkerAirport)
                {
                    // do nothing - readonly
                    return;
                }
                if (item is GMapMarkerPOI)
                {
                    CurrentPOIMarker = item as GMapMarkerPOI;
                }
                if (item is MissionPlanner.GMapMarkerWP)
                {
                    //CurrentGMapMarker = item;
                }
                if (item is GMapMarker)
                {
                    //CurrentGMapMarker = item;
                }
            }
        }
        void gMapControl1_OnMarkerLeave(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (item is MissionPlanner.GMapMarkerRect)
                {
                    CurentRectMarker = null;
                    MissionPlanner.GMapMarkerRect rc = item as MissionPlanner.GMapMarkerRect;
                    rc.ResetColor();
                    gMapControl1.Invalidate(false);
                }
                if (item is GMapMarkerRallyPt)
                {
                    CurrentRallyPt = null;
                }
                if (item is GMapMarkerPOI)
                {
                    CurrentPOIMarker = null;
                }
                if (item is GMapMarker)
                {
                    // when you click the context menu this triggers and causes problems
                    CurrentGMapMarker = null;
                }
            }
        }
        private void clearPolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygongridmode = false;
            if (drawnpolygon == null)
                return;
            drawnpolygon.Points.Clear();
            drawnpolygonsoverlay.Markers.Clear();
            gMapControl1.Invalidate();

            writeKML();
        }
        public void AddWPToMap(double lat, double lng, int alt)
        {
            if (polygongridmode)
            {
                addPolygonPointToolStripMenuItem_Click(null, null);
                return;
            }

            if (sethome)
            {
                sethome = false;
                callMeDrag("H", lat, lng, alt);
                return;
            }
            // creating a WP

            selectedrow = WayPointData.Commands.Rows.Add();

            if (splinemode)
            {
                WayPointData.Commands.Rows[selectedrow].Cells[WayPointData.Command.Index].Value = MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString();
                ChangeColumnHeader(MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString());
            }
            else
            {
                WayPointData.Commands.Rows[selectedrow].Cells[WayPointData.Command.Index].Value = MAVLink.MAV_CMD.WAYPOINT.ToString();
                ChangeColumnHeader(MAVLink.MAV_CMD.WAYPOINT.ToString());
            }

            setfromMap(lat, lng, alt);
        }
        public void callMeDrag(string pointno, double lat, double lng, int alt)
        {
            if (pointno == "")
            {
                return;
            }

            // dragging a WP
            if (pointno == "H")
            {
                // auto update home alt
                TXT_homealt.Text = (MissionPlanner.srtm.getAltitude(lat, lng).alt * MissionPlanner.CurrentState.multiplierdist).ToString();

                TXT_homelat.Text = lat.ToString();
                TXT_homelng.Text = lng.ToString();
                return;
            }

            if (pointno == "Tracker Home")
            {
                MainForm.comPort.MAV.cs.TrackerLocation = new PointLatLngAlt(lat, lng, alt, "");
                return;
            }

            try
            {
                selectedrow = int.Parse(pointno) - 1;
                WayPointData.Commands.CurrentCell = WayPointData.Commands[1, selectedrow];
                // depending on the dragged item, selectedrow can be reset 
                selectedrow = int.Parse(pointno) - 1;
            }
            catch
            {
                return;
            }

            setfromMap(lat, lng, alt);
        }
        public void writeKML()
        {
            // quickadd is for when loading wps from eeprom or file, to prevent slow, loading times
            if (quickadd)
                return;

            // this is to share the current mission with the data tab
            pointlist = new List<PointLatLngAlt>();

            fullpointlist.Clear();

            Debug.WriteLine(DateTime.Now);
            try
            {
                if (objectsoverlay != null) // hasnt been created yet
                {
                    objectsoverlay.Markers.Clear();
                }

                // process and add home to the list
                string home;
                if (TXT_homealt.Text != "" && TXT_homelat.Text != "" && TXT_homelng.Text != "")
                {
                    home = string.Format("{0},{1},{2}\r\n", TXT_homelng.Text, TXT_homelat.Text, WayPointData.TXT_DefaultAlt.Text);
                    if (objectsoverlay != null) // during startup
                    {
                        pointlist.Add(new PointLatLngAlt(double.Parse(TXT_homelat.Text), double.Parse(TXT_homelng.Text),
                            double.Parse(TXT_homealt.Text), "H"));
                        fullpointlist.Add(pointlist[pointlist.Count - 1]);
                        addpolygonmarker("H", double.Parse(TXT_homelng.Text), double.Parse(TXT_homelat.Text), 0, null);
                    }
                }
                else
                {
                    home = "";
                    pointlist.Add(null);
                    fullpointlist.Add(pointlist[pointlist.Count - 1]);
                }

                // setup for centerpoint calc etc.
                double avglat = 0;
                double avglong = 0;
                double maxlat = -180;
                double maxlong = -180;
                double minlat = 180;
                double minlong = 180;
                double homealt = 0;
                try
                {
                    if (!String.IsNullOrEmpty(TXT_homealt.Text))
                        homealt = (int)double.Parse(TXT_homealt.Text);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                /*if ((altmode)CMB_altmode.SelectedValue == altmode.Absolute)
                {
                    homealt = 0; // for absolute we dont need to add homealt
                }*/

                int usable = 0;

                updateRowNumbers();

                long temp = Stopwatch.GetTimestamp();

                string lookat = "";
                for (int a = 0; a < WayPointData.Commands.Rows.Count - 0; a++)
                {
                    try
                    {
                        if (WayPointData.Commands.Rows[a].Cells[WayPointData.Command.Index].Value.ToString().Contains("UNKNOWN"))
                            continue;

                        ushort command =
                            (ushort)
                                    Enum.Parse(typeof(MAVLink.MAV_CMD),
                                        WayPointData.Commands.Rows[a].Cells[WayPointData.Command.Index].Value.ToString(), false);
                        if (command < (ushort)MAVLink.MAV_CMD.LAST &&
                            command != (ushort)MAVLink.MAV_CMD.TAKEOFF && // doesnt have a position
                            command != (ushort)MAVLink.MAV_CMD.VTOL_TAKEOFF && // doesnt have a position
                            command != (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH &&
                            command != (ushort)MAVLink.MAV_CMD.CONTINUE_AND_CHANGE_ALT &&
                            command != (ushort)MAVLink.MAV_CMD.GUIDED_ENABLE
                            || command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI)
                        {
                            string cell2 = WayPointData.Commands.Rows[a].Cells[WayPointData.Alt.Index].Value.ToString(); // alt
                            string cell3 = WayPointData.Commands.Rows[a].Cells[WayPointData.Lat.Index].Value.ToString(); // lat
                            string cell4 = WayPointData.Commands.Rows[a].Cells[WayPointData.Lon.Index].Value.ToString(); // lng

                            // land can be 0,0 or a lat,lng
                            if (command == (ushort)MAVLink.MAV_CMD.LAND && cell3 == "0" && cell4 == "0")
                                continue;

                            if (cell4 == "?" || cell3 == "?")
                                continue;

                            if (command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI)
                            {
                                pointlist.Add(new PointLatLngAlt(double.Parse(cell3), double.Parse(cell4),
                                    double.Parse(cell2) + homealt, "ROI" + (a + 1))
                                { color = Color.Red });
                                // do set roi is not a nav command. so we dont route through it
                                //fullpointlist.Add(pointlist[pointlist.Count - 1]);
                                GMarkerGoogle m =
                                    new GMarkerGoogle(new PointLatLng(double.Parse(cell3), double.Parse(cell4)),
                                        GMarkerGoogleType.red);
                                m.ToolTipMode = MarkerTooltipMode.Always;
                                m.ToolTipText = (a + 1).ToString();
                                m.Tag = (a + 1).ToString();

                                MissionPlanner.GMapMarkerRect mBorders = new MissionPlanner.GMapMarkerRect(m.Position);
                                {
                                    mBorders.InnerMarker = m;
                                    mBorders.Tag = "Dont draw line";
                                }

                                // check for clear roi, and hide it
                                if (m.Position.Lat != 0 && m.Position.Lng != 0)
                                {
                                    // order matters
                                    objectsoverlay.Markers.Add(m);
                                    objectsoverlay.Markers.Add(mBorders);
                                }
                            }
                            else if (command == (ushort)MAVLink.MAV_CMD.LOITER_TIME ||
                                     command == (ushort)MAVLink.MAV_CMD.LOITER_TURNS ||
                                     command == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM)
                            {
                                pointlist.Add(new PointLatLngAlt(double.Parse(cell3), double.Parse(cell4),
                                    double.Parse(cell2) + homealt, (a + 1).ToString())
                                {
                                    color = Color.LightBlue
                                });
                                fullpointlist.Add(pointlist[pointlist.Count - 1]);
                                addpolygonmarker((a + 1).ToString(), double.Parse(cell4), double.Parse(cell3),
                                    double.Parse(cell2), Color.LightBlue);
                            }
                            else if (command == (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                            {
                                pointlist.Add(new PointLatLngAlt(double.Parse(cell3), double.Parse(cell4),
                                    double.Parse(cell2) + homealt, (a + 1).ToString())
                                { Tag2 = "spline" });
                                fullpointlist.Add(pointlist[pointlist.Count - 1]);
                                addpolygonmarker((a + 1).ToString(), double.Parse(cell4), double.Parse(cell3),
                                    double.Parse(cell2), Color.Green);
                            }
                            else
                            {
                                pointlist.Add(new PointLatLngAlt(double.Parse(cell3), double.Parse(cell4),
                                    double.Parse(cell2) + homealt, (a + 1).ToString()));
                                fullpointlist.Add(pointlist[pointlist.Count - 1]);
                                addpolygonmarker((a + 1).ToString(), double.Parse(cell4), double.Parse(cell3),
                                    double.Parse(cell2), null);
                            }

                            avglong += double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lon.Index].Value.ToString());
                            avglat += double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lat.Index].Value.ToString());
                            usable++;

                            maxlong = Math.Max(double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lon.Index].Value.ToString()), maxlong);
                            maxlat = Math.Max(double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lat.Index].Value.ToString()), maxlat);
                            minlong = Math.Min(double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lon.Index].Value.ToString()), minlong);
                            minlat = Math.Min(double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lat.Index].Value.ToString()), minlat);

                            Debug.WriteLine(temp - Stopwatch.GetTimestamp());
                        }
                        else if (command == (ushort)MAVLink.MAV_CMD.DO_JUMP) // fix do jumps into the future
                        {
                            pointlist.Add(null);

                            //int wpno = int.Parse(WayPointData.Commands.Rows[a].Cells[Param1.Index].Value.ToString());
                            //int repeat = int.Parse(WayPointData.Commands.Rows[a].Cells[Param2.Index].Value.ToString());

                            List<PointLatLngAlt> list = new List<PointLatLngAlt>();

                            // cycle through reps
                            /*for (int repno = repeat; repno > 0; repno--)
                            {
                                // cycle through wps
                                for (int no = wpno; no <= a; no++)
                                {
                                    if (pointlist[no] != null)
                                        list.Add(pointlist[no]);
                                }
                            }*/

                            fullpointlist.AddRange(list);
                        }
                        else
                        {
                            pointlist.Add(null);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Info("writekml - bad wp data " + e);
                    }
                }

                if (usable > 0)
                {
                    avglat = avglat / usable;
                    avglong = avglong / usable;
                    double latdiff = maxlat - minlat;
                    double longdiff = maxlong - minlong;
                    float range = 4000;

                    Locationwp loc1 = new Locationwp();
                    loc1.lat = (minlat);
                    loc1.lng = (minlong);
                    Locationwp loc2 = new Locationwp();
                    loc2.lat = (maxlat);
                    loc2.lng = (maxlong);

                    //double distance = getDistance(loc1, loc2);  // same code as ardupilot
                    double distance = 2000;

                    if (usable > 1)
                    {
                        range = (float)(distance * 2);
                    }
                    else
                    {
                        range = 4000;
                    }

                    if (avglong != 0 && usable < 3)
                    {
                        // no autozoom
                        lookat = "<LookAt>     <longitude>" + (minlong + longdiff / 2).ToString(new CultureInfo("en-US")) +
                                 "</longitude>     <latitude>" + (minlat + latdiff / 2).ToString(new CultureInfo("en-US")) +
                                 "</latitude> <range>" + range + "</range> </LookAt>";
                        //MainMap.ZoomAndCenterMarkers("objects");
                        //MainMap.Zoom -= 1;
                        //MainMap_OnMapZoomChanged();
                    }
                }
                else if (home.Length > 5 && usable == 0)
                {
                    lookat = "<LookAt>     <longitude>" + TXT_homelng.Text.ToString(new CultureInfo("en-US")) +
                             "</longitude>     <latitude>" + TXT_homelat.Text.ToString(new CultureInfo("en-US")) +
                             "</latitude> <range>4000</range> </LookAt>";

                    RectLatLng? rect = gMapControl1.GetRectOfAllMarkers("objects");
                    if (rect.HasValue)
                    {
                        gMapControl1.Position = rect.Value.LocationMiddle;
                    }

                    //MainMap.Zoom = 17;

                    gMapControl1_OnMapZoomChanged();
                }

                //RegeneratePolygon();

                RegenerateWPRoute(fullpointlist);

                if (fullpointlist.Count > 0)
                {
                    double homedist = 0;

                    if (home.Length > 5)
                    {
                        homedist = gMapControl1.MapProvider.Projection.GetDistance(fullpointlist[fullpointlist.Count - 1],
                            fullpointlist[0]);
                    }

                    double dist = 0;

                    for (int a = 1; a < fullpointlist.Count; a++)
                    {
                        if (fullpointlist[a - 1] == null)
                            continue;

                        if (fullpointlist[a] == null)
                            continue;

                        dist += gMapControl1.MapProvider.Projection.GetDistance(fullpointlist[a - 1], fullpointlist[a]);
                    }

                    /*lbl_distance.Text = rm.GetString("lbl_distance.Text") + ": " +
                                        FormatDistance(dist + homedist, false);*/
                }

                setgradanddistandaz();
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }

            Debug.WriteLine(DateTime.Now);
        }
        private void addPolygonPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (polygongridmode == false)
            {
                CustomMessageBox.Show(
                    "You will remain in polygon mode until you clear the polygon or create a grid/upload a fence");
            }

            polygongridmode = true;

            List<PointLatLng> polygonPoints = new List<PointLatLng>();
            if (drawnpolygonsoverlay.Polygons.Count == 0)
            {
                drawnpolygon.Points.Clear();
                drawnpolygonsoverlay.Polygons.Add(drawnpolygon);
            }

            drawnpolygon.Fill = Brushes.Transparent;

            // remove full loop is exists
            if (drawnpolygon.Points.Count > 1 &&
                drawnpolygon.Points[0] == drawnpolygon.Points[drawnpolygon.Points.Count - 1])
                drawnpolygon.Points.RemoveAt(drawnpolygon.Points.Count - 1); // unmake a full loop

            drawnpolygon.Points.Add(new PointLatLng(MouseDownStart.Lat, MouseDownStart.Lng));

            addpolygonmarkergrid(drawnpolygon.Points.Count.ToString(), MouseDownStart.Lng, MouseDownStart.Lat, 0);

            gMapControl1.UpdatePolygonLocalPosition(drawnpolygon);

            gMapControl1.Invalidate();
        }
        private void ChangeColumnHeader(string command)
        {
            try
            {
                if (cmdParamNames.ContainsKey(command))
                    for (int i = 1; i <= 7; i++)
                        WayPointData.Commands.Columns[i].HeaderText = cmdParamNames[command][i - 1];
                else
                    for (int i = 1; i <= 7; i++)
                        WayPointData.Commands.Columns[i].HeaderText = "setme";
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString());
            }
        }
        public void setfromMap(double lat, double lng, int alt, double p1 = 0)
        {
            if (selectedrow > WayPointData.Commands.RowCount)
            {
                CustomMessageBox.Show("Invalid coord, How did you do this?");
                return;
            }

            try
            {
                // get current command list
                var currentlist = GetCommandList();
                // add history
                history.Add(currentlist);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("A invalid entry has been detected\n" + ex.Message, MissionPlanner.Strings.ERROR);
            }

            // remove more than 20 revisions
            if (history.Count > 20)
            {
                history.RemoveRange(0, history.Count - 20);
            }

            DataGridViewTextBoxCell cell;
            /*if (alt == -2 && Commands.Columns[Alt.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][6] /*"Alt"*///))
            /*{
                if (CHK_verifyheight.Checked && (altmode)CMB_altmode.SelectedValue != altmode.Terrain) //Drag with verifyheight // use srtm data
                {
                    cell = Commands.Rows[selectedrow].Cells[Alt.Index] as DataGridViewTextBoxCell;
                    float ans;
                    if (float.TryParse(cell.Value.ToString(), out ans))
                    {
                        ans = (int)ans;

                        DataGridViewTextBoxCell celllat =
                            Commands.Rows[selectedrow].Cells[Lat.Index] as DataGridViewTextBoxCell;
                        DataGridViewTextBoxCell celllon =
                            Commands.Rows[selectedrow].Cells[Lon.Index] as DataGridViewTextBoxCell;
                        int oldsrtm =
                            (int)
                                ((srtm.getAltitude(double.Parse(celllat.Value.ToString()),
                                    double.Parse(celllon.Value.ToString())).alt) * CurrentState.multiplierdist);
                        int newsrtm = (int)((srtm.getAltitude(lat, lng).alt) * CurrentState.multiplierdist);
                        int newh = (int)(ans + newsrtm - oldsrtm);

                        cell.Value = newh;

                        cell.DataGridView.EndEdit();
                    }
                }
            }*/
            if (WayPointData.Commands.Columns[WayPointData.Lat.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][4] /*"Lat"*/))
            {
                cell = WayPointData.Commands.Rows[selectedrow].Cells[WayPointData.Lat.Index] as DataGridViewTextBoxCell;
                cell.Value = lat.ToString("0.0000000");
                cell.DataGridView.EndEdit();
            }
            if (WayPointData.Commands.Columns[WayPointData.Lon.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][5] /*"Long"*/))
            {
                cell = WayPointData.Commands.Rows[selectedrow].Cells[WayPointData.Lon.Index] as DataGridViewTextBoxCell;
                cell.Value = lng.ToString("0.0000000");
                cell.DataGridView.EndEdit();
            }
            if (alt != -1 && alt != -2 &&
                WayPointData.Commands.Columns[WayPointData.Alt.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][6] /*"Alt"*/))
            {
                cell = WayPointData.Commands.Rows[selectedrow].Cells[WayPointData.Alt.Index] as DataGridViewTextBoxCell;

                {
                    double result;
                    bool pass = double.TryParse(TXT_homealt.Text, out result);

                    if (pass == false)
                    {
                        CustomMessageBox.Show("You must have a home altitude");
                        string homealt = "100";
                        if (DialogResult.Cancel == InputBox.Show("Home Alt", "Home Altitude", ref homealt))
                            return;
                        TXT_homealt.Text = homealt;
                    }
                    int results1;
                    if (!int.TryParse(WayPointData.TXT_DefaultAlt.Text, out results1))
                    {
                        CustomMessageBox.Show("Your default alt is not valid");
                        return;
                    }

                    if (results1 == 0)
                    {
                        string defalt = "100";
                        if (DialogResult.Cancel == InputBox.Show("Default Alt", "Default Altitude", ref defalt))
                            return;
                        WayPointData.TXT_DefaultAlt.Text = defalt;
                    }
                }

                cell.Value = WayPointData.TXT_DefaultAlt.Text;

                float ans;
                if (float.TryParse(cell.Value.ToString(), out ans))
                {
                    ans = (int)ans;
                    if (alt != 0) // use passed in value;
                        cell.Value = alt.ToString();
                    if (ans == 0) // default
                        cell.Value = 50;
                    if (ans == 0 && (MainForm.comPort.MAV.cs.firmware == MainForm.Firmwares.ArduCopter2))
                        cell.Value = 15;

                    // not online and verify alt via srtm
                    /*if (CHK_verifyheight.Checked) // use srtm data
                    {
                        // is absolute but no verify
                        if ((altmode)CMB_altmode.SelectedValue == altmode.Absolute)
                        {
                            //abs
                            cell.Value =
                                ((srtm.getAltitude(lat, lng).alt) * CurrentState.multiplierdist +
                                 int.Parse(TXT_DefaultAlt.Text)).ToString();
                        }
                        else if ((altmode)CMB_altmode.SelectedValue == altmode.Terrain)
                        {
                            cell.Value = int.Parse(TXT_DefaultAlt.Text);
                        }
                        else
                        {
                            //relative and verify
                            cell.Value =
                                ((int)(srtm.getAltitude(lat, lng).alt) * CurrentState.multiplierdist +
                                 int.Parse(TXT_DefaultAlt.Text) -
                                 (int)
                                     srtm.getAltitude(MainV2.comPort.MAV.cs.HomeLocation.Lat,
                                         MainV2.comPort.MAV.cs.HomeLocation.Lng).alt * CurrentState.multiplierdist)
                                    .ToString();
                        }
                    }*/

                    cell.DataGridView.EndEdit();
                }
                else
                {
                    CustomMessageBox.Show("Invalid Home or wp Alt");
                    cell.Style.BackColor = Color.Red;
                }
            }

            // convert to utm
            //convertFromGeographic(lat, lng);

            // Add more for other params
            /*if (Commands.Columns[Param1.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][1] /*"Delay"*///))
            /*{
                cell = Commands.Rows[selectedrow].Cells[Param1.Index] as DataGridViewTextBoxCell;
                cell.Value = p1;
                cell.DataGridView.EndEdit();
            }*/

            writeKML();
            WayPointData.Commands.EndEdit();
        }
        private void addpolygonmarker(string tag, double lng, double lat, double alt, Color? color)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                MissionPlanner.GMapMarkerWP m = new MissionPlanner.GMapMarkerWP(point, tag);
                m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                m.ToolTipText = "Alt: " + alt.ToString("0");
                m.Tag = tag;

                int wpno = -1;
                if (int.TryParse(tag, out wpno))
                {
                    // preselect groupmarker
                    if (groupmarkers.Contains(wpno))
                        m.selected = true;
                }

                //MissionPlanner.GMapMarkerRectWPRad mBorders = new MissionPlanner.GMapMarkerRectWPRad(point, (int)float.Parse(TXT_WPRad.Text), MainMap);
                MissionPlanner.GMapMarkerRect mBorders = new MissionPlanner.GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                    mBorders.Tag = tag;
                    mBorders.wprad = (int)(float.Parse(WayPointData.TXT_WPRad.Text) / MissionPlanner.CurrentState.multiplierdist);
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                }

                objectsoverlay.Markers.Add(m);
                objectsoverlay.Markers.Add(mBorders);
            }
            catch (Exception)
            {
            }
        }
        public enum altmode
        {
            Relative = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT,
            Absolute = MAVLink.MAV_FRAME.GLOBAL,
            Terrain = MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT
        }
        void updateRowNumbers()
        {
            // number rows 
            this.BeginInvoke((MethodInvoker)delegate
            {
                // thread for updateing row numbers
                for (int a = 0; a < WayPointData.Commands.Rows.Count - 0; a++)
                {
                    try
                    {
                        if (WayPointData.Commands.Rows[a].HeaderCell.Value == null)
                        {
                            //Commands.Rows[a].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            WayPointData.Commands.Rows[a].HeaderCell.Value = (a + 1).ToString();
                        }
                        // skip rows with the correct number
                        string rowno = WayPointData.Commands.Rows[a].HeaderCell.Value.ToString();
                        if (!rowno.Equals((a + 1).ToString()))
                        {
                            // this code is where the delay is when deleting.
                            WayPointData.Commands.Rows[a].HeaderCell.Value = (a + 1).ToString();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }
        private void RegenerateWPRoute(List<PointLatLngAlt> fullpointlist)
        {
            route.Clear();
            homeroute.Clear();

            polygonsoverlay.Routes.Clear();

            PointLatLngAlt lastpnt = fullpointlist[0];
            PointLatLngAlt lastpnt2 = fullpointlist[0];
            PointLatLngAlt lastnonspline = fullpointlist[0];
            List<PointLatLngAlt> splinepnts = new List<PointLatLngAlt>();
            List<PointLatLngAlt> wproute = new List<PointLatLngAlt>();

            // add home - this causeszx the spline to always have a straight finish
            fullpointlist.Add(fullpointlist[0]);

            for (int a = 0; a < fullpointlist.Count; a++)
            {
                if (fullpointlist[a] == null)
                    continue;

                if (fullpointlist[a].Tag2 == "spline")
                {
                    if (splinepnts.Count == 0)
                        splinepnts.Add(lastpnt);

                    splinepnts.Add(fullpointlist[a]);
                }
                else
                {
                    if (splinepnts.Count > 0)
                    {
                        List<PointLatLng> list = new List<PointLatLng>();

                        splinepnts.Add(fullpointlist[a]);

                        MissionPlanner.Controls.Waypoints.Spline2 sp = new MissionPlanner.Controls.Waypoints.Spline2();

                        //sp._flags.segment_type = MissionPlanner.Controls.Waypoints.Spline2.SegmentType.SEGMENT_STRAIGHT;
                        //sp._flags.reached_destination = true;
                        //sp._origin = sp.pv_location_to_vector(lastpnt);
                        //sp._destination = sp.pv_location_to_vector(fullpointlist[0]);

                        // sp._spline_origin_vel = sp.pv_location_to_vector(lastpnt) - sp.pv_location_to_vector(lastnonspline);

                        sp.set_wp_origin_and_destination(sp.pv_location_to_vector(lastpnt2),
                            sp.pv_location_to_vector(lastpnt));

                        sp._flags.reached_destination = true;

                        for (int no = 1; no < (splinepnts.Count - 1); no++)
                        {
                            MissionPlanner.Controls.Waypoints.Spline2.spline_segment_end_type segtype =
                                MissionPlanner.Controls.Waypoints.Spline2.spline_segment_end_type.SEGMENT_END_STRAIGHT;

                            if (no < (splinepnts.Count - 2))
                            {
                                segtype = MissionPlanner.Controls.Waypoints.Spline2.spline_segment_end_type.SEGMENT_END_SPLINE;
                            }

                            sp.set_spline_destination(sp.pv_location_to_vector(splinepnts[no]), false, segtype,
                                sp.pv_location_to_vector(splinepnts[no + 1]));

                            //sp.update_spline();

                            while (sp._flags.reached_destination == false)
                            {
                                float t = 1f;
                                //sp.update_spline();
                                sp.advance_spline_target_along_track(t);
                                // Console.WriteLine(sp.pv_vector_to_location(sp.target_pos).ToString());
                                list.Add(sp.pv_vector_to_location(sp.target_pos));
                            }

                            list.Add(splinepnts[no]);
                        }

                        list.ForEach(x => { wproute.Add(x); });


                        splinepnts.Clear();

                        /*
                        MissionPlanner.Controls.Waypoints.Spline sp = new Controls.Waypoints.Spline();
                        
                        var spline = sp.doit(splinepnts, 20, lastlastpnt.GetBearing(splinepnts[0]),false);

                  
                         */

                        lastnonspline = fullpointlist[a];
                    }

                    wproute.Add(fullpointlist[a]);

                    lastpnt2 = lastpnt;
                    lastpnt = fullpointlist[a];
                }
            }
            /*

           List<PointLatLng> list = new List<PointLatLng>();
           fullpointlist.ForEach(x => { list.Add(x); });
           route.Points.AddRange(list);
           */
            // route is full need to get 1, 2 and last point as "HOME" route

            int count = wproute.Count;
            int counter = 0;
            PointLatLngAlt homepoint = new PointLatLngAlt();
            PointLatLngAlt firstpoint = new PointLatLngAlt();
            PointLatLngAlt lastpoint = new PointLatLngAlt();
            if (count > 2)
            {
                // homeroute = last, home, first
                wproute.ForEach(x =>
                {
                    counter++;
                    if (counter == 1)
                    {
                        homepoint = x;
                        return;
                    }
                    if (counter == 2)
                    {
                        firstpoint = x;
                    }
                    if (counter == count - 1)
                    {
                        lastpoint = x;
                    }
                    if (counter == count)
                    {
                        homeroute.Points.Add(lastpoint);
                        homeroute.Points.Add(homepoint);
                        homeroute.Points.Add(firstpoint);
                        return;
                    }
                    route.Points.Add(x);
                });

                homeroute.Stroke = new Pen(Color.Yellow, 2);
                // if we have a large distance between home and the first/last point, it hangs on the draw of a the dashed line.
                if (homepoint.GetDistance(lastpoint) < 5000 && homepoint.GetDistance(firstpoint) < 5000)
                    homeroute.Stroke.DashStyle = DashStyle.Dash;

                polygonsoverlay.Routes.Add(homeroute);

                route.Stroke = new Pen(Color.Yellow, 4);
                route.Stroke.DashStyle = DashStyle.Custom;
                polygonsoverlay.Routes.Add(route);
            }
        }
        void setgradanddistandaz()
        {
            int a = 0;
            PointLatLngAlt last = MainForm.comPort.MAV.cs.HomeLocation;
            foreach (var lla in pointlist)
            {
                if (lla == null)
                    continue;
                try
                {
                    if (lla.Tag != null && lla.Tag != "H" && !lla.Tag.Contains("ROI"))
                    {
                        double height = lla.Alt - last.Alt;
                        double distance = lla.GetDistance(last) * MissionPlanner.CurrentState.multiplierdist;
                        double grad = height / distance;

                        /*WayPointData.Commands.Rows[int.Parse(lla.Tag) - 1].Cells[Grad.Index].Value =
                            (grad * 100).ToString("0.0");

                        WayPointData.Commands.Rows[int.Parse(lla.Tag) - 1].Cells[Angle.Index].Value =
                            ((180.0 / Math.PI) * Math.Atan(grad)).ToString("0.0");*/

                        WayPointData.Commands.Rows[int.Parse(lla.Tag) - 1].Cells[WayPointData.Dist.Index].Value =
                            (Math.Sqrt(Math.Pow(distance, 2) + Math.Pow(height, 2))).ToString("0.0");

                        /*WayPointData.Commands.Rows[int.Parse(lla.Tag) - 1].Cells[AZ.Index].Value =
                            ((lla.GetBearing(last) + 180) % 360).ToString("0");*/
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                a++;
                last = lla;
            }
        }
        private void addpolygonmarkergrid(string tag, double lng, double lat, int alt)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMarkerGoogle m = new GMarkerGoogle(point, GMarkerGoogleType.red);
                m.ToolTipMode = MarkerTooltipMode.Never;
                m.ToolTipText = "grid" + tag;
                m.Tag = "grid" + tag;

                //MissionPlanner.GMapMarkerRectWPRad mBorders = new MissionPlanner.GMapMarkerRectWPRad(point, (int)float.Parse(TXT_WPRad.Text), MainMap);
                MissionPlanner.GMapMarkerRect mBorders = new MissionPlanner.GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                }

                drawnpolygonsoverlay.Markers.Add(m);
                drawnpolygonsoverlay.Markers.Add(mBorders);
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }
        }
        List<Locationwp> GetCommandList()
        {
            List<Locationwp> commands = new List<Locationwp>();

            for (int a = 0; a < WayPointData.Commands.Rows.Count - 0; a++)
            {
                var temp = DataViewtoLocationwp(a);

                commands.Add(temp);
            }

            return commands;
        }
        Locationwp DataViewtoLocationwp(int a)
        {
            try
            {
                Locationwp temp = new Locationwp();
                if (WayPointData.Commands.Rows[a].Cells[WayPointData.Command.Index].Value.ToString().Contains("UNKNOWN"))
                {
                    temp.id = (ushort)WayPointData.Commands.Rows[a].Cells[WayPointData.Command.Index].Tag;
                }
                else
                {
                    temp.id =
                        (ushort)
                                Enum.Parse(typeof(MAVLink.MAV_CMD),
                                    WayPointData.Commands.Rows[a].Cells[WayPointData.Command.Index].Value.ToString(),
                                    false);
                }
                //temp.p1 = float.Parse(Commands.Rows[a].Cells[Param1.Index].Value.ToString());

                temp.alt =
                    (float)
                        (double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Alt.Index].Value.ToString()) / MissionPlanner.CurrentState.multiplierdist);
                temp.lat = (double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lat.Index].Value.ToString()));
                temp.lng = (double.Parse(WayPointData.Commands.Rows[a].Cells[WayPointData.Lon.Index].Value.ToString()));

                /*temp.p2 = (float)(double.Parse(Commands.Rows[a].Cells[Param2.Index].Value.ToString()));
                temp.p3 = (float)(double.Parse(Commands.Rows[a].Cells[Param3.Index].Value.ToString()));
                temp.p4 = (float)(double.Parse(Commands.Rows[a].Cells[Param4.Index].Value.ToString()));

                temp.Tag = Commands.Rows[a].Cells[TagData.Index].Value;*/

                return temp;
            }
            catch (Exception ex)
            {
                throw new FormatException("Invalid number on row " + (a + 1).ToString(), ex);
            }
        }
        void updateCMDParams()
        {
            cmdParamNames = readCMDXML();

            List<string> cmds = new List<string>();

            foreach (string item in cmdParamNames.Keys)
            {
                cmds.Add(item);
            }

            cmds.Add("UNKNOWN");

            WayPointData.Command.DataSource = cmds;
        }
        Dictionary<string, string[]> readCMDXML()
        {
            Dictionary<string, string[]> cmd = new Dictionary<string, string[]>();

            // do lang stuff here

            string file = Settings.GetRunningDirectory() + "mavcmd.xml";

            if (!File.Exists(file))
            {
                CustomMessageBox.Show("Missing mavcmd.xml file");
                return cmd;
            }

            log.Info("Reading MAV_CMD for " + MainForm.comPort.MAV.cs.firmware);

            using (XmlReader reader = XmlReader.Create(file))
            {
                reader.Read();
                reader.ReadStartElement("CMD");
                if (MainForm.comPort.MAV.cs.firmware == MainForm.Firmwares.ArduPlane ||
                    MainForm.comPort.MAV.cs.firmware == MainForm.Firmwares.Ateryx)
                {
                    reader.ReadToFollowing("APM");
                }
                else if (MainForm.comPort.MAV.cs.firmware == MainForm.Firmwares.ArduRover)
                {
                    reader.ReadToFollowing("APRover");
                }
                else
                {
                    reader.ReadToFollowing("AC2");
                }

                XmlReader inner = reader.ReadSubtree();

                inner.Read();

                inner.MoveToElement();

                inner.Read();

                while (inner.Read())
                {
                    inner.MoveToElement();
                    if (inner.IsStartElement())
                    {
                        string cmdname = inner.Name;
                        string[] cmdarray = new string[7];
                        int b = 0;

                        XmlReader inner2 = inner.ReadSubtree();

                        inner2.Read();

                        while (inner2.Read())
                        {
                            inner2.MoveToElement();
                            if (inner2.IsStartElement())
                            {
                                cmdarray[b] = inner2.ReadString();
                                b++;
                            }
                        }

                        cmd[cmdname] = cmdarray;
                    }
                }
            }

            return cmd;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedrow = WayPointData.Commands.Rows.Add();  //新增DataGridView 資料列
            DataGridViewTextBoxCell cell;
            if (WayPointData.Commands.Columns[WayPointData.Lat.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][2] /*"Lat"*/))
            {
                cell = WayPointData.Commands.Rows[selectedrow].Cells[WayPointData.Lat.Index] as DataGridViewTextBoxCell;
                cell.Value = (123.1111).ToString();   //將來源清單Lat資料新增至DataGridView Lat欄位，list[0] 為 home 點所以需加1才會是第一個航點 
                cell.DataGridView.EndEdit();
            }
        }

    }
    
}
