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

            return en.Plants.FirstOrDefault(d => d.Plant_ID == plantId).Plant_Name;
        }

        public string getDeptName(string deptId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.FirstOrDefault(d => d.Department_ID == deptId).Department_Name;
        }

        public string getManagerEmail(string plantId, string deptId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.FirstOrDefault(d => d.Department_ID == deptId).Manager_Email;
        }

        public string getManagerEmailbyID(string deptId, string empId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments
                .Join(en.Employee_New,
                      d => d.Manager_ID,
                      e => e.Emp_CJ,
                      (d, e) => new
                      {
                          e.Emp_CJ,
                          e.Email,
                          d.Department_ID
                      })
                .FirstOrDefault(d => d.Department_ID == deptId && d.Emp_CJ == empId)
                .Email;
        }
    }
}