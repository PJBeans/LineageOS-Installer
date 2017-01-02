﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LineageOS_Installer
{
    class Adb
    {
        string currentDir = Directory.GetCurrentDirectory().ToString();
        string pullOutput;

        private void ExtractResource(string resource, string path)
        {
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            Stream stream = GetType().Assembly.GetManifestResourceStream(resource);
            byte[] bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            File.WriteAllBytes(path, bytes);
        }

        public void Start()
        {
            // Extract adb and libraries
            ExtractResource("LineageOS_Installer.adb.adb.exe", currentDir + "\\adb.exe");
            ExtractResource("LineageOS_Installer.adb.AdbWinApi.dll", currentDir + "\\AdbWinApi.dll");
            ExtractResource("LineageOS_Installer.adb.AdbWinUsbApi.dll", currentDir + "\\AdbWinUsbApi.dll");

            // Start ADB server
            System.Diagnostics.Process startAdb = new System.Diagnostics.Process();
            startAdb.StartInfo.FileName = "adb.exe";
            startAdb.StartInfo.Arguments = "start-server";
            startAdb.StartInfo.CreateNoWindow = true;
            startAdb.StartInfo.UseShellExecute = false;
            startAdb.StartInfo.RedirectStandardOutput = true;
            startAdb.Start();
            startAdb.WaitForExit();
        }

        public void Stop()
        {
            // Stop ADB server
            System.Diagnostics.Process stopAdb = new System.Diagnostics.Process();
            stopAdb.StartInfo.FileName = "adb.exe";
            stopAdb.StartInfo.Arguments = "kill-server";
            stopAdb.StartInfo.CreateNoWindow = true;
            stopAdb.StartInfo.UseShellExecute = false;
            stopAdb.StartInfo.RedirectStandardOutput = true;
            stopAdb.Start();
            stopAdb.WaitForExit();

            // Kill process
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("adb.exe"))
            {
                process.Kill();
            }

            // Delete leftover files
            /* This functionality does not work yet */
            //File.SetAttributes(currentDir + "\\adb.exe", FileAttributes.Normal);
            //File.SetAttributes(currentDir + "\\AdbWinApi.dll", FileAttributes.Normal);
            //File.SetAttributes(currentDir + "\\AdbWinUsbApi.dll", FileAttributes.Normal);
            //Console.WriteLine(currentDir + "\\adb.exe");
            //File.Delete(currentDir + "\\adb.exe");
            //File.Delete(currentDir + "\\AdbWinApi.dll");
            //File.Delete(currentDir + "\\AdbWinUsbApi.dll");
        }

        public void Pull(string file)
        {
            System.Diagnostics.Process pullAdb = new System.Diagnostics.Process();
            pullAdb.StartInfo.FileName = "adb.exe";
            pullAdb.StartInfo.Arguments = "pull " + file;
            pullAdb.StartInfo.CreateNoWindow = true;
            pullAdb.StartInfo.UseShellExecute = false;
            pullAdb.StartInfo.RedirectStandardOutput = true;
            pullAdb.Start();
            pullAdb.WaitForExit();
            pullOutput = pullAdb.StandardOutput.ReadToEnd().Replace("\r\n", null);
        }
    }
}