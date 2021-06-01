using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels.Mailing
{
    public class MailingEmailsViewModel
    {
        [Display(Name = "Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Position")]
        public string Position { get; set; }
    }
}