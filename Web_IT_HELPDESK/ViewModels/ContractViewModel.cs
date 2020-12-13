using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.ViewModels
{
    public class ContractViewModel
    {
        public CONTRACT CONTRACT { get; set; }
        [Required]
        public HttpPostedFileBase ContractFile { get; set; }
        public List<ContractSubViewModel> ContractSubViewModels { get; set; }
        public int countContractSubModel { get; set; } = 0;
        public int countRemoveSub { get; set; } = 0;
        public IEnumerable<CONTRACT_TYPE> Contract_Types { get; set; }
        public IEnumerable<PERIOD> Periods { get; set; }

        public ContractViewModel()
        {
            ContractSubViewModels = new List<ContractSubViewModel>();
            for (int i = 0; i < 10; i++)
            {
                ContractSubViewModels.Add(new ContractSubViewModel());
            }
        }
    }
}