using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using FrogyCore.Models;

namespace FrogyCore.Utils
{
    class ProcessHelper
    {
        #region DLL import
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        #endregion

        public static Process GetWindowPID(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out int calcID);
            Process result = Process.GetProcessById(calcID);

            return result;
        }

        public static string GetProcessName(Process process, SoftwareType softwareType = SoftwareType.Unspecified)
        {
            if(softwareType == SoftwareType.Unspecified)
            {
                softwareType = GetSoftwareType(process);
            }

            string applicationName = "";
            if (softwareType == SoftwareType.UWP)
            {
                try
                {
                    List<IntPtr> allChildWindows = WindowHelper.GetAllChildHandles(process.MainWindowHandle);
                    foreach (IntPtr ptr in allChildWindows)
                    {
                        Process uwpProcess = GetWindowPID(ptr);
                        if (uwpProcess.MainModule.ModuleName != "ApplicationFrameHost.exe")
                        {
                            applicationName = AppxPackageHelper.GetAppDisplayNameFromProcess(uwpProcess);
                            if (string.IsNullOrEmpty(applicationName))
                                applicationName = WindowHelper.GetWindowTitle(ptr);
                            break;
                        }
                    }
                }
                catch { applicationName = null; }
            }
            else
            {
                applicationName = process.MainModule.FileVersionInfo.FileDescription;

                if (string.IsNullOrEmpty(applicationName))
                    applicationName = process.ProcessName;
            }

            return applicationName;
        }

        public static string GetProcessPath(Process process, SoftwareType softwareType = SoftwareType.Unspecified)
        {
            if (softwareType == SoftwareType.Unspecified)
            {
                softwareType = GetSoftwareType(process);
            }

            string applicationPath = "";
            if (softwareType == SoftwareType.UWP)
            {
                try
                {
                    List<IntPtr> allChildWindows = WindowHelper.GetAllChildHandles(process.MainWindowHandle);
                    foreach (IntPtr ptr in allChildWindows)
                    {
                        Process uwpProcess = GetWindowPID(ptr);
                        if (uwpProcess.MainModule.ModuleName != "ApplicationFrameHost.exe")
                            applicationPath = uwpProcess.MainModule.FileName;
                    }
                }
                catch { applicationPath = null; }
            }
            else
            {
                applicationPath = process.MainModule.FileName;
            }

            return applicationPath;
        }

        public static Bitmap GetProcessIcon(Process process, SoftwareType softwareType = SoftwareType.Unspecified)
        {
            if (softwareType == SoftwareType.Unspecified)
            {
                softwareType = GetSoftwareType(process);
            }

            Bitmap result = new Bitmap(Properties.Resources.Default.ToBitmap());
            if (softwareType == SoftwareType.UWP)
            {
                try
                {
                    List<IntPtr> allChildWindows = WindowHelper.GetAllChildHandles(process.MainWindowHandle);
                    foreach (IntPtr ptr in allChildWindows)
                    {
                        Process uwpProcess = GetWindowPID(ptr);
                        if (uwpProcess.MainModule.ModuleName != "ApplicationFrameHost.exe")
                        {
                            AppxPackage package = AppxPackage.FromProcess(uwpProcess);
                            result = new Bitmap(package.FindHighestScaleQualifiedImagePath(package.Logo));
                        }
                    }
                }
                catch { result = new Bitmap(Properties.Resources.Default.ToBitmap()); }
            }
            else
            {
                result = Icon.ExtractAssociatedIcon(GetProcessPath(process)).ToBitmap();
            }

            return result;
        }

        public static SoftwareType GetSoftwareType(Process process)
        {
            try
            {
                if(process.MainModule.FileVersionInfo.FileDescription == "Application Frame Host")
                {
                    return SoftwareType.UWP;
                }
                else
                {
                    return SoftwareType.Win32;
                }
            }
            catch
            {
                throw;
            }
        }

        public static bool CheckAuthority(Process process)
        {
            try
            {
                process.MainModule.FileVersionInfo.FileDescription.ToString();
            }
            catch(Exception ex)
            {
                if(ex.Message == "Access is denied")
                {
                    return false;
                }
            }
            return true;
        }
    }
}
