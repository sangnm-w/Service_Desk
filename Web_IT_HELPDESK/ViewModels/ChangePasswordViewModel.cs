using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPsw { get; set; }
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPsw { get; set; }
        [Required]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        public string NewPsw_Confirm { get; set; }
    }
}