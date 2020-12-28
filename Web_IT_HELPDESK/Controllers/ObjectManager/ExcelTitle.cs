using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class ExcelTitle
    {
        public Dictionary<int, string> IncTitles()
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
        public Dictionary<int, string> ContactsTitles()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            res.Add(1, "Employee No");
            res.Add(2, "Full Name");
            res.Add(3, "Email");
            res.Add(4, "Phone");
            res.Add(5, "Birthday");
            return res;
        }

        public Dictionary<int, string> DevicesTitles()
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
        private ExcelTitle() { }

        private static ExcelTitle _instance;

        public static ExcelTitle Instance { get { if (_instance == null) _instance = new ExcelTitle(); return _instance; } private set => _instance = value; }
    }
}