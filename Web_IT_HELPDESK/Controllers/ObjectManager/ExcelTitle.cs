using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class ExcelTitle
    {
        public Dictionary<int, string> Incidents()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            res.Add(1, "Code");
            res.Add(2, "Date");
            res.Add(3, "Status");
            res.Add(4, "Level");
            res.Add(5, "Creator");
            res.Add(6, "Solver");
            res.Add(7, "Note");
            res.Add(8, "Reply");
            res.Add(9, "File");
            res.Add(10, "Department");
            res.Add(11, "Plant");
            return res;
        }

        public Dictionary<int, string> Contacts()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            res.Add(1, "Employee No");
            res.Add(2, "Full Name");
            res.Add(3, "Email");
            res.Add(4, "Phone");
            res.Add(5, "Birthday");
            return res;
        }

        public Dictionary<int, string> Devices()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            res.Add(1, "Device_Type_ID");
            res.Add(2, "Device_Name");
            res.Add(3, "Serial_No");
            res.Add(4, "Purchase_Date");
            res.Add(5, "Computer_Name");
            res.Add(6, "CPU");
            res.Add(7, "RAM");
            res.Add(8, "DISK");
            res.Add(9, "Operation_System");
            res.Add(10, "OS_License");
            res.Add(11, "Office");
            res.Add(12, "Office_License");
            res.Add(13, "Note");
            res.Add(14, "Depreciate");
            res.Add(15, "Device_Status");
            res.Add(16, "Addition_Information");
            res.Add(17, "Plant_ID");
            res.Add(18, "Create_Date");
            return res;
        }

        public Dictionary<int, string> DevicesExcelReport()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();

            // Device Part
            res.Add(1, "Device Type name");
            res.Add(2, "Device Code");
            res.Add(3, "Device Name");
            res.Add(4, "Serial");
            res.Add(5, "Purchase Date");
            res.Add(6, "Computer Name");
            res.Add(7, "CPU");
            res.Add(8, "RAM");
            res.Add(9, "DISK");
            res.Add(10, "Operation System");
            res.Add(11, "OS License");
            res.Add(12, "Office");
            res.Add(13, "Office License");
            res.Add(14, "Device Note");
            res.Add(15, "Depreciate");
            res.Add(16, "Device Status");
            res.Add(17, "Addition Information");
            res.Add(18, "Create_Date");

            // Providing (Allocation) Part
            res.Add(19, "Allocation_Code");
            res.Add(20, "Deliver_Name");
            res.Add(21, "Receiver_Name");
            res.Add(22, "Delivery_Date");
            res.Add(23, "Return_Date");
            res.Add(24, "Department_Name");
            res.Add(25, "Plant_Name");
            res.Add(26, "Providing_Note");
            res.Add(27, "IP");
            res.Add(28, "QRCodeFile");
            return res;
        }

        public Dictionary<int, string> QRDevices()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            res.Add(1, "Device Information");
            res.Add(2, "QR Code");
            return res;
        }

        //Contract title following ContractViewModel.Excel.ContractAndSub
        public Dictionary<int, string> Contracts()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            res.Add(1, "OrderName");
            res.Add(2, "VENDOR");
            res.Add(3, "ADDRESS");
            res.Add(4, "PHONE");
            res.Add(5, "CONTRACT NAME");
            res.Add(6, "CONTRACT TYPE");
            res.Add(7, "PERIOD");
            res.Add(8, "REPRESENTATION");
            res.Add(9, "DATE");
            res.Add(10, "MONTH");
            res.Add(11, "DATE MATURITY");
            res.Add(12, "NOTE");
            res.Add(13, "DEPARTMENTID");
            res.Add(14, "PLANT");
            res.Add(15, "USER CREATE");
            res.Add(16, "DATE CREATE");
            return res;
        }

        private ExcelTitle() { }

        private static ExcelTitle _instance;

        public static ExcelTitle Instance { get { if (_instance == null) _instance = new ExcelTitle(); return _instance; } private set => _instance = value; }
    }
}