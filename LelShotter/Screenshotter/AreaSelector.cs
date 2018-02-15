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
            public Rectangle Selection;
            private Point _startPoint;
            private Point _finishPoint;
            private bool _isMouseDown;

            public PictureForm(Image image)
            {
                MouseDown += (sender, args) =>
                {
                    _isMouseDown = true;
                    _startPoint.X = args.X;
                    _startPoint.Y = args.Y;
                };
                MouseMove += (sender, args) =>
                {
                    if (_isMouseDown)
                    {
                        _finishPoint.X = args.X;
                        _finishPoint.Y = args.Y;
                        Refresh();
                    }
                };
                MouseUp += (sender, args) =>
                {
                    if (_isMouseDown)
                    {
                        _finishPoint.X = args.X;
                        _finishPoint.Y = args.Y;

                        Selection = new Rectangle(
                            Math.Min(_startPoint.X, _finishPoint.X),
                            Math.Min(_startPoint.Y, _finishPoint.Y),
                            Math.Abs(_finishPoint.X - _startPoint.X),
                            Math.Abs(_finishPoint.Y - _startPoint.Y));
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
                            Math.Min(_startPoint.X, _finishPoint.X),
                            Math.Min(_startPoint.Y, _finishPoint.Y),
                            Math.Abs(_finishPoint.X - _startPoint.X),
                            Math.Abs(_finishPoint.Y - _startPoint.Y));
                        var pen = new Pen(Color.DarkRed, 5);
                        g.DrawRectangle(pen, Selection);
                    }
                };
                KeyPress += (sender, args) =>
                {
                    if (args.KeyChar == (char) Keys.Escape)
                    {
                        DialogResult = DialogResult.Cancel;
                    }
                };

                DoubleBuffered = true;
                ShowInTaskbar = false;
                FormBorderStyle = FormBorderStyle.None;
                DialogResult = DialogResult.None;
                Width = (int) Screenshotter.ScreenWidth;
                Height = (int) Screenshotter.ScreenHeight;
                BackgroundImage = MaskedImage(image);

                Focus();
                BringToFront();
                
                ShowDialog();
            }
            
            private Image MaskedImage(Image image)
            {
                var tempBitmap = (Bitmap)image;
                var newBitmap = new Bitmap(tempBitmap.Width, tempBitmap.Height);
                var newGraphics = Graphics.FromImage(newBitmap);
                float[][] floatColorMatrix = {
                    new [] { 1f, 0f, 0f, 0f, 0f },
                    new [] { 0f, 1f, 0f, 0f, 0f },
                    new [] { 0f, 0f, 1f, 0f, 0f },
                    new [] { 0f, 0f, 0f, 1f, 0f },
                    new [] { 20.0f/255.0f, 20.0f/255.0f, 20.0f/255.0f, 1f, 1f }
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
