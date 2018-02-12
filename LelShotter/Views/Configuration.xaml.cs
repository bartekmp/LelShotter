using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using LelShotter.Models;
using LelShotter.Utils;
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
                    string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

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
    }
}
