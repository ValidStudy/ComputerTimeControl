﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TimeControl.AppControl;

namespace TimeControl.Tools
{
    public static class TimeControlFile
    {
        public static readonly string BaseLocation = Environment.GetFolderPath
            (Environment.SpecialFolder.ApplicationData) + "\\TimeControl";
        //密码
        public static readonly string PassLocation = BaseLocation + "\\TCPass.txt";//获取密码位置
        //计时
        public static readonly string TimeFileDirectory = BaseLocation
            + "\\TCTimeData";
        //日志
        public static readonly string LogFile = BaseLocation + "\\Log.txt";
        //屏保
        public static readonly string WhiteAppLocation = BaseLocation + "\\WhiteApp.txt";//应用白名单保存
        public static readonly string TempTimeFile = BaseLocation + "\\Temp.txt";
        //自动关机
        public static readonly string ShutdownSpan = BaseLocation + "\\Shutdown.txt";

        public static void SaveToXML(List<App> apps)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(TimeFileDirectory);
            FileInfo[] files = directoryInfo.GetFiles();
            if (files.Length >= 10)
            {
                foreach (FileInfo file in files)
                    File.Delete(file.FullName);
            }
            using (StreamWriter sw = new StreamWriter(TimeFileDirectory +
                $"\\{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.xml"))
            {
                XmlSerializer xmlSerializer = new(typeof(List<AppInformation>));
                List<AppInformation> list = new List<AppInformation>();
                foreach (App app in apps)
                {
                    list.Add(app.appInformation);
                }
                xmlSerializer.Serialize(sw, list);
            }
        }

        public static List<App> ReadFromXML()
        {
            List<App> apps = new();
            DirectoryInfo directory = new DirectoryInfo(TimeFileDirectory);
            FileInfo latestFile = directory.GetFiles("*.xml")[0];
            //获取最新文件
            foreach (FileInfo file in directory.GetFiles("*.xml"))
            {
                if (latestFile.LastWriteTime < file.LastWriteTime)
                {
                    latestFile = file;
                }
            }
            using (StreamReader sr = new StreamReader(latestFile.FullName))
            {
                XmlSerializer xmlSerializer = new(typeof(List<AppInformation>));
                List<AppInformation> infos = (List<AppInformation>)xmlSerializer.Deserialize(sr);
                foreach (AppInformation information in infos)
                {
                    if (information.timeLimit != 0)
                        apps.Add(new LimitedApp(information));
                    else
                        apps.Add(new App(information));
                }
            }
            return apps;
        }
    }
}