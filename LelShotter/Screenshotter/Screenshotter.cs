using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LelShotter.Screenshotter
{
    public class Screenshotter
    {
        public readonly bool Upload;
        public readonly bool Save;
        public readonly bool Copy;
        
        public static readonly double ScreenLeft = SystemParameters.VirtualScreenLeft;
        public static readonly double ScreenTop = SystemParameters.VirtualScreenTop;
        public static readonly double ScreenWidth = SystemParameters.VirtualScreenWidth;
        public static readonly double ScreenHeight = SystemParameters.VirtualScreenHeight;

        public Screenshotter(bool upload, bool save, bool copy)
        {
            Upload = upload;
            Save = save;
            Copy = copy;
        }
        public async Task<Models.StatusMessage> TakeScreenshot(Models.ScreenshotMode mode)
        {
            var completeMessage = string.Empty;
            var levelMessage = Models.Level.Error;

            if (!(Upload || Save || Copy))
            {
                levelMessage = Models.Level.Info;
                completeMessage = "No action was selected!";
                return new Models.StatusMessage(levelMessage, completeMessage);
            }

            var bitmap = new Bitmap((int)ScreenWidth, (int)ScreenHeight);
            var screenGraphics = Graphics.FromImage(bitmap);
            try
            {
                screenGraphics.CopyFromScreen((int) ScreenLeft, (int) ScreenTop, 0, 0, bitmap.Size);
            }
            catch (Exception ex)
            {
                completeMessage = ex.Message;
                levelMessage = Models.Level.Error;

                return new Models.StatusMessage(levelMessage, completeMessage);
            }
            
            if (mode == Models.ScreenshotMode.Selection)
            {
                // let the user mark the selection on screen
                var areaSelector = new AreaSelector();
                var area = areaSelector.SelectArea(bitmap);
                if (area != null)
                {
                    bitmap = bitmap.Clone(area.Value, bitmap.PixelFormat);
                }
                else
                {
                    levelMessage = Models.Level.Error;
                    completeMessage = "Could not get user selection!";
                    return new Models.StatusMessage(levelMessage, completeMessage);
                }
            }
                
            if (Save)
            {
                var filename =
                    Path.Combine(Properties.Settings.Default.SavePath,
                        $"ScreenCapture-{DateTime.Now:yyyyMMdd-HHmmss}.png"
                    );
                bitmap.Save(filename);
                completeMessage += $"Screenshot saved to {filename}";
                levelMessage = Models.Level.Success;
            }

            if (Upload)
            {
                var url = await new ImageUploader().UploadImageAsync(bitmap);
                if (!Copy)
                {
                    System.Diagnostics.Process.Start(url);
                    completeMessage += "Image uploaded to Imgur, opening URL...";
                    levelMessage = Models.Level.Success;
                }
                else
                {
                    Clipboard.SetText(url);
                    completeMessage += "Image uploaded to Imgur, URL copied to clipboard.";
                    levelMessage = Models.Level.Success;
                }
            }

            if (Copy)
            {
                var bitmapData = LoadBitmap(bitmap);
                Clipboard.SetImage(bitmapData);
                completeMessage += "Image saved to clipboard.";
                levelMessage = Models.Level.Success;
            }

            screenGraphics.Dispose();
            bitmap.Dispose();
            return new Models.StatusMessage(levelMessage, completeMessage);
        }
     
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
        private static BitmapSource LoadBitmap(Bitmap source)
        {
            var ip = source.GetHbitmap();
            BitmapSource bs;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        ip, 
                        IntPtr.Zero, 
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }
    }

}
