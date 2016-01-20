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
        static List<Dictionary<char, string>> programs = new List<Dictionary<char, string>>();
        static double monitorWidth = SystemParameters.PrimaryScreenWidth;
        static double monitorHeight = SystemParameters.PrimaryScreenHeight;
        static double width = 0;
        static double height = 0;
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
            LoadConfig();
            LoadPrograms(File.ReadAllLines("programs.txt"));
            args = Environment.GetCommandLineArgs();    
            if (args.Length > 1)
            {
                for (int i = 0; i < programs.Count; i++)
                {
                    if (programs[i]['n'] == args[1])
                    {
                        if (args.Length > 2)
                        {
                            foreach (string item in args[2].Split(';'))
                            {
                                programs[i][Convert.ToChar(item.Split('=')[0])] = item.Split('=')[1];
                            }
                        }
                        Process startProg = new Process();
                        startProg.StartInfo.FileName = '"' + programs[i]['p'] + '"';
                        for (int j = 0; j < programs[i]['p'].Split('\\').Length - 1; j++)
                        {
                            startProg.StartInfo.WorkingDirectory += programs[i]['p'].Split('\\')[j] + "\\";
                        }
                        if (programs[i]['g'] != "")
                        {
                            startProg.StartInfo.Arguments = '"' + programs[i]['g'] + '"';
                        }
                        startProg.Start();
                    }
                    else
                    {
                        programs.RemoveAt(i);
                        i--;
                    }
                }
            }
            while (true)
            {
                Thread.Sleep(1000);
                foreach (Process process in Process.GetProcesses())
                {
                    foreach (Dictionary<char, string> value in programs)
                    {
                        if (process.ProcessName == value['n'])
                        {
                            if (args.Length > 1)
                            {
                                Thread.Sleep(Convert.ToInt16(value['d']));
                            }
                            string[] shift = value['s'].Split(',');
                            if (Convert.ToDouble(value['a'].Split(',')[0]) / Convert.ToDouble(value['a'].Split(',')[1]) >= monitorWidth / monitorHeight)
                            {
                                width = monitorWidth;
                                height = monitorWidth / (Convert.ToDouble(value['a'].Split(',')[0]) / Convert.ToDouble(value['a'].Split(',')[1]));
                            }
                            else
                            {
                                width = monitorHeight * Convert.ToDouble(value['a'].Split(',')[0]) / Convert.ToDouble(value['a'].Split(',')[1]);
                                height = monitorHeight;
                            }
                            if (value['c'] == "true")
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
                                            MoveWindow(hand, shift);
                                            Console.WriteLine(hand + " " + stringBuilder.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MoveWindow(process.MainWindowHandle, shift);
                                Console.WriteLine(process.MainWindowHandle + " " + process.MainWindowTitle);
                            }
                            Console.WriteLine(value['n'] + " compat " + value['c'] + " " + Convert.ToString(Convert.ToDouble(value['a'].Split(',')[0]) / Convert.ToDouble(value['a'].Split(',')[1])) + " " + Convert.ToInt32(monitorWidth / 2 - width / 2) + " - " + shift[0] + " " + Convert.ToInt16(monitorHeight / 2 - height / 2) + " - " + shift[1] + " " + width + " + " + shift[2] + " " + height + " + " + shift[3] + "\n");
                        }
                    }
                }
                if (config["debug"] == "true")
                {
                    Console.ReadLine();
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
        static bool Check(string check, int type)
        {
            if (type == 0)
            {
                int checkInt;
                if (!int.TryParse(check, out checkInt))
                {
                    return true;
                }
            }
            else if (type == 1)
            {
                bool checkBool;
                if (!bool.TryParse(check, out checkBool))
                {
                    return true;
                }
            }
            else if (type == 2)
            {
                int checkInt;
                if (!int.TryParse(check, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out checkInt))
                {
                    return true;
                }
            }
            return false;
        }
        static void LoadConfig()
        {
            config = new Dictionary<string, string>();
            config.Add("debug", "false");
            config.Add("background", "false");
            config.Add("backgroundcolor", "000000");
            foreach (string line in File.ReadAllLines("config.txt"))
            {
                string[] temp = line.Split(' ');
                if (config.ContainsKey(temp[0]))
                {
                    config[temp[0]] = temp[1];
                }
            }
            if (Check(config["debug"], 1))
            {
                config["debug"] = "false";
                MessageBox.Show("Debug options ignored.");
            }
            if (Check(config["background"], 1))
            {
                config["background"] = "False";
                MessageBox.Show("Background options ignored.");
            }
            if (Check(config["backgroundcolor"], 2))
            {
                config["backgroundcolor"] = "000000";
                MessageBox.Show("Background color options ignored.");
            }
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                config["debug"] = "false";
            }
            if (config["debug"] == "false")
            {
                ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 0);
            }
        }
        static void LoadPrograms(string[] progConf)
        {
            for (int i = 0; i < progConf.Length; i++)
            {
                programs.Add(new Dictionary<char, string> { { 'a', Convert.ToString(monitorWidth) + ',' + Convert.ToString(monitorHeight) }, { 'c', "false" }, { 'd', "0" }, { 'g', "" }, { 'n', "default" }, { 'p', "default" }, { 's', "0,0,0,0" } });
                string[] values = progConf[i].Split(';');
                for (int j = 0; j < values.Length; j++)
                {
                    if (values[j].Split('=').Length == 2)
                    {
                        if (programs[i].ContainsKey(Convert.ToChar(values[j].Split('=')[0])))
                        {
                            programs[i][Convert.ToChar(values[j].Split('=')[0])] = values[j].Split('=')[1];
                        }
                    }
                }
                if (programs[i]['a'].Split(',').Length < 2 || Check(programs[i]['a'].Split(',')[0], 0) || Check(programs[i]['a'].Split(',')[1], 0))
                {
                    programs[i]['a'] = Convert.ToString(monitorWidth) + ',' + Convert.ToString(monitorHeight);
                    MessageBox.Show("Aspect ratio options for " + values[0] + " ignored.");
                }
                if (Check(programs[i]['c'], 1))
                {
                    programs[i]['c'] = "false";
                    MessageBox.Show("Compatibility options for " + values[0] + " ignored.");
                }
                if (Check(programs[i]['d'], 0))
                {
                    programs[i]['d'] = "0";
                    MessageBox.Show("Compatibility options for " + values[0] + " ignored.");
                }
                if (programs[i]['s'].Split(',').Length < 4 || Check(programs[i]['s'].Split(',')[0], 0) || Check(programs[i]['s'].Split(',')[1], 0) || Check(programs[i]['s'].Split(',')[2], 0) || Check(programs[i]['s'].Split(',')[3], 0))
                {
                    programs[i]['s'] = "0,0,0,0";
                    MessageBox.Show("Border shift options for " + values[0] + " ignored.");
                }
            }
        }
        static void MoveWindow(IntPtr hand, string[] shift)
        {
            ShowWindow(hand, 9);
            SetWindowLong(hand, -16, 0x0008 | 0x00C);
            SetWindowPos(hand, 0, Convert.ToInt16(monitorWidth / 2 - width / 2 - Convert.ToInt16(shift[0])), Convert.ToInt16(monitorHeight / 2 - height / 2 - Convert.ToInt16(shift[1])), Convert.ToInt16(width) + Convert.ToInt16(shift[2]), Convert.ToInt16(height) + Convert.ToInt16(shift[3]), 0x0400);
            ShowWindow(hand, 5);
        }
    }
}