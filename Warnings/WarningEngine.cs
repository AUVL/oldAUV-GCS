﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using AUV_GCS;

namespace MissionPlanner.Warnings
{
    public class WarningEngine
    {
        public static List<CustomWarning> warnings = new List<CustomWarning>();

        public static string warningconfigfile = "warnings.xml";

        static bool run = false;

        static WarningEngine()
        {
            try
            {
                LoadConfig();
            }
            catch
            {
                Console.WriteLine("Failed to read WArning config file " + warningconfigfile);
            }
        }

        public static void LoadConfig()
        {
            if (!File.Exists(warningconfigfile))
                return;

            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof (List<CustomWarning>),
                    new Type[] {typeof (CustomWarning)});

            using (StreamReader sr = new StreamReader(warningconfigfile))
            {
                warnings = (List<CustomWarning>) reader.Deserialize(sr);
            }
        }

        public static void SaveConfig()
        {
            // save config
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof (List<CustomWarning>),
                    new Type[] {typeof (CustomWarning)});

            using (StreamWriter sw = new StreamWriter(warningconfigfile))
            {
                lock (warnings)
                {
                    writer.Serialize(sw, warnings);
                }
            }
        }

        public static void Start()
        {
            if (run == false)
            {
                thisthread = new Thread(MainLoop);
                thisthread.Name = "Warning Engine";
                thisthread.IsBackground = true;
                thisthread.Start();
            }
        }

        public static void Stop()
        {
            run = false;
            if (thisthread != null && thisthread.IsAlive)
                thisthread.Join();
        }

        static Thread thisthread;

        public static void MainLoop()
        {
            run = true;
            while (run)
            {
                if (MainForm.comPort.BaseStream.IsOpen)
                {
                    try
                    {
                        lock (warnings)
                        {
                            foreach (var item in warnings)
                            {
                                // check primary condition
                                if (checkCond(item))
                                {
                                    if (MainForm.speechEnable)
                                    {
                                        while (!MainForm.speechEngine.IsReady)
                                            System.Threading.Thread.Sleep(10);

                                        MainForm.speechEngine.SpeakAsync(item.SayText());
                                    }

                                    MainForm.comPort.MAV.cs.messageHigh = item.SayText();
                                    MainForm.comPort.MAV.cs.messageHighTime = DateTime.Now;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        static bool checkCond(CustomWarning item)
        {
            // if there is a child go recursive
            if (item.Child != null)
            {
                if (item.CheckValue() && checkCond(item.Child))
                    return true;
            }
            else
            {
                // is no child then simple check
                if (item.CheckValue())
                    return true;
            }

            return false;
        }
    }
}