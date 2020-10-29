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
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }

            private string _EmployeeField;

            public string EmployeeField
            {
                get { return EmployeeID + " - " + EmployeeName; }
                set { _EmployeeField = value; }
            }
        }
    }
}