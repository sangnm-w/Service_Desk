using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class UserManager
    {
        ServiceDeskEntities en = new ServiceDeskEntities();
        public string GetUserPassword(string userLogIn)
        {
            //var user = from i in en.Employees where i.Emp_CJ == userLogIn select i;
            var user = from i in en.Employee_New where i.Emp_CJ == userLogIn select i;
            if (user.ToList().Count > 0)
                return user.First().Password;
            else
                return string.Empty;
        }

        public string GetUserPlant(string userLogIn)
        {
            //var user = from i in en.Employees where i.Emp_CJ == userLogIn select i;
            var user = from i in en.Employee_New where i.Emp_CJ == userLogIn select i;
            if (user.ToList().Count > 0)
                return user.First().Plant_ID;
            else
                return string.Empty;
        } 
    }
}