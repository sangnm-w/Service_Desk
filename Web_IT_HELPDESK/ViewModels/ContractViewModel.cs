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
        public class CreateContractViewModel
        {
            public CreateContractViewModel()
            {
                ContractSubViewModels = new List<ContractSubViewModel.Create>();
            }
            public CONTRACT CONTRACT { get; set; }
            [Required]
            public HttpPostedFileBase ContractFile { get; set; }
            public List<ContractSubViewModel.Create> ContractSubViewModels { get; set; }
            public int countContractSubModel { get; set; } = 0;
            public int countRemoveSub { get; set; } = 0;
            public IEnumerable<CONTRACT_TYPE> Contract_Types { get; set; }
            public IEnumerable<PERIOD> Periods { get; set; }
        }
        public class EditContractViewModel
        {
            public EditContractViewModel()
            {
                ContractSubViewModels = new List<ContractSubViewModel.Edit>();
            }
            public CONTRACT CONTRACT { get; set; }
            public HttpPostedFileBase ContractFile { get; set; }
            public List<ContractSubViewModel.Edit> ContractSubViewModels { get; set; }
            public int countContractSubModel { get; set; } = 0;
            public int countRemoveSub { get; set; } = 0;
            public IEnumerable<CONTRACT_TYPE> Contract_Types { get; set; }
            public IEnumerable<PERIOD> Periods { get; set; }
        }
    }
}