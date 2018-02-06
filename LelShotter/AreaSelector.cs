using System.Drawing;
using System.Windows.Forms;


namespace LelShotter
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
            var start = form.StartPoint;
            var finish = form.FinishPoint;
            var span = new Size(finish.X - start.X, finish.Y - start.Y);
            return new Rectangle(start, span);
        }
        
        private sealed class PictureForm : Form
        {
            public Point StartPoint, FinishPoint;
            public PictureForm(Image image)
            {
                MouseDown += (sender, args) =>
                {
                    StartPoint.X = args.X;
                    StartPoint.Y = args.Y;
                };
                MouseUp += (sender, args) =>
                {
                    FinishPoint.X = args.X;
                    FinishPoint.Y = args.Y;
                    DialogResult = DialogResult.OK;
                };
                FormBorderStyle = FormBorderStyle.None;
                // TODO add image overlay and selection rectangle
                BackgroundImage = image;
                Width = (int) Screenshotter.ScreenWidth;
                Height = (int) Screenshotter.ScreenHeight;
                DialogResult = DialogResult.None;
                Focus();
                ShowDialog();
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
