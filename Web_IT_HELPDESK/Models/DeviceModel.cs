using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public string Generate_DeviceCode_Upload(string plantId, int? deviceTypeId, List<Device> deviceList)
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

        public IEnumerable<DeviceViewModel> GetDevices()
        {
            ServiceDeskEntities en = new ServiceDeskEntities();
            var devices = en.Devices.Include(d => d.CONTRACT).Include(d => d.Device_Type);

            var plants = en.Departments.Select(p => new { p.Plant_Id, p.Plant_Name }).Distinct();

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

            var plants = en.Departments.Select(p => new { p.Plant_Id, p.Plant_Name }).Distinct();

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

    }
}