using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LBCServiceSettings
{
    public partial class SettingsForm : Form
    {
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
   
        }

        public static int IdleTime() //In seconds
        {
            LASTINPUTINFO lastinputinfo = new LASTINPUTINFO();
            lastinputinfo.cbSize = (uint) Marshal.SizeOf(lastinputinfo);
            GetLastInputInfo(ref lastinputinfo);
            return (int) ((((Environment.TickCount & int.MaxValue) - (lastinputinfo.dwTime & int.MaxValue)) & int.MaxValue) / 1000);
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
        /// <summary>
        /// Get the Last input time in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }
            return lastInPut.dwTime;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnableBacklight();
        }

        public static void EnableBacklight()
        {
            var client = new NamedPipeClientStream("LenovoBacklightControlPipe");
            client.Connect();
            var writer = new StreamWriter(client);
            //Thread.Sleep(5000);
            var idleTime = IdleTime();
            writer.WriteLine("LBC-EnableBacklight");
            writer.Flush();
            client.Dispose();
        }

        public void DisableBacklight()
        {
            var client = new NamedPipeClientStream("LenovoBacklightControlPipe");
            client.Connect();
            var writer = new StreamWriter(client);
            //Thread.Sleep(5000);
            var idleTime = IdleTime();
            writer.WriteLine("LBC-DisableBacklight");
            writer.Flush();
            client.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DisableBacklight();
        }
    }
}
