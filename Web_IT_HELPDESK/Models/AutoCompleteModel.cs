using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Models
{
    public class AutoCompleteModel
    {
        public class EmployeeFieldModel
        {
            public string Emp_CJ { get; set; }
            public string EmployeeName { get; set; }

            private string _EmployeeField;

            public string EmployeeField
            {
                get { return Emp_CJ + " - " + EmployeeName; }
                set { _EmployeeField = value; }
            }
        }
    }
}