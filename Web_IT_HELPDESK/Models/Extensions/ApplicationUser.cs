using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Models.Extensions
{
    public class ApplicationUser
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        private ServiceDeskEntities en { get; set; }

        public ApplicationUser()
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
                .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                .FirstOrDefault(grp => grp.e.Emp_CJ == EmployeeID)
                .d.Department_Name
                .ToString();

            return result;
        }

        public string GetPlantID()
        {
            string result = null;

            result = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                .FirstOrDefault(grp => grp.e.Emp_CJ == EmployeeID)
                .d.Plant_Id
                .ToString();

            return result;
        }

        public string GetPlantName()
        {
            string result = null;

            result = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                .Join(en.Plants, grp => grp.d.Plant_Id, p => p.Plant_Id, (grp, p) => new { grp.e, grp.d, p })
                .FirstOrDefault(j => j.e.Emp_CJ == EmployeeID)
                .p.Plant_Name
                .ToString();

            return result;
        }

        public string GetManagerIdOfUser()
        {
            string result = null;

            result = en.Employee_New
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == EmployeeID)
                    .d.Manager_Id;

            return result;
        }

        public IEnumerable<Authorization> GetAuths()
        {
            var result = en.Authorizations.Where(x => x.Emp_CJ == EmployeeID).ToList();
            return result;
        }

        public IEnumerable<Authorization> GetAuthsByModuleName(string moduleName)
        {
            var ruleIdByModule = en.Rules.Where(ru => ru.Module.Module_Name.ToUpper() == moduleName.ToUpper()).Select(ru => ru.Rule_ID);

            var roleIdByModule = en.Rules.Where(mm => ruleIdByModule.Contains(mm.Rule_ID)).SelectMany(mm => mm.Roles).Select(ro => ro.Role_ID);

            var result = en.Authorizations.Where(au => roleIdByModule.Contains(au.Role_ID)).Where(au => au.Emp_CJ == EmployeeID);

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

        public List<Role> GetRolesByModuleName(string moduleName)
        {
            List<Role> result = new List<Role>();

            var authsByModuleName = GetAuthsByModuleName(moduleName);

            result = authsByModuleName.Select(au => au.Role).Distinct().ToList();

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
                    .Where(grp => grp.mo.Module_Name.ToUpper() == moduleName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();
            }

            return result.Distinct().ToList();
        }

        public List<Plant> GetAuthoPlantsByModuleName(string moduleName)
        {
            var authsByModuleName = GetAuthsByModuleName(moduleName);

            var result = authsByModuleName.Join(en.Plants, au => au.Plant_ID, p => p.Plant_Id, (au, p) => p).Distinct().ToList();

            return result;
        }
    }
}