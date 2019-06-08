using System;
using System.Windows.Forms;
using LBCService.Common;
using LBCService.Common.Messages;
using LBCServiceSettings.Messages;
using LBCServiceSettings.Properties;
using TinyMessenger;

namespace LBCServiceSettings
{
    public class SysTray : IDisposable
    {
        private readonly ITinyMessengerHub _hub;
        private readonly NotifyIcon _trayIcon;

        public bool IsVisible
        {
            get => _trayIcon.Visible;
            set => _trayIcon.Visible = value;
        }

        public SysTray(ITinyMessengerHub hub)
        {
            _hub = hub;

            _trayIcon = new NotifyIcon { Icon = Resources.icon2 };
            var cm = new ContextMenu();
            cm.MenuItems.Add("Show Settings", ShowForm);
            cm.MenuItems.Add("-");
            cm.MenuItems.Add("Enable Backlight", EnableClicked);
            cm.MenuItems.Add("Disable Backlight", DisableClicked);
            cm.MenuItems.Add("-");
            cm.MenuItems.Add("Exit", Exit);
            _trayIcon.DoubleClick += TrayIconOnDoubleClick;
            _trayIcon.ContextMenu = cm;
        }

        private void TrayIconOnDoubleClick(object sender, EventArgs e)
        {
            ShowForm(sender, e);
        }

        private void EnableClicked(object sender, EventArgs e)
        {
            // Send the message to enable the backlight using a thread
            _hub.PublishAsync(new SendStatusRequestMessage(this, Status.EnableBacklight));
        }

        private void DisableClicked(object sender, EventArgs e)
        {
            // Send the message to disable the backlight using a thread
            _hub.PublishAsync(new SendStatusRequestMessage(this, Status.DisableBacklight));
        }

        private void ShowForm(object sender, EventArgs e)
        {
            _hub.PublishAsync(new FormOpenRequestMessage(this));
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            IsVisible = false;
            Application.Exit();
        }

        public void Dispose()
        {
            if (_trayIcon != null)
            {
                _trayIcon.DoubleClick -= TrayIconOnDoubleClick;
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }
        }
    }
}