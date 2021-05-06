using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class MailingViewModel
    {
        public class IndexVM
        {
            public Guid MailID { get; set; }
            [Display(Name = "Mail Title")]
            public string MailTitle { get; set; }
            [Display(Name = "From")]
            public string FromAddress { get; set; }
            [Display(Name = "To")]
            public string ToAddress { get; set; }
            [Display(Name = "CC")]
            public string CcAddress { get; set; }
            [Display(Name = "Attachment")]
            public string Attachment { get; set; }
            [Display(Name = "Employee")]
            public string EmployeeName { get; set; }
            [Display(Name = "Sending Date")]
            public DateTime? SendingDate { get; set; }
            [Display(Name = "Status")]
            public string SendingStatus { get; set; }
            [Display(Name = "Department")]
            public string DepartmentName { get; set; }
        }
    }
}