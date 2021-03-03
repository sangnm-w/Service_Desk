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
        public class Edit
        {
            public Edit()
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
        public class Excel
        {
                public Excel() { }

                public Excel(CONTRACT c, string orderName)
                {
                    OrderName = orderName;
                    VENDOR = c.VENDOR;
                    ADDRESS = c.ADDRESS;
                    PHONE = c.PHONE;
                    CONTRACTNAME = c.CONTRACTNAME;
                    CONTRACT_TYPE = c.CONTRACT_TYPE?.CONTRACT_TYPE_NAME;
                    PERIOD = c.PERIOD?.PERIOD_NAME;
                    REPRESENTATION = c.REPRESENTATION;
                    DATE = c.DATE;
                    MONTHS = c.MONTHS;
                    DATE_MATURITY = c.DATE_MATURITY;
                    NOTE = c.NOTE;
                    DEPARTMENTID = c.DEPARTMENTID;
                    PLANT = c.PLANT;
                    USER_CREATE = c.USER_CREATE;
                    DATE_CREATE = c.DATE_CREATE;
                }

                public Excel(CONTRACT_SUB s, string orderName)
                {
                    OrderName = orderName;
                    CONTRACTNAME = s.SUBNAME;
                    PERIOD = s.PERIOD?.PERIOD_NAME;
                    DATE = s.DATE;
                    NOTE = s.NOTE;
                }

                public string OrderName { get; set; }
                public string VENDOR { get; set; }
                public string ADDRESS { get; set; }
                public string PHONE { get; set; }
                public string CONTRACTNAME { get; set; }
                public string CONTRACT_TYPE { get; set; }
                public string PERIOD { get; set; }
                public string REPRESENTATION { get; set; }
                public DateTime? DATE { get; set; }
                public int? MONTHS { get; set; }
                public DateTime? DATE_MATURITY { get; set; }
                public string NOTE { get; set; }
                public string DEPARTMENTID { get; set; }
                public string PLANT { get; set; }
                public string USER_CREATE { get; set; }
                public DateTime? DATE_CREATE { get; set; }
            }
        }
    
}