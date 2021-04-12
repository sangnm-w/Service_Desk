using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Models.Extensions
{
    public class ApplicationUser
    {
        private static ApplicationUser _instance;
        public static ApplicationUser Instance { get { if (_instance == null) _instance = new ApplicationUser(); return _instance; } set => _instance = value; }

        public string EmployeeID { get; set; }

        private bool _isAdmin = false;

        private bool? _isDeactive = false;

        public bool isAdmin
        {
            get
            {
                _isAdmin = en.Authorizations.FirstOrDefault(x => x.Emp_CJ == EmployeeID).Role_ID == 1 ? true : false;

                return _isAdmin;
            }
            set { _isAdmin = value; }
        }

        public bool? isDeactive
        {
            get
            {
                if (isAdmin == true)
                    return _isDeactive;

                _isDeactive = en.Employee_New.Find(EmployeeID).Deactive;
                return _isDeactive;
            }
            set { _isDeactive = value; }
        }



        private ServiceDeskEntities en = new ServiceDeskEntities();

        private ApplicationUser()
        {
            EmployeeID = HttpContext.Current.User.Identity.Name;
        }

        public string GetEmployeeName()
        {
            string result = null;

            result = en.Employee_New.Find(EmployeeID).Employee_Name;

            return result;
        }

        public string GetDepartmentID()
        {
            string result = null;

            result = en.Employee_New.Find(EmployeeID).Department_ID;

            return result;
        }

        public string GetDepartmentName()
        {
            string result = null;
            string departmentID = GetDepartmentID();

            result = en.Departments.Find(departmentID).Department_Name;

            return result;
        }

        public string GetPlantID()
        {
            string result = null;
            string departmentID = GetDepartmentID();

            result = en.Departments.Find(departmentID).Plant_ID;

            return result;
        }

        public string GetPlantName()
        {
            string result = null;
            string plantID = GetPlantID();

            result = en.Plants.Find(plantID).Plant_Name;

            return result;
        }

        //public bool isAdmin()
        //{
        //    bool result = false;

        //    result = en.Authorizations.FirstOrDefault(x => x.Emp_CJ == EmployeeID).Role_ID == 1 ? true : false;

        //    return result;
        //}


        public List<Authorization> GetAuthorizations()
        {
            List<Authorization> result = new List<Authorization>();

            result = en.Authorizations.Where(x => x.Emp_CJ == EmployeeID).ToList();

            return result;
        }

        public List<Role> GetRoles()
        {
            List<Role> result = new List<Role>();

            result = en.Authorizations
                .Join(en.Roles, au => au.Role_ID, ro => ro.Role_ID, (au, ro) => ro)
                .Where(ro => ro.Deactive != true)
                .ToList();

            return result;
        }

        //public List<Rule> GetRules(List<Role> roles = null)
        //{
        //    List<Rule> result = new List<Rule>();
        //    if (roles.Count() <= 0)
        //        roles = GetRoles();

        //    foreach (var role in roles)
        //    {
        //        var rulesByRoleId = en.Roles.Where(ro => ro.Role_ID == role.Role_ID).SelectMany(ro => ro.Rules);

        //        rulesByRoleId = rulesByRoleId.Where(ru => ru.Deactive != true);

        //        result.AddRange(rulesByRoleId);
        //    }

        //    return result.Distinct().ToList();
        //}

        public List<Rule> GetRules(List<Role> roles = null, string moduleName = null)
        {
            List<Rule> result = new List<Rule>();
            if (roles.Count() <= 0)
                roles = GetRoles();

            foreach (var role in roles)
            {
                var rulesByRoleId = en.Roles.Where(ro => ro.Role_ID == role.Role_ID).SelectMany(ro => ro.Rules);

                rulesByRoleId = rulesByRoleId.Where(ru => ru.Deactive != true);

                result.AddRange(rulesByRoleId);
            }

            //if (string.IsNullOrEmpty(moduleName))
            //{
            //    result = result.Join(en.Modules, ru=>ru.module)
            //}

            return result.Distinct().ToList();
        }
    }
}