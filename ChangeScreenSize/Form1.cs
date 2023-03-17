using Dapplo.Windows.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using Application = System.Windows.Forms.Application;
using Timer = System.Timers.Timer;

namespace ChangeScreenSize
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int ChangeDisplaySettings([In] ref DEVMODE lpDevMode, int dwFlags);

        Timer timer = new Timer();

        public enum DMDO
        {
            DEFAULT = 0,
            D90 = 1,
            D180 = 2,
            D270 = 3
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct DEVMODE
        {
            public const int DM_PELSWIDTH = 0x80000;
            public const int DM_PELSHEIGHT = 0x100000;
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;

            public int dmPositionX;
            public int dmPositionY;
            public DMDO dmDisplayOrientation;
            public int dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }


        private static void SetResolution(int w, int h)
        {
            long RetVal = 0;
            DEVMODE dm = new DEVMODE();

            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            dm.dmPelsWidth = w;
            dm.dmPelsHeight = h;
            dm.dmFields = DEVMODE.DM_PELSWIDTH | DEVMODE.DM_PELSHEIGHT;

            RetVal = ChangeDisplaySettings(ref dm, 0);
        }


        public Form1()
        {
            InitializeComponent();
            

            timer.Interval = 5000;
            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var primaryMonitorScale = (100 * Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth);

                var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\PerMonitorSettings", true);
                
                var monitors = key.GetSubKeyNames();
                
                RegistryKey auxKey = null;




                auxKey = key.OpenSubKey(monitors[0], true);
                if (Screen.AllScreens.Count() > 1)
                {
                    auxKey.SetValue("DpiValue", 0);
                }
                else
                {
                    auxKey.SetValue("DpiValue", -1);
                }
                

                SetResolution(1920, 1080);
                SetResolution(2560, 1440);

                //SetResolution(1280, 720);
                //SetResolution(1920, 1080);

            }
            catch (Exception ex)
            {
                File.AppendAllText("D:\\log.txt", "Exception thrown: " + ex.Message + ".\n");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            timer.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}