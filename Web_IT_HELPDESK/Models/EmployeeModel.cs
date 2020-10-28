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

            var employeeFields = en.Employees.Select(e => new EmployeeFieldModel { EmployeeID = e.EmployeeID, EmployeeName = e.EmployeeName }).Distinct().ToList();

            return employeeFields;
        }
        public List<EmployeeFieldModel> EmployeeFieldsByPlant(string PlantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            var employeeFields = en.Employees.Where(e => e.Plant_Id == PlantId).Select(e => new EmployeeFieldModel { EmployeeID = e.EmployeeID, EmployeeName = e.EmployeeName }).Distinct().ToList();

            return employeeFields;
        }
    }
}