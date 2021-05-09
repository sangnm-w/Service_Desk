using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels.Mailing
{
    public class MailingCreateViewModel
    {
        //filter part
        public IEnumerable<Plant> Plants { get; set; }
        public IEnumerable<DepartmentViewModel> DepartmentVMs { get; set; }
        public string PlantId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }

        //mail model part
        [Display(Name = "Title")]
        public string MailTitle { get; set; }
        [Display(Name = "Content")]
        public string MailContent { get; set; }
        [Display(Name = "From")]
        public string FromAddress { get; set; } // SenderEmail: Auto laod currEmployee Email. Can edit to other email.
        public string ToAddress { get; set; }
        [Display(Name = "Attachment")]
        public string Attachment { get; set; }
        public string MailPicture { get; set; }
        public string EmployeeId { get; set; }

        //other part
        [Display(Name = "Password")]
        public string SenderPW { get; set; }

    }
}