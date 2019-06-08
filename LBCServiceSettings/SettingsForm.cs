using System;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using LBCService.Common;
using LBCService.Common.Messages;
using LBCServiceSettings.Messages;
using TinyMessenger;

namespace LBCServiceSettings
{
    public partial class SettingsForm : Form
    {
        private readonly ITinyMessengerHub _hub;
        private readonly SysTray _sysTray;
        private readonly IdleTimerControl _idleTimer;
        private readonly SynchronizationContext _ctx;

        public SettingsForm(ITinyMessengerHub hub, SysTray sysTray, IdleTimerControl idleTimer)
        {
            _hub = hub;
            _sysTray = sysTray;
            _idleTimer = idleTimer;

            InitializeComponent();
            _ctx = SynchronizationContext.Current;
            try
            {
                var sc = new ServiceController("LenovoBacklightControl");
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    //Do nothing, this is to allow the app to catch the exception if the service is not present
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not find service. Please install.", "Service Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }
            
            _hub.Subscribe<OnConfigLoadedMessage>( x => ExecuteOnUi(OnConfigLoaded, x));
            _hub.Subscribe<ServiceStateMessage>(x => ExecuteOnUi(CheckService, x));
            _hub.Subscribe<FormOpenRequestMessage>(x => ExecuteOnUi(ShowSettingsForm));
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
            _sysTray.IsVisible = true;
            _hub.Publish(new ConfigReloadRequestMessage(this));
        }

        private void OnConfigLoaded(OnConfigLoadedMessage message)
        {
            var configData = message.Data;
            enableDebugLoggingCheck.Checked = configData.EnableDebugLog;
            wakeStateCheck.Checked = configData.SaveBacklightState;
            keyboardCorePathText.Text = configData.KeyboardCorePath;
            timeoutUpDown.Value = configData.TimeoutPreference;
            radioLow.Checked = configData.LightLevel == LightLevel.Low;
            radioHigh.Checked = configData.LightLevel == LightLevel.High;
            var fileInfo = new FileInfo(configData.KeyboardCorePath);
            var kbCoreParent = fileInfo.DirectoryName;
            openFileDialog.InitialDirectory = kbCoreParent;
            openFileDialog.FileName = fileInfo.Name;
            if (configData.TimeoutPreference > 0) _idleTimer.SetTimer(configData.TimeoutPreference);
            else _idleTimer.StopTimer();
        }

        public void ShowSettingsForm()
        {
            Show();
            ShowInTaskbar = true;
            _hub.Publish(new FormOpenedMessage(this));
        }

        public void HideSettingsForm()
        {
            Hide();
            ShowInTaskbar = false;
            _hub.Publish(new FormClosingMessage(this));
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
                var configData = new ConfigData
                {
                    KeyboardCorePath = keyboardCorePathText.Text,
                    LightLevel = radioLow.Checked ? LightLevel.Low : LightLevel.High,
                    TimeoutPreference = (int) timeoutUpDown.Value,
                    EnableDebugLog = enableDebugLoggingCheck.Checked,
                    SaveBacklightState = wakeStateCheck.Checked
                };
                _hub.PublishAsync(new ConfigSaveRequestMessage(this, configData), _ =>
                {
                    // Send message to the service to reload the backlight value
                    _hub.PublishAsync(new SendStatusRequestMessage(this, Status.UpdateConfig));

                    if (configData.TimeoutPreference > 0)
                    {
                        _idleTimer.SetTimer(configData.TimeoutPreference);
                    }
                    else
                    {
                        _idleTimer.StopTimer();
                    }
                });
            }
            else
            {
                MessageBox.Show("Path to Keyboard_Core.dll is invalid. File not found.", "DLL path error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _hub.Publish(new FormClosingMessage(this));
            _sysTray.IsVisible = false;
            base.OnFormClosed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.WindowsShutDown:
                case CloseReason.TaskManagerClosing:
                case CloseReason.ApplicationExitCall:

                    return;
            }
            e.Cancel = true;
            //assuming you want the close-button to only hide the form, 
            //and are overriding the form's OnFormClosing method:
            HideSettingsForm();
        }

        /// <summary>
        /// Check service status thread. Updates the status on the form, if it is being shown.
        /// </summary>
        private void CheckService(ServiceStateMessage message)
        {
            if (!message.Status.HasValue)
            {
                statusTextBox.Text = "Status UNKNOWN";
                statusTextBox.ForeColor = Color.Red;
            }
            else
            {
                switch (message.Status.Value)
                {
                    case ServiceControllerStatus.Running:
                        statusTextBox.Text = "Running";
                        statusTextBox.ForeColor = Color.LawnGreen;
                        break;
                    case ServiceControllerStatus.Stopped:
                        statusTextBox.Text = "Stopped";
                        statusTextBox.ForeColor = Color.Red;
                        break;
                    case ServiceControllerStatus.Paused:
                        statusTextBox.Text = "Paused";
                        statusTextBox.ForeColor = Color.Orange;
                        break;
                    case ServiceControllerStatus.StopPending:
                        statusTextBox.Text = "Stopping..";
                        statusTextBox.ForeColor = Color.Yellow;
                        break;
                    case ServiceControllerStatus.StartPending:
                        statusTextBox.Text = "Starting...";
                        statusTextBox.ForeColor = Color.Yellow;
                        break;
                    case ServiceControllerStatus.ContinuePending:
                        break;
                    case ServiceControllerStatus.PausePending:
                        break;
                    default:
                        statusTextBox.Text = "Status UNKNOWN";
                        statusTextBox.ForeColor = Color.Red;
                        break;
                }
            }
        }

        private void ExecuteOnUi<T>(Action<T> action, T parameter)
        {
            _ctx.Send(x => action((T)x), parameter);
        }

        private void ExecuteOnUi(Action action)
        {
            _ctx.Send(x => action(), null);
        }
    }
}