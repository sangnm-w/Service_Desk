using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.Models
{
    public partial class ContractViewModel
    {
        public virtual CONTRACT CONTRACT { get; set; }
        public virtual CONTRACT_SUB CONTRACT_SUB { get; set; }
        public virtual PERIOD PERIOD { get; set; }
        public virtual CONTRACT_TYPE CONTRACT_TYPE { get; set; }
        public string CONTRACT_ID { get; set; }

        public ContractViewModel()
        {
        }
    }
}