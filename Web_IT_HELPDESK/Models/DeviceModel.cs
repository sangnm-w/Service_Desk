using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Models
{
    public class DeviceModel
    {
        private static DeviceModel instance;
        public static DeviceModel Instance { get { if (instance == null) instance = new DeviceModel(); return instance; } set => instance = value; }
        public DeviceModel() { }

        public string Generate_DeviceCode(string plantId, int? deviceTypeId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            Device_Type deviceType = en.Device_Type.FirstOrDefault(d => d.Device_Type_Id == deviceTypeId);

            List<string> strDeviceCodes = en.Devices.Where(d => d.Device_Type_Id == deviceType.Device_Type_Id && d.Plant_Id == plantId).Select(a => a.Device_Code).ToList();
            if (strDeviceCodes.Count <= 0)
            {
                return plantId + "-" + deviceType.Device_Type_Name + "0001";
            }
            List<int> intDeviceCodes = new List<int>();
            for (int i = 0; i < strDeviceCodes.Count(); i++)
            {
                if (strDeviceCodes[i] != null)
                {
                    int length = strDeviceCodes[i].Length;
                    int code = Convert.ToInt32(strDeviceCodes[i].Substring(length - 4, 4));
                    intDeviceCodes.Add(code);
                }
            }

            int intDeviceCode = intDeviceCodes.Max() + 1;
            string strDeviceCode = intDeviceCode.ToString("D4");

            return plantId + "-" + deviceType.Device_Type_Name + strDeviceCode;
        }

        public string Generate_DeviceCode_Upload_ByList(string plantId, int? deviceTypeId, List<Device> deviceList)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            Device_Type deviceType = en.Device_Type.FirstOrDefault(d => d.Device_Type_Id == deviceTypeId);
            List<string> strDeviceCodes = new List<string>();
            if (deviceList.Count <= 0)
            {
                strDeviceCodes = en.Devices.Where(d => d.Device_Type_Id == deviceType.Device_Type_Id && d.Plant_Id == plantId).Select(a => a.Device_Code).ToList();
            }
            else
            {
                strDeviceCodes = deviceList.Where(d => d.Device_Type_Id == deviceType.Device_Type_Id).Select(a => a.Device_Code).ToList();
            }

            if (strDeviceCodes.Count <= 0)
            {
                return plantId + "-" + deviceType.Device_Type_Name + "0001";
            }

            List<int> intDeviceCodes = new List<int>();
            for (int i = 0; i < strDeviceCodes.Count(); i++)
            {
                if (strDeviceCodes[i] != null)
                {
                    int length = strDeviceCodes[i].Length;
                    int code = Convert.ToInt32(strDeviceCodes[i].Substring(length - 4, 4));
                    intDeviceCodes.Add(code);
                }
            }

            int intDeviceCode = intDeviceCodes.Max() + 1;
            string strDeviceCode = intDeviceCode.ToString("D4");

            return plantId + "-" + deviceType.Device_Type_Name + strDeviceCode;
        }
        public string Generate_DeviceCode_Upload_OnebyOne(string plantId, int? deviceTypeId)
        {
            string maxDeviceCodeInDB = null;
            Device_Type deviceType = null;
            try
            {
                ServiceDeskEntities en = new ServiceDeskEntities();

                deviceType = en.Device_Type.FirstOrDefault(d => d.Device_Type_Id == deviceTypeId);
                maxDeviceCodeInDB = en.Devices.Where(d => d.Device_Type_Id == deviceType.Device_Type_Id && d.Plant_Id == plantId).Max(d => d.Device_Code);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(maxDeviceCodeInDB))
            {
                return plantId + "-" + deviceType.Device_Type_Name + "0001";
            }

            int lenDeviceCodeInDB = maxDeviceCodeInDB.Length;
            int numDeviceCodeInDB = Convert.ToInt32(maxDeviceCodeInDB.Substring(lenDeviceCodeInDB - 4, 4));
            int intDeviceCode = numDeviceCodeInDB + 1;
            string strDeviceCode = intDeviceCode.ToString("D4");

            return plantId + "-" + deviceType.Device_Type_Name + strDeviceCode;
        }
        public IEnumerable<DeviceViewModel> GetDevices()
        {
            ServiceDeskEntities en = new ServiceDeskEntities();
            var devices = en.Devices.Include(d => d.CONTRACT).Include(d => d.Device_Type);

            var plants = en.Plants.Select(p => new { p.Plant_Id, p.Plant_Name }).Distinct();

            var dvm = devices
                .Join(
                plants,
                d => d.Plant_Id,
                p => p.Plant_Id,
                (d, p) => new DeviceViewModel
                {
                    Device = d,
                    Plant_Name = p.Plant_Name
                });
            return dvm;
        }

        public IEnumerable<DeviceViewModel> GetDevicesByPlantId(string plantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();
            var devices = en.Devices.Include(d => d.CONTRACT).Include(d => d.Device_Type).Where(d => d.Plant_Id == plantId);

            var plants = en.Plants.Select(p => new { p.Plant_Id, p.Plant_Name }).Distinct();

            var dvm = devices
                .Join(
                plants,
                d => d.Plant_Id,
                p => p.Plant_Id,
                (d, p) => new DeviceViewModel
                {
                    Device = d,
                    Plant_Name = p.Plant_Name
                });
            return dvm;
        }

        public List<DeviceViewModel.QRDevices> GetQRDevicesByPlantID(string plantID)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();
            List<DeviceViewModel.QRDevices> res = new List<DeviceViewModel.QRDevices>();

            string devicePlantName = DepartmentModel.Instance.getPlantNameByPlantId(plantID);
            var devices = en.Devices.Where(model => model.Plant_Id == plantID).OrderBy(model=>model.Device_Code).ToList();

            foreach (Device d in devices)
            {
                var allocation = en.Allocations.FirstOrDefault(model => model.Device_Id == d.Device_Id);

                string QRText = ""
                         + "- Plant: " + devicePlantName + " \n"
                         + "- Device Code: " + d.Device_Code + " \n"
                         + "- Device Name: " + d.Device_Name + " \n"
                         + "- Purchase Date: " + d.Purchase_Date?.ToString("dd/MM/yyyy") + " \n";
                if (allocation == null)
                {
                    QRText = QRText
                         + "- Employee: Not yet \n"
                         + "- Department: Not yet \n"
                         + "- Delivery Date: Not yet";
                }
                else
                {
                    string allocationEmpName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).Employee_Name;
                    string allocationDeptName = DepartmentModel.Instance.getDeptNameByDeptId(allocation.Department_Id);
                    string allocationDeliveryDate = allocation.Delivery_Date?.ToString("dd/MM/yyyy");
                    QRText = QRText
                         + "- Employee: " + allocationEmpName + " \n"
                         + "- Department: " + allocationDeptName + " \n"
                         + "- Delivery Date: " + allocationDeliveryDate;

                    if (allocation.Return_Date != null)
                    {
                        QRText = QRText + "\n" + "- Revoke Date: " + allocation.Return_Date?.ToString("dd/MM/yyyy");
                    }

                    if (new List<int> { 3, 6 }.Contains((int)d.Device_Type_Id))
                    {
                        QRText = QRText + "\n" + "- IP: " + allocation.IP;
                    }
                }

                DeviceViewModel.QRDevices qr = new DeviceViewModel.QRDevices()
                {
                    Content = QRText,
                    QRCode = d.QRCodeFile
                };

                res.Add(qr);
            }

            return res;
        }

        public enum DeviceStatus
        {
            In_Stock,
            Using
        }
    }
}