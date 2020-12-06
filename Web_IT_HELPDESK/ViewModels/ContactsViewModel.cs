using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class ContactsViewModel
    {
        public string Emp_CJ { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
    }
}