using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using LelShotter.Models;
using LelShotter.Properties;
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

            var savePath = Environment.ExpandEnvironmentVariables(Settings.Default.SavePath);

            if (!Directory.Exists(savePath))
            {
                var dirCreated = Utility.CreateDirectory(savePath);
                if (dirCreated)
                {
                    Logger.Log(Level.Info, $"Created default saving directory at {savePath}");
                }
                else
                {
                    Settings.Default.SavePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\LelShotter");
                    Logger.Log(Level.Info, $"Setting {Settings.Default.SavePath} as default saving directory");
                }

            }

            _notifyIcon = new NotifyIcon
            {
                Icon = LelShotter.Properties.Resources.MyIcon,
                Visible = true
            };

            _hotKeys.Add(new HotKey(Key.W, KeyModifier.Shift | KeyModifier.Alt, (h, m) => { TakeScreenshot(ScreenshotMode.FullScreen); }));
            _hotKeys.Add(new HotKey(Key.S, KeyModifier.Shift | KeyModifier.Alt, (h, m) => { TakeScreenshot(ScreenshotMode.Selection); }));

            CreateContextMenu();
            LoadSettings();

            Logger.Log(Level.Info, "LelShotter started");
        }

        private void LoadSettings()
        {
            _upload = Settings.Default.UploadMode;
            _save = Settings.Default.SaveMode;
            _copy = Settings.Default.ClipboardMode;
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Full screen capture") {ShowShortcutKeys = true, ShortcutKeys = Keys.W | Keys.Shift | Keys.Alt},
                new ToolStripMenuItem("Select area to take") {ShowShortcutKeys = true, ShortcutKeys = Keys.S | Keys.Shift | Keys.Alt},
                new ToolStripSeparator(),
                new ToolStripMenuItem("Save to disk") { CheckOnClick = true, Checked = Settings.Default.SaveMode },
                new ToolStripMenuItem("Upload to Imgur") { CheckOnClick = true, Checked = Settings.Default.UploadMode },
                new ToolStripMenuItem("Copy to clipboard") { CheckOnClick = true, Checked = Settings.Default.ClipboardMode},
                new ToolStripSeparator(),
                new ToolStripMenuItem("Configuration"),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Exit") 
            });

            _notifyIcon.ContextMenuStrip.Items[0].Click += (s, e) => TakeScreenshot(ScreenshotMode.FullScreen);
            _notifyIcon.ContextMenuStrip.Items[1].Click += (s, e) => TakeScreenshot(ScreenshotMode.Selection);
            _notifyIcon.ContextMenuStrip.Items[3].Click += (s, e) => 
            {
                _save = !_save;
                Settings.Default.SaveMode = _save;
            };
            _notifyIcon.ContextMenuStrip.Items[4].Click += (s, e) => 
            {
                _upload = !_upload;
                Settings.Default.UploadMode = _upload;
            };
            _notifyIcon.ContextMenuStrip.Items[5].Click += (s, e) =>
            {
                _copy = !_copy;
                Settings.Default.ClipboardMode = _copy;
            };
            _notifyIcon.ContextMenuStrip.Items[7].Click += (s, e) =>
            {
                var conf = new Configuration();
                conf.ShowDialog();
            };
            _notifyIcon.ContextMenuStrip.Items[9].Click += (s, e) => ExitApplication();
        }

        private async void TakeScreenshot(ScreenshotMode mode)
        {
            Logger.Log(Level.Info, $"Taking screenshot with flags: upload={_upload}, save={_save}, copy={_copy}");
            var message = await new Screenshotter.Screenshotter(_upload, _save, _copy).TakeScreenshot(mode);
            Logger.Log(message.Level, message.Message);
            if (Settings.Default.DisplayPopups)
            {
                CompletePopup(message);
            }
        }

        private void CompletePopup(StatusMessage msg)
        {
            _notifyIcon.BalloonTipText = msg.Level.ToString();
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.BalloonTipTitle = msg.Message;
            _notifyIcon.ShowBalloonTip(500);
        }

        private void ExitApplication()
        {
            Logger.Log(Level.Info, "LelShotter closing");
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
