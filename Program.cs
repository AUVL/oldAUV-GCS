using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.Linq;
using MissionPlanner.Utilities;
using MissionPlanner;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MissionPlanner.Comms;
using MissionPlanner.Controls;



namespace AUV_GCS
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Program");

        public static DateTime starttime = DateTime.Now;

        public static string name { get; internal set; }

        public static bool WindowsStoreApp { get { return Application.ExecutablePath.Contains("WindowsApps"); } }

        public static Image Logo = null;
        public static Image IconFile = null;

        

        internal static Thread Thread;

        public static string[] args = new string[] { };
        public static Bitmap SplashBG = null;

        public static string[] names = new string[] { "VVVVZ" };
        public static bool MONO = false;
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
