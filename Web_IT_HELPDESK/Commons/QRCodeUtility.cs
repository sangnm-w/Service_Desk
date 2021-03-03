using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using QRCoder;

namespace Web_IT_HELPDESK.Commons
{
    public class QRCodeUtility
    {
        private static QRCodeUtility _Instance;

        public static QRCodeUtility Instance { get { if (_Instance == null) _Instance = new QRCodeUtility(); return _Instance; } set => _Instance = value; }
        private QRCodeUtility() { }

        public Bitmap GetBitmapQRCode(string QRText)
        {
            string logoPath = HostingEnvironment.MapPath(@"~/Library/Picture/Logo.jpg");
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(QRText, QRCodeGenerator.ECCLevel.Q, forceUtf8: true);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(2, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(logoPath));
            return qrCodeImage;
        }

        public Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            if (image.Width <= width && image.Height <= height)
            {
                return image;
            }

            int newWidth;
            int newHeight;
            if (image.Width > image.Height)
            {
                newWidth = width;
                newHeight = (int)(image.Height * ((float)width / image.Width));
            }
            else
            {
                newHeight = height;
                newWidth = (int)(image.Width * ((float)height / image.Height));
            }

            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.FillRectangle(Brushes.Transparent, 0, 0, newWidth, newHeight);
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                return newImage;
            }
        }
    }
}