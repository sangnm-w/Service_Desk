using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class ContractSubViewModel
    {
        public CONTRACT_SUB ContractSub { get; set; }
        [Required(ErrorMessage = "Á à!")]
        public HttpPostedFileBase ContentSubFile { get; set; }
    }
}