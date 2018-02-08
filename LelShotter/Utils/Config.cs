using System.Configuration;

namespace LelShotter.Utils
{
    public static class Config
    {
        public static string SavePath
        {
            get => Properties.Settings.Default.SavePath;
            set { Properties.Settings.Default.SavePath = value; Properties.Settings.Default.Save(); }
        }
    }
}
