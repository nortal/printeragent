using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrinterAgent.Util
{
    public static class ImageDrawer
    {

        public static byte[] DrawImage(string text)
        {
            var fontFamily = new FontFamily("Verdana");
            var font = new Font(fontFamily, 11, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF textSize;

            using (Bitmap bitmap = new Bitmap(1,1))
            using (Graphics drawing = Graphics.FromImage(bitmap))
            {
                textSize=drawing.MeasureString(text, font);
            }

            using (Bitmap bitmap = new Bitmap((int)textSize.Width, (int)textSize.Height))
            using (Graphics drawing = Graphics.FromImage(bitmap))
            {
                
                drawing.Clear(Color.Transparent);
                drawing.TextRenderingHint = TextRenderingHint.AntiAlias;
                Brush textBrush = new SolidBrush(Color.FromArgb(124, 110, 12));
                drawing.DrawString(text, font, textBrush, 0, 0);
                return ImageToBytes(bitmap, ImageFormat.Png);
            }

        }

        private static byte[] ImageToBytes(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                return imageBytes;
            }
        }
    }
}
