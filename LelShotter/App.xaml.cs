using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace LelShotter
{
    public partial class App
    {
        private NotifyIcon _notifyIcon;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<HotKey> _hotKeys = new List<HotKey>();
        private bool _upload;
        private bool _save;
        private bool _copy;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _notifyIcon = new NotifyIcon
            {
                Icon = LelShotter.Properties.Resources.MyIcon,
                Visible = true
            };
            _hotKeys.Add(new HotKey(Key.W, KeyModifier.Shift | KeyModifier.Alt, (h, m) => { TakeScreenshot(DataModels.ScreenshotMode.FullScreen); }));
            _hotKeys.Add(new HotKey(Key.S, KeyModifier.Shift | KeyModifier.Alt, (h, m) => { TakeScreenshot(DataModels.ScreenshotMode.Selection); }));
            CreateContextMenu();
            Logger.LogInfo("LelShotter started");
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            
            _notifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Full screen capture"),
                new ToolStripMenuItem("Select area to take"),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Save to desktop") { CheckOnClick = true },
                new ToolStripMenuItem("Upload to Imgur") { CheckOnClick = true },
                new ToolStripMenuItem("Copy to clipboard") { CheckOnClick = true },
                new ToolStripSeparator(),
                new ToolStripMenuItem("Exit") 
            });

            _notifyIcon.ContextMenuStrip.Items[0].Click += (s, e) => TakeScreenshot(DataModels.ScreenshotMode.FullScreen);
            _notifyIcon.ContextMenuStrip.Items[1].Click += (s, e) => TakeScreenshot(DataModels.ScreenshotMode.Selection);
            _notifyIcon.ContextMenuStrip.Items[3].Click += (s, e) => { _save = !_save; };
            _notifyIcon.ContextMenuStrip.Items[4].Click += (s, e) => { _upload = !_upload; };
            _notifyIcon.ContextMenuStrip.Items[5].Click += (s, e) => { _copy = !_copy; };
            _notifyIcon.ContextMenuStrip.Items[7].Click += (s, e) => ExitApplication();
        }

        private async void TakeScreenshot(DataModels.ScreenshotMode mode)
        {
            Logger.LogInfo($"Taking screenshot with flags: upload={_upload}, save={_save}, copy={_copy}");
            var message = await new Screenshotter(_upload, _save, _copy).TakeScreenshot(mode);
            Logger.Log(message.Level, message.Message);
            CompletePopup(message);
        }

        private void CompletePopup(DataModels.StatusMessage msg)
        {
            _notifyIcon.BalloonTipText = msg.Level.ToString();
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.BalloonTipTitle = msg.Message;
            _notifyIcon.ShowBalloonTip(500);
        }

        private void ExitApplication()
        {
            Logger.LogInfo("LelShotter closing");
            Logger.Dispose();
            foreach (var hotKey in _hotKeys)
            {
                hotKey.Unregister();
                hotKey.Dispose();
            }
            _notifyIcon.Dispose();
            _notifyIcon = null;
            Shutdown();
        }
    }
}
