using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using LelShotter.Utils;
using LelShotter.Views;

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

            var savePath = LelShotter.Properties.Settings.Default.SavePath;
            if (!Directory.Exists(savePath))
            {
                var dirCreated = Utility.CreateDirectory(savePath);
                if (dirCreated)
                {
                    Logger.LogInfo($"Created default saving directory at {savePath}");
                }
                else
                {
                    Logger.LogInfo("Setting desktop as default saving directory");
                    LelShotter.Properties.Settings.Default.SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }

            }

            _notifyIcon = new NotifyIcon
            {
                Icon = LelShotter.Properties.Resources.MyIcon,
                Visible = true
            };

            _hotKeys.Add(new HotKey(Key.W, KeyModifier.Shift | KeyModifier.Alt, (h, m) => { TakeScreenshot(Models.ScreenshotMode.FullScreen); }));
            _hotKeys.Add(new HotKey(Key.S, KeyModifier.Shift | KeyModifier.Alt, (h, m) => { TakeScreenshot(Models.ScreenshotMode.Selection); }));
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
                new ToolStripMenuItem("Configuration"),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Exit") 
            });

            _notifyIcon.ContextMenuStrip.Items[0].Click += (s, e) => TakeScreenshot(Models.ScreenshotMode.FullScreen);
            _notifyIcon.ContextMenuStrip.Items[1].Click += (s, e) => TakeScreenshot(Models.ScreenshotMode.Selection);
            _notifyIcon.ContextMenuStrip.Items[3].Click += (s, e) => { _save = !_save; };
            _notifyIcon.ContextMenuStrip.Items[4].Click += (s, e) => { _upload = !_upload; };
            _notifyIcon.ContextMenuStrip.Items[5].Click += (s, e) => { _copy = !_copy; };
            _notifyIcon.ContextMenuStrip.Items[7].Click += (s, e) => new Configuration().Show();
            _notifyIcon.ContextMenuStrip.Items[9].Click += (s, e) => ExitApplication();
        }

        private async void TakeScreenshot(Models.ScreenshotMode mode)
        {
            Logger.LogInfo($"Taking screenshot with flags: upload={_upload}, save={_save}, copy={_copy}");
            var message = await new Screenshotter(_upload, _save, _copy).TakeScreenshot(mode);
            Logger.Log(message.Level, message.Message);
            CompletePopup(message);
        }

        private void CompletePopup(Models.StatusMessage msg)
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
