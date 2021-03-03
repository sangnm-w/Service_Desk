using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Web_IT_HELPDESK.Commons;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Properties;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class AllocationHelper
    {
        private static AllocationHelper _Instance;

        public static AllocationHelper Instance { get { if (_Instance == null) _Instance = new AllocationHelper(); return _Instance; } set => _Instance = value; }

        private AllocationHelper() { }

        private string domainName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        public string CreateQRCode(Allocation allocation)
        {
            string deviceQRPath;

            ServiceDeskEntities en = new ServiceDeskEntities();
            Device device = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id);

            string devicePlantName = DepartmentModel.Instance.getPlantName(device.Plant_Id);

            string allocationEmpName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).EmployeeName;
            string allocationDeptName = DepartmentModel.Instance.getDeptName(allocation.Plant_Id, allocation.Department_Id);
            string allocationDeliveryDate = allocation.Delivery_Date?.ToString("dd/MM/yyyy");

            string QRText = "- Plant: " + devicePlantName + " \n"
                          + "- Device Code: " + device.Device_Code + " \n"
                          + "- Device Name: " + device.Device_Name + " \n"
                          + "- Purchase Date: " + device.Purchase_Date?.ToString("dd/MM/yyyy") + " \n"
                          + "- Employee: " + allocationEmpName + " \n"
                          + "- Department: " + allocationDeptName + " \n"
                          + "- Delivery Date: " + allocationDeliveryDate;

            if (allocation.Return_Date != null)
            {
                QRText = QRText + "\n" + "- Revoke Date: " + allocation.Return_Date?.ToString("dd/MM/yyyy");
            }

            if (new List<int> { 3, 6 }.Contains((int)device.Device_Type_Id))
            {
                QRText = QRText + "\n" + "- IP: " + allocation.IP;
            }

            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string DetailsLink = "- Details Link: " + domainName + @"/servicedesk/Devices/Details/" + device.Device_Id + " \n";

            QRText = QRText + "\n" + DetailsLink;

            string filePath = Path.Combine(Resources.DeviceQRCodePath, device.Plant_Id);
            string serverPath = HostingEnvironment.MapPath(filePath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            string fileName = device.Device_Code + "-" + device.Create_Date?.ToString("dd-MM-yyyy") + ".png";
            string savePath = Path.Combine(serverPath, fileName);
            try
            {
                using (var orginalImage = QRCodeUtility.Instance.GetBitmapQRCode(QRText))
                {
                    //var format = orginalImage.RawFormat;
                    //using (var newImage = QRCodeUtility.Instance.ResizeImage(orginalImage, 200, 200))
                    //{
                    //    newImage.Save(savePath, format);
                    //}
                    orginalImage.Save(savePath);
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