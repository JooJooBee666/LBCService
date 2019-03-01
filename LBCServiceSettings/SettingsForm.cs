using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace LBCServiceSettings
{
    public partial class SettingsForm : Form
    {
        private static SettingsForm Callback;
        private static XMLConfigMethods.ConfigData configData;
        private static Thread CheckServiceThread;
        private static bool StopThread;
        private static SynchronizationContext ctx;
        private static Thread NamedPipeThread;

        public SettingsForm()
        {
            InitializeComponent();
            ctx = SynchronizationContext.Current;
            try
            {
                var sc = new ServiceController("LenovoBacklightControl");
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    //Do nothing, this is to allow the app to catch the exception if the service is not present
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not find service.  Please install.", "Service Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            openFileDialog.Title = "Browse for Keyboard_Core.dll";
            openFileDialog.DefaultExt = "dll";
            openFileDialog.CheckFileExists = true;
            Callback = this;
            Callback.ShowInTaskbar = false;
            Callback.Opacity = 0;
            NamedPipeThread = new Thread(NamedPipeServer.EnableNamedPipeServer);
            NamedPipeThread.Start();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            Systray.InitializeTrayIcon();
            LoadConfig();
            var file_info = new FileInfo(configData.Keyboard_Core_Path);
            var kbCoreParent = file_info.DirectoryName;
            openFileDialog.InitialDirectory = kbCoreParent;
            IdleTimerControl.SetTimer(configData.Timeout_Preference);
            enableDebugLoggingCheck.Checked = configData.Enable_Debug_Log;
            wakeStateCheck.Checked = configData.Save_Backlight_State;
        }

        public static void ShowSettingsForm()
        {
            Callback.Show();
            Callback.ShowInTaskbar = true;
            Callback.Opacity = 100;
            StopThread = false;
            CheckServiceThread = new Thread(CheckService);
            CheckServiceThread.Start();
        }

        public static void HideSettingsForm()
        {
            Callback.Hide();
            Callback.ShowInTaskbar = false;
            Callback.Opacity = 0;
            StopThread = true;
            CheckServiceThread.Abort();
        }
        public static void CloseSettingsForm()
        {
            Systray.trayIcon.Visible = false;
            Callback.Close();   
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                keyboardCorePathText.Text = openFileDialog.FileName;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(keyboardCorePathText.Text))
            {
                if (XMLConfigMethods.SaveConfigXML(keyboardCorePathText.Text, radioLow.Checked ? 1 : 2,
                    (int)timeoutUpDown.Value, enableDebugLoggingCheck.Checked, wakeStateCheck.Checked))
                {
                    // Send message to the service to reload the backlight value
                    var t = new Thread(() => LBCServiceUpdateNotify("LBC-UpdateConfigData"));
                    t.Start();
                    configData.Timeout_Preference = (int)timeoutUpDown.Value;
                    configData.Light_Level = radioLow.Checked ? 1 : 2;
                    configData.Keyboard_Core_Path = keyboardCorePathText.Text;
                    configData.Enable_Debug_Log = enableDebugLoggingCheck.Checked;
                    configData.Save_Backlight_State = wakeStateCheck.Checked;
                    IdleTimerControl.UserTimeout = configData.Timeout_Preference;
                    IdleTimerControl.RestartTimer();
                }
            }
            else
            {
                MessageBox.Show("Path to Keyboard_Core.dll is invalid.  File not found.", "DLL path error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Send given message to the named pipe for the service
        /// </summary>
        /// <param name="message"></param>
        public static void  LBCServiceUpdateNotify(string message)
        {
            try
            {
                var client = new NamedPipeClientStream(".", "LenovoBacklightControlPipe", PipeDirection.Out);
                client.Connect();
                var writer = new StreamWriter(client);
                writer.WriteLine(message);
                writer.Flush();
                client.Dispose();
            }
            catch (Exception e)
            {                
                //Not implemented
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.TaskManagerClosing)
            {
                StopThread = true;
                return;
            }
            e.Cancel = true;
            //assuming you want the close-button to only hide the form, 
            //and are overriding the form's OnFormClosing method:
            NamedPipeServer.StopNamedPipe();
            NamedPipeThread.Join();
            HideSettingsForm();
        }

        private static void LoadConfig()
        {
            configData = XMLConfigMethods.ReadConfigXML();
            if (configData != null)
            {
                Callback.keyboardCorePathText.Text = configData.Keyboard_Core_Path;
                Callback.timeoutUpDown.Value = configData.Timeout_Preference;
                if (configData.Light_Level == 1)
                {
                    Callback.radioLow.Checked = true;
                    Callback.radioHigh.Checked = false;
                }
                else if (configData.Light_Level == 2)
                {
                    Callback.radioLow.Checked = false;
                    Callback.radioHigh.Checked = true;
                }
            }
        }

        
        /// <summary>
        /// Check service status thread.  Updates the status on the form, if it is being shown.
        /// </summary>
        private static void CheckService()
        {
            while (Callback.ShowInTaskbar && !StopThread)
            {
                try
                {
                    var sc = new ServiceController("LenovoBacklightControl");
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Running:
                            ctx.Send(delegate
                            {
                                Callback.statusTextBox.Text = "Running";
                                Callback.statusTextBox.ForeColor = Color.LawnGreen;
                            }, null);
                            break;
                        case ServiceControllerStatus.Stopped:
                            ctx.Send(delegate
                            {
                                Callback.statusTextBox.Text = "Stopped";
                                Callback.statusTextBox.ForeColor = Color.Red;
                            }, null);
                            break;
                        case ServiceControllerStatus.Paused:
                            ctx.Send(delegate
                            {
                                Callback.statusTextBox.Text = "Paused";
                                Callback.statusTextBox.ForeColor = Color.Orange;
                            }, null);
                            break;
                        case ServiceControllerStatus.StopPending:
                            ctx.Send(delegate
                            {
                                Callback.statusTextBox.Text = "Stopping..";
                                Callback.statusTextBox.ForeColor = Color.Yellow;
                            }, null);
                            break;
                        case ServiceControllerStatus.StartPending:
                            ctx.Send(delegate
                            {
                                Callback.statusTextBox.Text = "Starting...";
                                Callback.statusTextBox.ForeColor = Color.Yellow;
                            }, null);
                            break;
                        case ServiceControllerStatus.ContinuePending:
                            break;
                        case ServiceControllerStatus.PausePending:
                            break;
                        default:
                            ctx.Send(delegate
                            {
                                Callback.statusTextBox.Text = "Status UNKNOWN";
                                Callback.statusTextBox.ForeColor = Color.Red;
                            }, null);
                            break;
                    }
                }
                catch (Exception e)
                {
                    ctx.Send(delegate
                    {
                        Callback.statusTextBox.Text = "Status UNKNOWN";
                        Callback.statusTextBox.ForeColor = Color.Red;
                    }, null);
                }
                Thread.Sleep(500);
            }
        }
    }
}
