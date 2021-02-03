using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Models
{
    public class CurrentUser
    {
        private CurrentUser()
        {
            sessionUserID = HttpContext.Current.User.Identity.Name;
            entity = new ServiceDeskEntities();
        }
        private static CurrentUser _instance;

        public static CurrentUser Instance { get { if (_instance == null || sessionUser_Changed()) _instance = new CurrentUser(); return _instance; } private set => _instance = value; }

        private static string sessionUserID = null;
        private ServiceDeskEntities entity = null;
        private Employee_New _user;
        public Employee_New User
        {
            get
            {
                if (_user == null)
                {
                    //_user = entity.Employees.FirstOrDefault(e => e.Emp_CJ == sessionUserID);
                    _user = entity.Employee_New.FirstOrDefault(e => e.Emp_CJ == sessionUserID);
                }
                return _user;
            }
            private set => _user = value;
        }


        private List<Rights_Management> _rights;
        public List<Rights_Management> Rights
        {
            get
            {
                if (_rights == null)
                {
                    _rights = entity.Rights_Management.Where(r => r.Employee_Id == sessionUserID).ToList();
                }
                return _rights;
            }
            private set => _rights = value;
        }

        private bool? _isAdministrator = null;
        public bool? isAdministrator
        {
            get
            {
                if (_isAdministrator == null)
                {
                    _isAdministrator = Rights.FirstOrDefault(r => r.Role_Id == 1) != null;
                }
                return _isAdministrator;
            }
            private set => _isAdministrator = value;
        }


        public bool? hasPermission(string actionName, string moduleName)
        {
            if (isAdministrator == true)
            {
                return true;
            }

            List<Rule> userRules = RulesByModuleName(moduleName);

            if (userRules.Count > 0)
            {
                return userRules.FirstOrDefault(r => r.Rule_Name == actionName) != null;
            }

            return false;
        }


        public List<Rule> RulesByModuleName(string moduleName)
        {
            return entity.Rights_Management
                    .Join
                    (
                        entity.Rules,
                        a => a.Rule_Id,
                        b => b.Id,
                        (a, b) => new { a, b }
                    )
                    .Where(temp0 => temp0.b.Module_Name == moduleName && temp0.a.Employee_Id == sessionUserID)
                    .Select(temp0 => temp0.b)
                    .ToList();
        }

        private static bool sessionUser_Changed()
        {
            return sessionUserID != HttpContext.Current.User.Identity.Name;
        }


    }
}