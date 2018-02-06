using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LelShotter
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
        public async Task<DataModels.StatusMessage> TakeScreenshot(DataModels.ScreenshotMode mode)
        {
            var completeMessage = string.Empty;
            var levelMessage = DataModels.Level.Error;

            if (!(Upload || Save || Copy))
            {
                levelMessage = DataModels.Level.Info;
                completeMessage = "No action was selected!";
                return new DataModels.StatusMessage(levelMessage, completeMessage);
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
                levelMessage = DataModels.Level.Error;

                return new DataModels.StatusMessage(levelMessage, completeMessage);
            }
            
            if (mode == DataModels.ScreenshotMode.Selection)
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
                    levelMessage = DataModels.Level.Error;
                    completeMessage = "Could not get user selection!";
                    return new DataModels.StatusMessage(levelMessage, completeMessage);
                }
            }
                
            if (Save)
            {
                var filename =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"ScreenCapture-{DateTime.Now:yyyyMMdd-HHmmss}.png"
                    );
                bitmap.Save(filename);
                completeMessage += $"Screenshot saved to {filename}";
                levelMessage = DataModels.Level.Success;
            }

            if (Upload)
            {
                var url = await new ImageUploader().UploadImageAsync(bitmap);
                if (!Copy)
                {
                    System.Diagnostics.Process.Start(url);
                    completeMessage += "Image uploaded to Imgur, opening URL...";
                    levelMessage = DataModels.Level.Success;
                }
                else
                {
                    Clipboard.SetText(url);
                    completeMessage += "Image uploaded to Imgur, URL copied to clipboard.";
                    levelMessage = DataModels.Level.Success;
                }
            }

            if (Copy)
            {
                var bitmapData = LoadBitmap(bitmap);
                Clipboard.SetImage(bitmapData);
                completeMessage += "Image saved to clipboard.";
                levelMessage = DataModels.Level.Success;
            }

            screenGraphics.Dispose();
            bitmap.Dispose();
            return new DataModels.StatusMessage(levelMessage, completeMessage);
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
