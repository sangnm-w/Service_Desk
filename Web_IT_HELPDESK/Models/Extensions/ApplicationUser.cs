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
        public string EmployeeName { get; set; }
        private ServiceDeskEntities en { get; set; }

        private ApplicationUser()
        {
            en = new ServiceDeskEntities();
            EmployeeID = HttpContext.Current.User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(EmployeeID))
                EmployeeName = en.Employee_New.Find(EmployeeID).Employee_Name;
        }

        private bool _isAdmin = false;

        private bool _isManager = false;

        private bool? _isDeactive = false;

        public bool isAdmin
        {
            get
            {
                _isAdmin = en.Authorizations.Where(a => a.Emp_CJ == EmployeeID).Select(a => a.Role_ID).Contains(1);

                return _isAdmin;
            }
            set { _isAdmin = value; }
        }
        public bool IsManager
        {
            get
            {
                string managerIdOfUser = GetManagerIdOfUser();
                _isManager = string.Equals(managerIdOfUser, EmployeeID);
                return _isManager;
            }
            set { _isManager = value; }
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

        public string GetDepartmentID()
        {
            string result = null;

            result = en.Employee_New.Find(EmployeeID).Department_ID;

            return result;
        }

        public string GetDepartmentName()
        {
            string result = null;

            result = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                .FirstOrDefault(grp => grp.e.Emp_CJ == EmployeeID)
                .d.Department_Name
                .ToString();

            return result;
        }

        public string GetPlantID()
        {
            string result = null;

            result = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                .FirstOrDefault(grp => grp.e.Emp_CJ == EmployeeID)
                .d.Plant_ID
                .ToString();

            return result;
        }

        public string GetPlantName()
        {
            string result = null;

            result = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                .Join(en.Plants, grp => grp.d.Plant_ID, p => p.Plant_ID, (grp, p) => new { grp.e, grp.d, p })
                .FirstOrDefault(j => j.e.Emp_CJ == EmployeeID)
                .p.Plant_Name
                .ToString();

            return result;
        }

        public string GetManagerIdOfUser()
        {
            string result = null;

            result = en.Employee_New
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == EmployeeID)
                    .d.Manager_ID
                    .ToString();

            return result;
        }


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
                .Join(en.Roles, au => au.Role_ID, ro => ro.Role_ID, (au, ro) => new { au, ro })
                .Where(joinedI => joinedI.ro.Deactive != true && joinedI.au.Emp_CJ == EmployeeID)
                .Select(joinedI => joinedI.ro)
                .Distinct()
                .ToList();

            return result;
        }

        public List<Rule> GetRules(List<Role> roles = null, string moduleName = null)
        {
            List<Rule> result = new List<Rule>();
            if (roles == null || roles.Count() <= 0)
                roles = GetRoles();

            foreach (var role in roles)
            {
                var rulesByRoleId = en.Roles.Where(ro => ro.Role_ID == role.Role_ID)
                    .SelectMany(ro => ro.Rules)
                    .Where(ru => ru.Deactive != true);

                result.AddRange(rulesByRoleId);
            }

            if (!string.IsNullOrEmpty(moduleName))
            {
                result = result.Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
                    .Where(grp => grp.mo.Name.ToUpper() == moduleName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();
            }

            return result.Distinct().ToList();
        }
    }
}