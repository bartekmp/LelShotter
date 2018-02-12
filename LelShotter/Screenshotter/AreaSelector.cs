using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace LelShotter.Screenshotter
{
    public class AreaSelector
    {
        protected Pen SelectPen;
        protected Point SourcePoint;
        protected Point DestinationPoint;

        public Rectangle? SelectArea(Bitmap bmp)
        {
            var form = new PictureForm(bmp);
            if (form.DialogResult != DialogResult.OK)
            {
                return null;
            }
            
            return form.Selection;
        }
        
        private sealed class PictureForm : Form
        {
            public Point StartPoint, FinishPoint;
            public Rectangle Selection;
            private bool _isMouseDown;
            public PictureForm(Image image)
            {

                DoubleBuffered = true;

                MouseDown += (sender, args) =>
                {
                    _isMouseDown = true;
                    StartPoint.X = args.X;
                    StartPoint.Y = args.Y;
                };
                MouseMove += (sender, args) =>
                {
                    if (_isMouseDown)
                    {
                        FinishPoint.X = args.X;
                        FinishPoint.Y = args.Y;
                        Refresh();
                    }
                };
                MouseUp += (sender, args) =>
                {
                    if (_isMouseDown)
                    {
                        FinishPoint.X = args.X;
                        FinishPoint.Y = args.Y;

                        Selection = new Rectangle(
                            Math.Min(StartPoint.X, FinishPoint.X),
                            Math.Min(StartPoint.Y, FinishPoint.Y),
                            Math.Abs(FinishPoint.X - StartPoint.X),
                            Math.Abs(FinishPoint.Y - StartPoint.Y));
                        DialogResult = DialogResult.OK;
                        _isMouseDown = false;
                    }
                };

                Paint += (sender, args) =>
                {
                    if (_isMouseDown)
                    {
                        var g = args.Graphics;
                        Selection = new Rectangle(
                            Math.Min(StartPoint.X, FinishPoint.X),
                            Math.Min(StartPoint.Y, FinishPoint.Y),
                            Math.Abs(FinishPoint.X - StartPoint.X),
                            Math.Abs(FinishPoint.Y - StartPoint.Y));
                        var pen = new Pen(Color.DarkRed, 5);
                        g.DrawRectangle(pen, Selection);
                    }
                };

                FormBorderStyle = FormBorderStyle.None;
                BackgroundImage = DarkenImage(image);
                Width = (int) Screenshotter.ScreenWidth;
                Height = (int) Screenshotter.ScreenHeight;
                DialogResult = DialogResult.None;
                Focus();
                ShowDialog();
            }
            
            private Image DarkenImage(Image image)
            {
                var tempBitmap = (Bitmap)image;
                var newBitmap = new Bitmap(tempBitmap.Width, tempBitmap.Height);
                var newGraphics = Graphics.FromImage(newBitmap);
                float[][] floatColorMatrix = {
                    new [] {1f, 0f, 0f, 0f, 0f},
                    new [] {0f, 1f, 0f, 0f, 0f},
                    new [] {0f, 0f, 1f, 0f, 0f},
                    new [] {0f, 0f, 0f, 1f, 0f},
                    new[] { (float)20.0 / 255.0f, (float)20.0 / 255.0f, (float)20.0 / 255.0f, 1, 1}
                };

                var newColorMatrix = new ColorMatrix(floatColorMatrix);
                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(newColorMatrix);
                    newGraphics.DrawImage(tempBitmap, new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), 0, 0,
                        tempBitmap.Width, tempBitmap.Height, GraphicsUnit.Pixel, attributes);
                    newGraphics.Dispose();

                    return newBitmap;
                }
            }

            ~PictureForm()
            {
                Hide();
                Close();
                Dispose();
            }
        }
    }
}
