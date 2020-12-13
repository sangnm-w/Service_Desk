using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Models
{
    public class DepartmentModel
    {
        private DepartmentModel() { }
        private static DepartmentModel _instance;

        public static DepartmentModel Instance { get { if (_instance == null) _instance = new DepartmentModel(); return _instance; } private set => _instance = value; }

        public string getPlantName(string plantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.FirstOrDefault(d => d.Plant_Id == plantId).Plant_Name;
        }

        public string getDeptName(string plantId, string deptId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.FirstOrDefault(d => d.Plant_Id == plantId && d.Department_Id == deptId).Department_Name;
        }

        internal string getManagerEmail(string plantId, string deptId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.FirstOrDefault(d => d.Plant_Id == plantId && d.Department_Id == deptId).Manager_Email;
        }
    }
}