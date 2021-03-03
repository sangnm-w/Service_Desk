using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Models
{
    public class ContractModel
    {
        private static ContractModel instance;

        public static ContractModel Instance { get { if (instance == null) instance = new ContractModel(); return instance; } set => instance = value; }
        private ContractModel() { }

        public List<ContractViewModel.Excel> GetContractsForExport(string deptID, string plantID, List<CONTRACT> contracts = null)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            List<ContractViewModel.Excel> contractAndSubs = new List<ContractViewModel.Excel>();

            if (contracts == null || contracts.Count <= 0)
            {
                contracts = en.CONTRACTs.Where(model => model.DEL != true && model.DEPARTMENTID == deptID && model.PLANT == plantID).ToList();
            }

            int countContract = 0, countsub = 0;
            foreach (var c in contracts)
            {
                ContractViewModel.Excel conEx = new ContractViewModel.Excel(c, "Contract" + countContract);
                contractAndSubs.Add(conEx);

                List<CONTRACT_SUB> subs = en.CONTRACT_SUB.Where(model => model.CONTRACTID == c.ID && model.DEL != true).ToList();
                if (subs.Count > 0)
                {
                    foreach (var s in subs)
                    {
                        ContractViewModel.Excel subEx = new ContractViewModel.Excel(s, "Contract_" + countContract + " - Sub_" + countsub);
                        contractAndSubs.Add(subEx);
                    }
                }
                //List<CONTRACT_SUB> subs = new List<CONTRACT_SUB>();
                //subs = raws.Where(model => model.Contract.ID == c.Contract.ID).Select(model => model.Sub).ToList();
                //countsub = 0;
                //foreach (var s in subs)
                //{
                //    ContractViewModel.Excel.ContractAndSub sub = new ContractViewModel.Excel.ContractAndSub(s, "Sub-Contract" + countsub);
                //    countsub++;
                //}
                countContract++;
            }

            return contractAndSubs;
        }
    }
}