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

        public List<string> getPlantNames()
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Plants.Select(p => p.Plant_Name).ToList();
        }

        public string getPlantNameByPlantId(string plantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Plants.FirstOrDefault(p => p.Plant_Id == plantId).Plant_Name;
        }
        public string getPlantNameByDeptId(string deptId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();
            string result = null;

            result = en.Departments
                .Join(en.Plants, d => d.Plant_Id, p => p.Plant_Id, (d, p) => new { d, p })
                .FirstOrDefault(grp => grp.d.Department_Id == deptId).p.Plant_Name.ToString();

            return result;
        }
        public List<string> getDeptNames()
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.Where(d => d.Deactive != true).Select(d => d.Department_Name).ToList();
        }

        public string getDeptNameByDeptId(string deptId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.FirstOrDefault(d => d.Department_Id == deptId).Department_Name;
        }

        public string getManagerEmail(string plantId, string deptId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments.FirstOrDefault(d => d.Department_Id == deptId).Manager_Email;
        }

        public string getManagerEmailbyID(string deptId, string empId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            return en.Departments
                .Join(en.Employee_New,
                      d => d.Manager_Id,
                      e => e.Emp_CJ,
                      (d, e) => new
                      {
                          e.Emp_CJ,
                          e.Email,
                          d.Department_Id
                      })
                .FirstOrDefault(d => d.Department_Id == deptId && d.Emp_CJ == empId)
                .Email;
        }
    }
}