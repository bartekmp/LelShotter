using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using LelShotter.Utils;

namespace LelShotter.Views
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            Properties.Settings.Default.Upgrade();
            InitializeComponent();
        }

        private void SetPath_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK ||
                    string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

                if (Properties.Settings.Default.SavePath != fbd.SelectedPath)
                {
                    if (!Directory.Exists(fbd.SelectedPath))
                    {
                        var createdDir = Utility.CreateDirectory(fbd.SelectedPath);
                        if (!createdDir)
                        {
                            Logger.LogError($"Unable to create and change destination directory to {fbd.SelectedPath}");
                            return;
                        }
                    }
                }
                Properties.Settings.Default.SavePath = fbd.SelectedPath;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            base.OnClosing(e);
        }
    }
}
