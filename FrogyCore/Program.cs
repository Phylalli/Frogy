using FrogyCore.Models;
using FrogyCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Text;
using System.Threading;

namespace FrogyCore
{
    class Program
    {
        private static Dictionary<DateTime, Day> timeLine = new Dictionary<DateTime, Day>();
        private static SoftwareInfo softwareInfo = new SoftwareInfo();
        private static CategoryInfo categoryInfo = new CategoryInfo();

        private static Thread interphoneThread;

        private static string workingDirectory;
        private static string guid;

        private static string storagePath
        {
            get
            {
                return Path.Combine(workingDirectory, guid + "\\");
            }
        }

        private static string iconPath
        {
            get
            {
                return Path.Combine(workingDirectory, "AppIcon\\");
            }
        }

        /// <summary>
        /// Entry of frogy core.
        /// </summary>
        /// <param name="args">
        /// [0] Working directory
        /// [1] Device GUID
        /// </param>
        static void Main(string[] args)
        {
            workingDirectory = args[0];
            guid = args[1];

            //Update device name
            DataHelper.WriteFile(storagePath, "device.config", Dns.GetHostName());

            load();

            interphoneThread = new Thread(interphone);
            interphoneThread.Start();

            do
            {
                loop_Tick();
                Thread.Sleep(1000);
            } while (interphoneThread.IsAlive);

            save();
        }

        private static void interphone()
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("FrogyCorePipe");
            pipeServer.WaitForConnection();

            StreamString ss = new StreamString(pipeServer);
            string clientMessage = "";
            do
            {
                clientMessage = ss.ReadString();
                if (clientMessage == "please")
                {
                    ss.WriteString(JsonConvert.SerializeObject(timeLine[DateTime.Today], Formatting.Indented));
                }
                Thread.Sleep(1000);
            } while (clientMessage != "exit");
        }

        private static int counter = 0;
        private static void loop_Tick()
        {
            TimeSpan now = new TimeSpan(
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second);

            DateTime today = DateTime.Today;

            //timeLine not containg today means date changed while running.
            if (!timeLine.ContainsKey(today))
            {
                DateTime yestday = today.AddDays(-1);
                if (timeLine[yestday].Slides.Count > 0)
                {
                    timeLine[yestday].Refresh(new TimeSpan(24, 0, 0));
                    save(yestday);
                }
                timeLine.Add(today, new Day());
            }

            if (DeviceHelper.DeviceStatus == DeviceStatus.Running)
            {
                IntPtr windowPtr = WindowHelper.GetFocusWindow();
                if (windowPtr != IntPtr.Zero)
                {
                    string windowTitle = WindowHelper.GetWindowTitle(windowPtr);
                    if (timeLine[today].Slides.Count == 0 || timeLine[today].LatestSlide().WindowTitle != windowTitle)
                    {
                        if (timeLine[today].Slides.Count != 0)
                        {
                            timeLine[today].Refresh(now);
                        }

                        Process process = ProcessHelper.GetWindowPID(windowPtr);
                        if (ProcessHelper.CheckAuthority(process))
                        {
                            SoftwareType softwareType = ProcessHelper.GetSoftwareType(process);

                            string iconBase64 = DataHelper.ImgToBase64String(ProcessHelper.GetProcessIcon(process, softwareType));
                            string iconMD5 = DataHelper.MD5Crypto(iconBase64);
                            saveIcon(iconBase64, iconMD5);

                            string softwareID = softwareInfo.MatchSoftwareID(
                                ProcessHelper.GetProcessName(process, softwareType),
                                ProcessHelper.GetProcessPath(process, softwareType),
                                iconMD5,
                                softwareType);
                            saveSoftwareInfo();

                            timeLine[today].Add(new Slide(softwareID, windowTitle, DeviceStatus.Running, now, now));
                        }
                        else
                        {
                            timeLine[today].Add(new Slide("", windowTitle, DeviceStatus.InsufficientPermission, now, now));
                        }
                    }
                    else
                    {
                        timeLine[today].Refresh(now);
                    }
                }
                else
                {
                    if (timeLine[today].Slides.Count > 0 && timeLine[today].LatestSlide().DeviceStatus == DeviceStatus.BadUser)
                    {
                        timeLine[today].Refresh(now);
                    }
                    else
                    {
                        timeLine[today].Add(new Slide("", "", DeviceStatus.BadUser, now, now));
                    }
                }
            }
            else
            {
                if (timeLine[today].Slides.Count > 0 && timeLine[today].LatestSlide().DeviceStatus == DeviceStatus.DeviceLocked)
                {
                    timeLine[today].Refresh(now);
                }
                else
                {
                    timeLine[today].Add(new Slide("", "", DeviceStatus.DeviceLocked, now, now));
                }
            }

            if (counter >= 60)
            {
                counter = 0;
                save(today);
            }

            counter++;
        }

        private static void saveIcon(string iconBase64, string iconMD5)
        {
            Directory.CreateDirectory(iconPath);
            if (!File.Exists(Path.Combine(iconPath, iconMD5 + ".dat")))
            {
                DataHelper.WriteFile(iconPath, iconMD5 + ".dat", iconBase64);
            }
        }

        private static void saveSoftwareInfo()
        {
            string softwareInfoContent = JsonConvert.SerializeObject(softwareInfo, Formatting.Indented);
            DataHelper.WriteFile(storagePath, "SoftwareInfo.json", softwareInfoContent);
        }

        private static void save(DateTime dateTime)
        {
            string dayLocation = Path.Combine(
                storagePath,
                dateTime.Year.ToString() + "\\",
                dateTime.Month.ToString() + "\\");
            string dayContent = JsonConvert.SerializeObject(timeLine[dateTime], Formatting.Indented);
            DataHelper.WriteFile(dayLocation, dateTime.ToString("yyyyMMdd") + ".json", dayContent);

            string categoryInfoContent = JsonConvert.SerializeObject(categoryInfo, Formatting.Indented);
            DataHelper.WriteFile(storagePath, "CategoryInfo.json", categoryInfoContent);
        }

        private static void save()
        {
            DateTime today = DateTime.Today;
            foreach(DateTime dateTime in timeLine.Keys)
            {
                if(dateTime == today)
                {
                    save(dateTime);
                    return;
                }
            }
            save(today.AddDays(-1));
        }

        private static void load()
        {
            TimeSpan now = new TimeSpan(
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second);

            DateTime today = DateTime.Today;
            string dayPath = Path.Combine(
                storagePath,
                today.Year.ToString() + "\\",
                today.Month.ToString() + "\\",
                today.ToString("yyyyMMdd") + ".json");
            if (File.Exists(dayPath))
            {
                string json = DataHelper.ReadFile(dayPath);

                try
                {
                    timeLine.Add(today, JsonConvert.DeserializeObject<Day>(json));
                    if (timeLine[today].Slides.Count > 0)
                    {
                        timeLine[today].Add(
                            new Slide(
                                "",
                                "",
                                DeviceStatus.SoftwareOffline,
                                timeLine[today].LatestSlide().EndTime,
                                now));
                    }
                }
                catch (JsonReaderException)
                {
                    timeLine.Add(today, new Day());
                }
            }
            else
            {
                timeLine.Add(today, new Day());
            }

            string softwareInfoPath = Path.Combine(storagePath, "SoftwareInfo.json");
            if (File.Exists(softwareInfoPath))
            {
                string json = DataHelper.ReadFile(softwareInfoPath);

                try
                {
                    softwareInfo = JsonConvert.DeserializeObject<SoftwareInfo>(json);
                }
                catch { }
            }

            string categoryInfoPath = Path.Combine(storagePath, "CategoryInfo.json");
            if (File.Exists(categoryInfoPath))
            {
                string json = DataHelper.ReadFile(categoryInfoPath);

                try
                {
                    categoryInfo = JsonConvert.DeserializeObject<CategoryInfo>(json);
                }
                catch { }
            }
        }
    }

}
