using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class DeviceViewModel
    {
        public Device Device { get; set; }

        public string Plant_Name { get; set; }

        public class DeviceExcelTemplate
        {
            public string Device_Type_Id { get; set; }
            public string Device_Code { get; set; }
            public string Device_Name { get; set; }
            public string Serial_No { get; set; }
            public string Purchase_Date { get; set; }
            public string Computer_Name { get; set; }
            public string CPU { get; set; }
            public string RAM { get; set; }
            public string DISK { get; set; }
            public string Operation_System { get; set; }
            public string OS_License { get; set; }
            public string Office { get; set; }
            public string Office_License { get; set; }
            public string Note { get; set; }
            public string Depreciation { get; set; }
            public string Device_Status { get; set; }
            public string Addition_Information { get; set; }
            public string Plant_Id { get; set; }
            public string Create_Date { get; set; }
        }

        public class ErrDeviceExcel
        {
            public ErrDeviceExcel() { }
            public ErrDeviceExcel(Device device)
            {
                this.Device_Type_Id = device.Device_Type_Id;
                this.Device_Name = device.Device_Name;
                this.Serial_No = device.Serial_No;
                this.Purchase_Date = device.Purchase_Date;
                this.Computer_Name = device.Computer_Name;
                this.CPU = device.CPU;
                this.RAM = device.RAM;
                this.DISK = device.DISK;
                this.Operation_System = device.Operation_System;
                this.OS_License = device.OS_License;
                this.Office = device.Office;
                this.Office_License = device.Office_License;
                this.Note = device.Note;
                this.Depreciation = device.Depreciation;
                this.Device_Status = device.Device_Status;
                this.Addition_Information = device.Addition_Information;
                this.Plant_Id = device.Plant_Id;
                this.Create_Date = device.Create_Date;
            }

            #region Properties
            public int? Device_Type_Id { get; set; }
            public string Device_Name { get; set; }
            public string Serial_No { get; set; }
            public DateTime? Purchase_Date { get; set; }
            public string Computer_Name { get; set; }
            public string CPU { get; set; }
            public string RAM { get; set; }
            public string DISK { get; set; }
            public string Operation_System { get; set; }
            public string OS_License { get; set; }
            public string Office { get; set; }
            public string Office_License { get; set; }
            public string Note { get; set; }
            public DateTime? Depreciation { get; set; }
            public string Device_Status { get; set; }
            public string Addition_Information { get; set; }
            public string Plant_Id { get; set; }
            public DateTime? Create_Date { get; set; }
            public string errMsg { get; set; } 
            #endregion
        }
    }
}