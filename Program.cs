using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Text;
using System.IO;
using System;
namespace BoWind
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        static Dictionary<string, string> config = new Dictionary<string, string>();
        static Dictionary<double, Dictionary<string, Dictionary<char, string>>> programs = new Dictionary<double, Dictionary<string, Dictionary<char, string>>>();
        static void Main(string[] args)
        {
            if (!File.Exists("config.txt"))
            {
                using (FileStream fs = File.Create("config.txt"))
                {

                }
            }
            if (!File.Exists("programs.txt"))
            {
                using (FileStream fs = File.Create("programs.txt"))
                {

                }
            }
            loadFiles();
            string[] command;
            double width = 0;
            double height = 0;
            double monitorWidth = SystemParameters.PrimaryScreenWidth;
            double monitorHeight = SystemParameters.PrimaryScreenHeight;
            while (1 == 1)
            {
                Thread.Sleep(1000);
                foreach (Process process in Process.GetProcesses())
                {
                    foreach (KeyValuePair<double, Dictionary<string, Dictionary<char, string>>> item in programs)
                    {
                        foreach (KeyValuePair<string, Dictionary<char, string>> proc in item.Value)
                        {
                            if (process.ProcessName == proc.Key)
                            {
                                if (item.Key >= monitorWidth / monitorHeight)
                                {
                                    width = monitorWidth;
                                    height = monitorWidth / item.Key;
                                }
                                else
                                {
                                    width = monitorHeight * item.Key;
                                    height = monitorHeight;
                                }
                                if (proc.Value['c'] == "true")
                                {
                                    uint compareA;
                                    uint compareB;
                                    List<IntPtr> handles = new List<IntPtr>();
                                    handles.Add(process.MainWindowHandle);
                                    IntPtr previousHandle = process.MainWindowHandle;
                                    while (GetWindow(previousHandle, 3) != IntPtr.Zero)
                                    {
                                        handles.Add(GetWindow(previousHandle, 3));
                                        previousHandle = GetWindow(previousHandle, 3);
                                    }
                                    previousHandle = process.MainWindowHandle;
                                    while (GetWindow(previousHandle, 2) != IntPtr.Zero)
                                    {
                                        handles.Add(GetWindow(previousHandle, 2));
                                        previousHandle = GetWindow(previousHandle, 2);
                                    }
                                    foreach (IntPtr hand in handles)
                                    {
                                        GetWindowThreadProcessId(hand, out compareA);
                                        GetWindowThreadProcessId(process.MainWindowHandle, out compareB);
                                        if (compareA == compareB)
                                        {
                                            StringBuilder stringBuilder = new StringBuilder(1024);
                                            GetWindowText(hand, stringBuilder, stringBuilder.Capacity);
                                            if (stringBuilder.ToString() != "" && stringBuilder.ToString() != "Default IME" && stringBuilder.ToString() != "MSCTFIME UI")
                                            {
                                                SetWindowLong(hand, -16, 0x0008 | 0x00C);
                                                SetWindowPos(hand, 0, Convert.ToInt16(monitorWidth / 2 - width / 2), Convert.ToInt16(monitorHeight / 2 - height / 2) - Convert.ToInt16(proc.Value['b']), Convert.ToInt16(width), Convert.ToInt16(height) + Convert.ToInt16(proc.Value['b']), 0x0400);
                                                ShowWindow(hand, 5);
                                                if (config["debug"] == "true")
                                                {
                                                    Console.WriteLine(hand + " " + stringBuilder.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SetWindowLong(process.MainWindowHandle, -16, 0x0008 | 0x00C);
                                    SetWindowPos(process.MainWindowHandle, 0, Convert.ToInt16(monitorWidth / 2 - width / 2), Convert.ToInt16(monitorHeight / 2 - height / 2) - Convert.ToInt16(proc.Value['b']), Convert.ToInt16(width), Convert.ToInt16(height) + Convert.ToInt16(proc.Value['b']), 0x0400);
                                    ShowWindow(process.MainWindowHandle, 5);
                                    if (config["debug"] == "true")
                                    {
                                        Console.WriteLine(process.MainWindowHandle + " " + process.MainWindowTitle);
                                    }
                                }
                                if (config["debug"] == "true")
                                {
                                    Console.WriteLine(proc.Key + " compat "  + proc.Value['c'] + " " + item.Key + " " + Convert.ToInt32(monitorWidth / 2 - width / 2) + " " + Convert.ToInt16(monitorHeight / 2 - height / 2) + " - " + proc.Value['b'] + " " + width + " " + height + " + " + proc.Value['b'] + "\n");
                                }
                            }
                        }
                    }
                }
                if (config["debug"] == "true")
                {
                    command = Console.ReadLine().Split(' ');
                    if (command[0] == "edit")
                    {
                        Process edit = new Process();
                        edit.StartInfo.FileName = command[1] + ".txt";
                        edit.Start();
                    }
                    if (command[0] == "reload")
                    {
                        loadFiles();
                    }
                }
                else
                {
                    if (width != 0)
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }
        static void loadFiles()
        {
            int checkInt;
            bool checkBool;
            config = new Dictionary<string, string>();
            programs = new Dictionary<double, Dictionary<string, Dictionary<char, string>>>();
            config.Add("debug", "false");
            config.Add("background", "false");
            config.Add("backgroundcolor", "000000");
            foreach (string line in File.ReadAllLines("config.txt"))
            {
                if (line[0] == '/' && line[1] == '/' || line.Length == 0)
                {
                    break;
                }
                string[] temp = line.Split(' ');
                if (config.ContainsKey(temp[0]))
                {
                    config[temp[0]] = temp[1];
                }
            }
            if (!bool.TryParse(config["debug"], out checkBool))
            {
                config["debug"] = "false";
                MessageBox.Show("Debug options ignored.");
            }
            if (!bool.TryParse(config["background"], out checkBool))
            {
                config["background"] = "False";
                MessageBox.Show("Background options ignored.");
            }
            if (!int.TryParse(config["backgroundcolor"], NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out checkInt))
            {
                config["backgroundcolor"] = "000000";
                MessageBox.Show("Background color options ignored.");
            }
            if (config["debug"] == "false")
            {
                ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 0);
            }
            foreach (string line in File.ReadAllLines("programs.txt"))
            {
                if (line[0] == '/' && line[1] == '/' || line.Length == 0)
                {
                    break;
                }
                string[] temp = line.Split(':');
                programs.Add(Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1]), new Dictionary<string, Dictionary<char, string>>());
                for (int i = 2; i < temp.Length; i++)
                {
                    string[] values = temp[i].Split('.');
                    programs[Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1])].Add(values[0], new Dictionary<char, string>() { { 'b', "0" }, { 'c', "false" } });
                    for (int j = 1; j < values.Length; j++)
                    {
                        if (values[j].Split('/')[0].Length == 1)
                        {
                            if (programs[Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1])][values[0]].ContainsKey(Convert.ToChar(values[j].Split('/')[0])))
                            {
                                programs[Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1])][values[0]][Convert.ToChar(values[j].Split('/')[0])] = values[j].Split('/')[1];
                            }
                        }
                    }
                    if (!int.TryParse(programs[Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1])][values[0]]['b'], out checkInt))
                    {
                        programs[Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1])][values[0]]['b'] = "0";
                        MessageBox.Show("Border shift options for " + values[0] + " ignored.");
                    }
                    if (!bool.TryParse(programs[Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1])][values[0]]['c'], out checkBool))
                    {
                        programs[Convert.ToDouble(temp[0]) / Convert.ToDouble(temp[1])][values[0]]['c'] = "false";
                        MessageBox.Show("Compatibility options for " + values[0] + " ignored.");
                    }
                }
            }
        }
    }
}