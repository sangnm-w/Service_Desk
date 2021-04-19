using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Web_IT_HELPDESK.Models.AutoCompleteModel;

namespace Web_IT_HELPDESK.Models
{
    public class EmployeeModel
    {
        private EmployeeModel() { }

        private static EmployeeModel _instance;

        public static EmployeeModel Instance { get { if (_instance == null) _instance = new EmployeeModel(); return _instance; } private set => _instance = value; }

        public List<EmployeeFieldModel> EmployeeFields()
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            //var employeeFields = en.Employees.Select(e => new EmployeeFieldModel { Emp_CJ = e.Emp_CJ, EmployeeName = e.EmployeeName }).Distinct().ToList();
            var employeeFields = en.Employee_New.Select(e => new EmployeeFieldModel { Emp_CJ = e.Emp_CJ, EmployeeName = e.Employee_Name }).Distinct().ToList();

            return employeeFields;
        }
        public List<EmployeeFieldModel> EmployeeFieldsByPlant(string PlantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            var employeeFields = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                .Where(grp => grp.d.Plant_Id == PlantId)
                .Select(grp => new EmployeeFieldModel { Emp_CJ = grp.e.Emp_CJ, EmployeeName = grp.e.Employee_Name })
                .Distinct()
                .ToList();

            return employeeFields;
        }
    }
}