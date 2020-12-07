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
            private ExcelTitle() { }

        private static ExcelTitle _instance;

        public static ExcelTitle Instance { get { if (_instance == null) _instance = new ExcelTitle(); return _instance; } private set => _instance = value; }
    }
}