using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using LelShotter.Models;
using LelShotter.Utils;
using Microsoft.Win32;
using CheckBox = System.Windows.Controls.CheckBox;
using Settings = LelShotter.Properties.Settings;

namespace LelShotter.Views
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            Settings.Default.Upgrade();
            InitializeComponent();
        }

        private void SetPath_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK ||
                    string.IsNullOrWhiteSpace(fbd.SelectedPath)) { return; }

                if (Settings.Default.SavePath != fbd.SelectedPath)
                {
                    if (!Directory.Exists(fbd.SelectedPath))
                    {
                        var createdDir = Utility.CreateDirectory(fbd.SelectedPath);
                        if (!createdDir)
                        {
                            Logger.Log(Level.Error, $"Unable to create and change destination directory to {fbd.SelectedPath}");
                            return;
                        }
                    }
                }
                Settings.Default.SavePath = fbd.SelectedPath;
                Settings.Default.Save();
                Settings.Default.Upgrade();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.Save();
            Settings.Default.Upgrade();

            base.OnClosing(e);

            e.Cancel = true;
            Hide();
        }

        private void AutostartCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (AutostartCheckBox.IsChecked == false)
                {
                    key?.DeleteValue("LelShotter", false);
                }
                else
                {
                    key?.SetValue("LelShotter", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
                }
            }

        }

        private void LogModeCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            var logCheckBox = sender as CheckBox;
            if (logCheckBox != null && logCheckBox.Name == "DebugModeCheckBox")
            {
                if (logCheckBox.IsChecked != null) Settings.Default.DebugMode = (bool) logCheckBox.IsChecked;
            }
            else if (logCheckBox != null && logCheckBox.Name == "VerboseModeCheckBox")
            {
                if (logCheckBox.IsChecked != null) Settings.Default.VerboseMode = (bool) logCheckBox.IsChecked;
            }
            if (logCheckBox?.IsChecked != null && (bool)logCheckBox.IsChecked)
            {
                Logger.OpenStreams();
            }
            else if(logCheckBox?.IsChecked != null && !(bool)logCheckBox.IsChecked)
            {
                Logger.Dispose();
            }
        }
    }
}
