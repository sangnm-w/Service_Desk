using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class UserManager
    {
        Web_IT_HELPDESKEntities en = new Web_IT_HELPDESKEntities();
        public string GetUserPassword(string userLogIn)
        {
            var user = from i in en.Employees where i.EmployeeID == userLogIn select i;
            if (user.ToList().Count > 0)
                return user.First().Password;
            else
                return string.Empty;
        }

        public string GetUserPlant(string userLogIn)
        {
            var user = from i in en.Employees where i.EmployeeID == userLogIn select i;
            if (user.ToList().Count > 0)
                return user.First().Plant;
            else
                return string.Empty;
        } 
    }
}