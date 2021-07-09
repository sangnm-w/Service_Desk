using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.ViewModels.Mailing
{
    public class MailingCreateViewModel
    {
        //filter part
        public SelectList Plants { get; set; }
        public SelectList Departments { get; set; }
        public SelectList Positions { get; set; }
        public string PlantId { get; set; }
        public string DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }

        //search result table
        public IEnumerable<MailingEmailsViewModel> InitialEmployees { get; set; }

        //receivers table
        public IEnumerable<MailingEmailsViewModel> Receivers { get; set; }

        //mail model part
        [Required]
        public string MailTitle { get; set; }
        [Required]
        [AllowHtml]
        public string MailContent { get; set; }
        public string FromAddress { get; set; } // SenderEmail: Auto load currEmployee Email. Can edit to other email.
        public string ToAddress { get; set; }
        [Display(Name = "Attachment")]
        public List<HttpPostedFileBase> Attachments { get; set; }
        public string MailPicture { get; set; }
        public string EmployeeId { get; set; }

        //other part
        [DataType(DataType.Password)]
        public string SenderPW { get; set; }

    }
}