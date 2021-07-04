using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels.Mailing
{
    public class MailingPositionViewModel
    {
        public string PositionId { get; set; }
        public string PositionName { get; set; }
    }

    public enum Position
    {
        HeadOfDepartment,
        Manager,
        Sales
    }
}