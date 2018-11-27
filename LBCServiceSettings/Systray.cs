using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using LBCServiceSettings.Properties;
using Microsoft.Win32;

namespace LBCServiceSettings
{
    class Systray
    {
        public static NotifyIcon trayIcon;

        public static void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon {Icon = Resources.icon2};
            var cm = new ContextMenu();
            cm.MenuItems.Add("Show Settings", ShowForm);
            cm.MenuItems.Add("-");
            cm.MenuItems.Add("Enable Backlight", EnableClicked);
            cm.MenuItems.Add("Disable Backlight", DisableClicked);
            cm.MenuItems.Add("-");
            cm.MenuItems.Add("Exit", Exit);
            trayIcon.ContextMenu = cm;
            trayIcon.Visible = true;
        }

        private static void EnableClicked(object sender, EventArgs e)
        {
            // Send the message to enable the backlight using a thread
            var t = new Thread(() => SettingsForm.LBCServiceUpdateNotify("LBC-EnableBacklight"));
            t.Start();
            IdleTimerControl.BackLightOn = true;
        }

        private static void DisableClicked(object sender, EventArgs e)
        {
            // Send the message to disable the backlight using a thread
            var t = new Thread(() => SettingsForm.LBCServiceUpdateNotify("LBC-DisableBacklight"));
            t.Start();
            IdleTimerControl.BackLightOn = false;
        }

        private static void ShowForm(object sender, EventArgs e)
        {
            SettingsForm.ShowSettingsForm();
        }
        private static void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
