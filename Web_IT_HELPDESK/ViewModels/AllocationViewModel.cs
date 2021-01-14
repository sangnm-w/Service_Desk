using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class AllocationViewModel
    {
        public Allocation Allocation { get; set; }
        public Device Device { get; set; }

        public string Deliver_Name { get; set; }
        public string Receiver_Name { get; set; }
        public string Department_Name { get; set; }

        public AllocationViewModel()
        {
            Allocation = new Allocation();
            Device = new Device();
        }
        public class ExcelReport
        {
            public ExcelReport(Device d, Allocation p, string deviceTypeName, string deliverName, string receiverName, string deptName, string plantName)
            {
                // Device Part
                Device_Type_Name = deviceTypeName;
                Device_Code = d?.Device_Code;
                Device_Name = d?.Device_Name;
                Serial_No = d?.Serial_No;
                Purchase_Date = d?.Purchase_Date;
                Computer_Name = d?.Computer_Name;
                CPU = d?.CPU;
                RAM = d?.RAM;
                DISK = d?.DISK;
                Operation_System = d?.Operation_System;
                OS_License = d?.OS_License;
                Office = d?.Office;
                Office_License = d?.Office_License;
                Device_Note = d?.Note;
                Depreciation = d?.Depreciation;
                Device_Status = d?.Device_Status;
                Addition_Information = d?.Addition_Information;
                Create_Date = d?.Create_Date;

                // Providing (Allocation) Part
                Allocation_Code = p?.Allocation_Code;
                Deliver_Name = deliverName;
                Receiver_Name = receiverName;
                Delivery_Date = p?.Delivery_Date;
                Return_Date = p?.Return_Date;
                Department_Name = deptName;
                Plant_Name = plantName;
                Providing_Note = p?.Note;
                IP = p?.IP;
                QRCodeFile = p?.QRCodeFile;
            }

            // Device Information
            public string Device_Type_Name { get; set; }
            public string Device_Code { get; set; }
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
            public string Device_Note { get; set; }
            public DateTime? Depreciation { get; set; }
            public string Device_Status { get; set; }
            public string Addition_Information { get; set; }
            public DateTime? Create_Date { get; set; }

            //Providing (Allocation) Information
            public string Allocation_Code { get; set; }
            public string Deliver_Name { get; set; }
            public string Receiver_Name { get; set; }
            public DateTime? Delivery_Date { get; set; }
            public DateTime? Return_Date { get; set; }
            public string Department_Name { get; set; }
            public string Plant_Name { get; set; }
            public string Providing_Note { get; set; }
            public string IP { get; set; }
            public string QRCodeFile { get; set; }
        }
    }
}