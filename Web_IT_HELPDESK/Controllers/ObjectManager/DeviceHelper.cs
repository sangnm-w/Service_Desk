using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Web_IT_HELPDESK.Commons;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Properties;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class DeviceHelper
    {
        private static DeviceHelper _Instance;

        public static DeviceHelper Instance { get { if (_Instance == null) _Instance = new DeviceHelper(); return _Instance; } set => _Instance = value; }

        private DeviceHelper() { }

        public string CreateQRCode(Device device)
        {
            string deviceQRPath;

            string devicePlantName = DepartmentModel.Instance.getPlantName(device.Plant_Id);

            string QRText = "- Plant: " + devicePlantName + " \n"
                          + "- Device Code: " + device.Device_Code + " \n"
                          + "- Device Name: " + device.Device_Name + " \n"
                          + "- Purchase Date: " + device.Purchase_Date?.ToString("dd/MM/yyyy") + " \n"
                          + "- Employee: Not yet \n"
                          + "- Department: Not yet \n"
                          + "- Delivery Date: Not yet";

            if (new List<int> { 3, 6 }.Contains((int)device.Device_Type_Id))
            {
                QRText = QRText + "\n" + "- IP: Not yet";
            }

            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string DetailsLink = "- Details Link: " + domainName + @"/servicedesk/Devices/Details/" + device.Device_Id + " \n";

            QRText = QRText + "\n" + DetailsLink;

            string filePath = Path.Combine(Resources.DeviceQRCodePath, device.Plant_Id);
            string serverPath = HttpContext.Current.Server.MapPath(filePath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            string fileName = device.Device_Code + "-" + device.Create_Date?.ToString("dd-MM-yyyy") + ".png";
            string savePath = Path.Combine(serverPath, fileName);
            try
            {
                using (var oldImage = QRCodeUtility.Instance.GetBitmapQRCode(QRText))
                {
                    var format = oldImage.RawFormat;
                    using (var newImage = QRCodeUtility.Instance.ResizeImage(oldImage, 200, 200))
                    {
                        newImage.Save(savePath, format);
                    }
                }
                deviceQRPath = Path.Combine(filePath, fileName);
            }
            catch (Exception)
            {
                deviceQRPath = null;
            }

            return deviceQRPath;
        }
    }
}