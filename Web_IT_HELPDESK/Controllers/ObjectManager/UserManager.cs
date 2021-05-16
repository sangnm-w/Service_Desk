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
            var user = from i in en.Employees where i.Emp_CJ == userLogIn && i.Deactive != true select i;
            if (user.ToList().Count > 0)
                return user.First().Password;
            else
                return string.Empty;
        }
    }
}