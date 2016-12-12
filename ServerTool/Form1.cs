using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Timers;


namespace ServerTool
{
    public partial class Form1 : Form
    {
        private static System.Timers.Timer aTimer = null;
        TextWriter _writer = null;

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GenerateConsoleCtrlEvent(ConsoleCtrlEvent dwCtrlEvent, int dwProcessGroupId);
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public enum ConsoleCtrlEvent
        {
            CTRL_C = 0,
            CTRL_BREAK = 1,
            CTRL_CLOSE = 2,
            CTRL_LOGOFF = 5,
            CTRL_SHUTDOWN = 6
        }        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _writer = new TextBoxStreamWriter(textBox2);
            Console.SetOut(_writer);
            Console.WriteLine("Welcome to the Warframe DS Tool\n");
            checkTitle();
        }

        public static void ExecuteCommandSync(object command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardOutput = false;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = false;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        public static void Wait(int ms)
        {
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < ms)
                Application.DoEvents();

        }

        public static Process WinGetHandle(string wName)
        {
            Process a = null;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(wName))
                {
                    a = pList;
                    break;
                }
            }
            return a;
            
        }

      

        private void checkTitle()
        {
                Process title = WinGetHandle("Retail Windows");
                if (title != null)
                {
                    if (title.MainWindowTitle != "")
                    {
                        label2.Text = title.MainWindowTitle;
                    }
                    else
                    {
                        label2.Text = "Can't get Window handle";
                    }
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkTitle();
        }

        public static void killRestart()
        {
            Process p = WinGetHandle("Retail Windows");

            if (p != null)
            {
                p.Kill();
                Form1.Wait(3000);
                //string localdir = Environment.GetEnvironmentVariable("LocalAppData");
                // string workdir = Path.Combine(localdir, "\\Warframe\\Downloaded\\Public\\Tools\\");
                //MessageBox.Show(workdir);
                //string cmd = "C:/Users/user/AppData/Local/Warframe/Downloaded/Public/Tools/Launcher.exe -headless -dedicated";
                string cmd = "WINEARCH=win32 WINEPREFIX=/games/Warframe wine /games/Downloaded/Public/Warframe.exe -fullscreen:0 -dx10:0 -dx11:0 -threadedworker:1 -cluster:public -language:en -allowmultiple -log:DedicatedServer.log -applet:/Lotus/Types/Game/DedicatedServer /Lotus/Types/GameRules/DefaultDedicatedServerSettings -override:{\"missionId\":CTF_Title}";
                ExecuteCommandSync(cmd);
            }
        }

        public static void CheckRestart()
        {

            Process title = WinGetHandle("Retail Windows");
            String t = "";
            if (title != null)
            {
                if (title.MainWindowTitle != "")
                {
                    Console.WriteLine("[+] Checking that no players are on");
                    if (title.MainWindowTitle.Contains("0 player(s)"))
                    {
                        Console.WriteLine("[+] No players found, restarting...");
                        killRestart();
                    } else
                    {
                        Console.WriteLine("[!] Players online, not restarting this cycle");
                    }
                }
            }                   
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (aTimer != null)
            {
                if (aTimer.Enabled == true)
                {
                    Console.WriteLine("[+] Disabling autorestart.");
                    aTimer.Enabled = false;
                }
            }
            else
            {
                Console.WriteLine("[+] Starting auto restart loop to execute every {0} hours", Convert.ToInt32(textBox1.Text));

                aTimer = new System.Timers.Timer(Convert.ToInt32(textBox1.Text) * 60 * 1000); //one hour in milliseconds
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                button2.Text = "Stop";
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Do the stuff you want to be done every hour;
            Console.WriteLine("[+] Restarting Server");
            Form1.CheckRestart();
            
        }



    }
}
