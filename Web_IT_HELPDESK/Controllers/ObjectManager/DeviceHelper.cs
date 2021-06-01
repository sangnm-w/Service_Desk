using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Web_IT_HELPDESK.Commons;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Properties;
using Web_IT_HELPDESK.ViewModels;

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

            string devicePlantName = DepartmentModel.Instance.getPlantNameByPlantId(device.Plant_Id);

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

            string DetailsLink = "- Details Link: " + domainName + @"/Devices/Details/" + device.Device_Id + " \n";

            QRText = QRText + "\n" + DetailsLink;

            string filePath = Path.Combine(Resources.DeviceQRCodePath, device.Plant_Id);
            string serverPath = HostingEnvironment.MapPath(filePath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            
            if (!string.IsNullOrWhiteSpace(device.QRCodeFile))
            {
                string oldFileName = HostingEnvironment.MapPath(device.QRCodeFile);
                if (File.Exists(oldFileName))
                {
                    File.Delete(oldFileName);
                }
            }

            string fileName = device.Device_Code + "-" + DateTime.Now.ToString("dd-MM-yyyy") + ".png";
            string savePath = Path.Combine(serverPath, fileName);
            try
            {
                using (var orginalImage = QRCodeUtility.Instance.GetBitmapQRCode(QRText))
                {
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
        public List<Device> GetDevicesFromExcel(Stream fileStream, out List<DeviceViewModel.ErrDeviceExcel> errDeviceExcels)
        {
            List<Device> devices = new List<Device>();
            List<DeviceViewModel.DeviceExcelTemplate> deviceTemplates = new List<DeviceViewModel.DeviceExcelTemplate>();
            errDeviceExcels = new List<DeviceViewModel.ErrDeviceExcel>();

            deviceTemplates = readDeviceExcelTemplate(fileStream);

            foreach (var item in deviceTemplates)
            {
                Device device = new Device();
                try
                {
                    device.Device_Id = Guid.NewGuid();
                    device.Contract_Id = null;
                    device.Device_Type_Id = Convert.ToInt32(item.Device_Type_Id);
                    device.Device_Code = null;
                    device.Device_Name = item.Device_Name;
                    device.Serial_No = item.Serial_No;
                    device.Purchase_Date = string.IsNullOrWhiteSpace(item.Purchase_Date) ? null : (DateTime?)Convert.ToDateTime(item.Purchase_Date);
                    device.Computer_Name = item.Computer_Name;
                    device.CPU = item.CPU;
                    device.RAM = item.RAM;
                    device.DISK = item.DISK;
                    device.Operation_System = item.Operation_System;
                    device.OS_License = item.OS_License;
                    device.Office = item.Office;
                    device.Office_License = item.Office_License;
                    device.Note = item.Note;
                    device.Depreciation = string.IsNullOrWhiteSpace(item.Depreciation) ? null : (DateTime?)Convert.ToDateTime(item.Depreciation);
                    device.Device_Status = item.Device_Status.Trim();
                    device.Addition_Information = item.Addition_Information;
                    device.Plant_Id = item.Plant_Id;
                    device.Create_Date = string.IsNullOrWhiteSpace(item.Create_Date) ? null : (DateTime?)Convert.ToDateTime(item.Create_Date);

                    if (!Enum.IsDefined(typeof(DeviceModel.DeviceStatus), item.Device_Status.Trim()))
                    {
                        throw new Exception("Wrong Device Status!");
                    }

                    devices.Add(device);
                }
                catch (Exception ex)
                {
                    DeviceViewModel.ErrDeviceExcel errDevice = new DeviceViewModel.ErrDeviceExcel(device);
                    errDevice.errMsg = ex.ToString();
                    errDeviceExcels.Add(errDevice);
                }
            }

            return devices;
        }
        private List<DeviceViewModel.DeviceExcelTemplate> readDeviceExcelTemplate(Stream fileStream)
        {
            List<DeviceViewModel.DeviceExcelTemplate> deviceTemplates = new List<DeviceViewModel.DeviceExcelTemplate>();

            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                var table = worksheet.Tables.First();
                int startRow = table.Address.Start.Row;
                int endRow = table.Address.End.Row;

                for (int rowNo = startRow + 1; rowNo <= endRow; rowNo++)
                {
                    DeviceViewModel.DeviceExcelTemplate deviceTemplate = new DeviceViewModel.DeviceExcelTemplate();
                    deviceTemplate.Device_Type_Id = worksheet.Cells[rowNo, 1].Value?.ToString();
                    deviceTemplate.Device_Name = worksheet.Cells[rowNo, 2].Value?.ToString();
                    deviceTemplate.Serial_No = worksheet.Cells[rowNo, 3].Value?.ToString();
                    deviceTemplate.Purchase_Date = worksheet.Cells[rowNo, 4].Value?.ToString();
                    deviceTemplate.Computer_Name = worksheet.Cells[rowNo, 5].Value?.ToString();
                    deviceTemplate.CPU = worksheet.Cells[rowNo, 6].Value?.ToString();
                    deviceTemplate.RAM = worksheet.Cells[rowNo, 7].Value?.ToString();
                    deviceTemplate.DISK = worksheet.Cells[rowNo, 8].Value?.ToString();
                    deviceTemplate.Operation_System = worksheet.Cells[rowNo, 9].Value?.ToString();
                    deviceTemplate.OS_License = worksheet.Cells[rowNo, 10].Value?.ToString();
                    deviceTemplate.Office = worksheet.Cells[rowNo, 11].Value?.ToString();
                    deviceTemplate.Office_License = worksheet.Cells[rowNo, 12].Value?.ToString();
                    deviceTemplate.Note = worksheet.Cells[rowNo, 13].Value?.ToString();
                    deviceTemplate.Depreciation = worksheet.Cells[rowNo, 14].Value?.ToString();
                    deviceTemplate.Device_Status = worksheet.Cells[rowNo, 15].Value?.ToString();
                    deviceTemplate.Addition_Information = worksheet.Cells[rowNo, 16].Value?.ToString();
                    deviceTemplate.Plant_Id = worksheet.Cells[rowNo, 17].Value?.ToString();
                    deviceTemplate.Create_Date = worksheet.Cells[rowNo, 18].Value?.ToString();

                    deviceTemplates.Add(deviceTemplate);
                }
                return deviceTemplates;
            }
        }
    }
}