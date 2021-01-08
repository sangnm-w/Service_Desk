using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class ContractSubViewModel
    {
        public class Create
        {
            public CONTRACT_SUB ContractSub { get; set; }
            [Required(ErrorMessage = "File not found!")]
            public HttpPostedFileBase ContentSubFile { get; set; }
        }
        public class Edit
        {
            public CONTRACT_SUB ContractSub { get; set; }
            public HttpPostedFileBase ContentSubFile { get; set; }
        }
    }
}