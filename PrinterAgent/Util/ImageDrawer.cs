using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static readonly string FontFamily = ConfigurationManager.AppSettings["FontFamily"];
        private static readonly string FontColor = ConfigurationManager.AppSettings["FontColor"];
        private static readonly int FontSizePixels = int.Parse(ConfigurationManager.AppSettings["FontSizePixels"]);

        public static byte[] DrawImage(string text)
        {
            if (text == null)
                text = ".";

            var fontFamily = new FontFamily(FontFamily);
            var font = new Font(fontFamily, FontSizePixels, FontStyle.Bold, GraphicsUnit.Pixel);
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
                Brush textBrush = new SolidBrush(ColorTranslator.FromHtml(FontColor));
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
