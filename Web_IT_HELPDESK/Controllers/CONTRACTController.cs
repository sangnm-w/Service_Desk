using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class CONTRACTController : Controller
    {
        public ServiceDeskEntities en { get; set; }
        public ApplicationUser _appUser { get; set; }
        public string currUserId { get; set; }
        public string currUserPlantId { get; set; }
        public string currUserDeptId { get; set; }

        public CONTRACTController()
        {
            en = new ServiceDeskEntities();
            _appUser = new ApplicationUser();
            currUserId = _appUser.EmployeeID;
            currUserPlantId = _appUser.GetPlantID();
            currUserDeptId = _appUser.GetDepartmentID();
        }

        // GET: Index
        [CustomAuthorize]
        public ActionResult Index()
        {
            bool currUserIsManager = _appUser.IsManager;
            ViewBag.currUserIsManager = currUserIsManager;

            var depts = en.Departments
                .Where(d => d.Plant_Id == currUserPlantId && d.Deactive != true)
                .Select(d => new DepartmentViewModel
                {
                    Department_Id = d.Department_Id,
                    Department_Name = d.Department_Name
                })
                .OrderByDescending(d => d.Department_Name).ToList();
            ViewBag.Departments = depts;
            ViewBag.Selected_depts = new List<string>();

            var contract_types = en.CONTRACT_TYPE;
            ViewBag.Contract_Types = contract_types;

            DateTime minDate = DateTime.Now;
            DateTime maxDate = DateTime.Now.AddYears(1);

            var contract_list = en.CONTRACTs.Where(c => c.DEL != true
                                                 && c.PLANT == currUserPlantId
                                                 && (minDate < c.DATE_MATURITY && c.DATE_MATURITY <= maxDate));
            if (currUserIsManager != true)
            {
                contract_list = contract_list.Where(c => c.DEPARTMENTID == currUserDeptId);
            }

            //Store value of filter parameters
            Dictionary<object, object> filter_Contracts = new Dictionary<object, object>();

            filter_Contracts.Add("filter_search", "");
            filter_Contracts.Add("filter_date", DateTime.Now);
            filter_Contracts.Add("filter_depts", null);
            filter_Contracts.Add("filter_contractType", "ALL");
            filter_Contracts.Add("filter_daynum", 365);

            Session.Clear();
            Session["filter_Contracts"] = filter_Contracts;

            return View(contract_list);
        }

        // POST: Index
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Index(string search_, DateTime? date_, ICollection<string> v_depts, string v_contract_type, int daynum_)
        {
            bool currUserIsManager = _appUser.isAdmin;

            ViewBag.currUserIsManager = currUserIsManager;

            var depts = en.Departments.Where(d => d.Plant_Id == currUserPlantId && d.Deactive == false)
               .Select(d => new DepartmentViewModel
               {
                   Department_Id = d.Department_Id,
                   Department_Name = d.Department_Name
               }
               ).OrderByDescending(d => d.Department_Name).ToList();
            ViewBag.Departments = depts;

            List<string> selectedDepts = new List<string>();
            if (v_depts == null)
            {
                ViewBag.Selected_depts = selectedDepts = depts.Select(m => m.Department_Id).ToList();
            }
            else
            {
                ViewBag.Selected_depts = selectedDepts = v_depts.ToList();
            }

            var contract_types = en.CONTRACT_TYPE;
            ViewBag.Contract_Types = contract_types;

            DateTime minDate = date_.Value.AddDays(-daynum_);
            DateTime maxDate = date_.Value;

            var contract_list = en.CONTRACTs.Where(c => c.DEL != true
                                                 && c.PLANT == currUserPlantId
                                                 && (minDate < c.DATE_MATURITY && c.DATE_MATURITY <= maxDate));

            if (currUserIsManager == true)
            {
                string v_dept_string = "";
                if (v_depts != null)
                {
                    foreach (var v_dept in v_depts)
                    {
                        v_dept_string += v_dept.ToString() + " ";
                    }
                    contract_list = contract_list.Where(c => v_dept_string.Trim().Contains(c.DEPARTMENTID));
                }
            }
            else
            {
                contract_list = contract_list.Where(c => c.USER_CREATE == currUserId);
            }


            if (v_contract_type != "ALL")
            {
                contract_list = contract_list.Where(c => c.CONTRACT_TYPE_ID == v_contract_type);
            }

            if (search_ != "")
            {
                contract_list = contract_list.Where(c => c.CONTRACTNAME.Contains(search_));
            }

            //Store value of filter parameters
            Dictionary<object, object> filter_Contracts = new Dictionary<object, object>();

            filter_Contracts.Add("filter_search", search_);
            filter_Contracts.Add("filter_date", date_);
            filter_Contracts.Add("filter_depts", v_depts);
            filter_Contracts.Add("filter_contractType", v_contract_type);
            filter_Contracts.Add("filter_daynum", daynum_);

            Session["filter_Contracts"] = filter_Contracts;
            //

            return View(contract_list);
        }

        // GET: CONTRACT/Create
        public ActionResult Create()
        {
            ContractViewModel.CreateContractViewModel _contractviewModel = new ContractViewModel.CreateContractViewModel();
            _contractviewModel.Contract_Types = en.CONTRACT_TYPE;
            _contractviewModel.Periods = en.PERIODs.ToList();

            return View(_contractviewModel);
        }

        // POST: CONTRACT/Create
        [HttpPost]
        public ActionResult Create(ContractViewModel.CreateContractViewModel _contractviewModel)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase contractFile = _contractviewModel.ContractFile;

                CONTRACT contract = _contractviewModel.CONTRACT;
                contract.ID = Guid.NewGuid();

                contract.NOTE = Path.GetFileName(contractFile.FileName);
                contract.DEPARTMENTID = currUserDeptId;
                contract.USER_CREATE = currUserId;
                contract.PLANT = currUserPlantId;
                contract.DATE_CREATE = DateTime.Now;

                contract.NOTE = Path.GetFileName(contractFile.FileName);
                contract.FILE_PATH = ContractHelper.SaveContractFile(contract, contractFile);

                en.CONTRACTs.Add(contract);

                Guid contract_id = contract.ID;

                foreach (var item in _contractviewModel.ContractSubViewModels)
                {
                    item.ContractSub.ID = Guid.NewGuid();
                    item.ContractSub.CONTRACTID = contract_id;
                    item.ContractSub.NOTE = Path.GetFileName(item.ContentSubFile.FileName);
                    item.ContractSub.USER_CREATE = currUserId;
                    item.ContractSub.PLANT = currUserPlantId;
                    item.ContractSub.NOTE = Path.GetFileName(item.ContentSubFile.FileName);
                    item.ContractSub.FILE_PATH = ContractHelper.SaveContractFile(item.ContractSub, contractFile);

                    en.CONTRACT_SUB.Add(item.ContractSub);
                }
                en.SaveChanges();
                return RedirectToAction("Index");
            }

            _contractviewModel.Contract_Types = en.CONTRACT_TYPE;
            _contractviewModel.Periods = en.PERIODs;
            return View(_contractviewModel);
        }

        // GET: CONTRACT/Edit
        public ActionResult Edit(Guid id)
        {
            ContractViewModel.Edit _contractviewmodel = new ContractViewModel.Edit();
            _contractviewmodel.Contract_Types = en.CONTRACT_TYPE;
            _contractviewmodel.Periods = en.PERIODs;

            var c = en.CONTRACTs.FirstOrDefault(i => i.ID == id && i.DEL != true);
            _contractviewmodel.CONTRACT = c;

            var cs = en.CONTRACT_SUB.Where(o => o.CONTRACTID == id && o.DEL != true).OrderByDescending(o => o.DATE).ToList();
            _contractviewmodel.countContractSubModel = cs.Count;
            foreach (CONTRACT_SUB contractSub in cs)
            {
                _contractviewmodel.ContractSubViewModels.Add(new ContractSubViewModel.Edit() { ContractSub = contractSub });
            }
            return View(_contractviewmodel);
        }

        // POST: CONTRACT/Edit
        [HttpPost]
        public ActionResult Edit(ContractViewModel.Edit _contractviewModel)
        {
            //check subfile of new contractsub
            int subCount = 0;
            foreach (var item in _contractviewModel.ContractSubViewModels)
            {
                if (item.ContractSub.ID == Guid.Empty && item.ContentSubFile == null)
                {
                    ModelState.AddModelError("ContractSubViewModels[" + subCount + "].ContentSubFile", "File not found!");
                }
                subCount++;
            }

            if (ModelState.IsValid)
            {
                // Contract Part
                CONTRACT contract = _contractviewModel.CONTRACT;
                HttpPostedFileBase contractFile = _contractviewModel.ContractFile;

                CONTRACT modifyContract = en.CONTRACTs.FirstOrDefault(c => c.ID == contract.ID);

                modifyContract.VENDOR = contract.VENDOR;
                modifyContract.PHONE = contract.PHONE;
                modifyContract.ADDRESS = contract.ADDRESS;
                modifyContract.CONTRACTNAME = contract.CONTRACTNAME;
                modifyContract.REPRESENTATION = contract.REPRESENTATION;
                modifyContract.CONTRACT_TYPE_ID = contract.CONTRACT_TYPE_ID;
                modifyContract.PERIODID = contract.PERIODID;
                modifyContract.DATE = contract.DATE;
                modifyContract.MONTHS = contract.MONTHS;
                modifyContract.DATE_MATURITY = modifyContract.DATE.Value.AddMonths((int)modifyContract.MONTHS);

                if (contractFile != null)
                {
                    modifyContract.NOTE = Path.GetFileName(contractFile.FileName);
                    modifyContract.FILE_PATH = ContractHelper.SaveContractFile(modifyContract, contractFile);
                }

                en.Entry(modifyContract).State = EntityState.Modified;
                en.SaveChanges();

                // Contract Sub Part
                List<ContractSubViewModel.Edit> csVMs = _contractviewModel.ContractSubViewModels;
                List<CONTRACT_SUB> oldSubs = en.CONTRACT_SUB.Where(s => s.CONTRACTID == contract.ID && s.DEL != true).ToList();
                List<CONTRACT_SUB> newSubs = csVMs.Select(csVM => csVM.ContractSub).ToList();

                // Delete oldSubs have removed in View
                foreach (CONTRACT_SUB oldSub in oldSubs)
                {
                    if (!newSubs.Contains(oldSub))
                    {
                        oldSub.DEL = true;
                        en.Entry(oldSub).State = EntityState.Modified;
                        en.SaveChanges();
                    }
                }

                // Insert or modified sub in newSubs
                oldSubs = en.CONTRACT_SUB.Where(s => s.CONTRACTID == contract.ID && s.DEL != true).ToList();
                int i = 0;
                foreach (ContractSubViewModel.Edit csVM in csVMs)
                {
                    CONTRACT_SUB sub = csVM.ContractSub;
                    HttpPostedFileBase subFile = csVM.ContentSubFile;
                    if (oldSubs.Contains(sub))                                                                  // newSub existed in database => modify
                    {
                        CONTRACT_SUB modifySub = en.CONTRACT_SUB.FirstOrDefault(c => c.ID == sub.ID);

                        modifySub.DATE = sub.DATE;
                        modifySub.SUBNAME = sub.SUBNAME;
                        modifySub.PERIODID = sub.PERIODID;

                        if (subFile != null)
                        {
                            modifySub.NOTE = Path.GetFileName(subFile.FileName);
                            modifySub.FILE_PATH = ContractHelper.SaveContractFile(modifySub, subFile);
                        }

                        en.Entry(modifySub).State = EntityState.Modified;
                    }
                    else                                                                                        // newSub not existed in database => insert
                    {
                        sub.ID = Guid.NewGuid();
                        sub.CONTRACTID = contract.ID;
                        sub.USER_CREATE = currUserId;
                        sub.PLANT = currUserPlantId;

                        if (subFile != null)
                        {
                            sub.NOTE = Path.GetFileName(subFile.FileName);
                            sub.FILE_PATH = ContractHelper.SaveContractFile(sub, subFile);
                        }
                        else
                        {
                            ModelState.AddModelError("ContractSubViewModels[" + i + "].ContentSubFile", "File not found!");

                            _contractviewModel.Contract_Types = en.CONTRACT_TYPE;
                            _contractviewModel.Periods = en.PERIODs.ToList();

                            return View(_contractviewModel);
                        }

                        en.CONTRACT_SUB.Add(sub);
                    }
                    en.SaveChanges();
                    i++;
                }

                return RedirectToAction("Index");
            }

            _contractviewModel.Contract_Types = en.CONTRACT_TYPE;
            _contractviewModel.Periods = en.PERIODs.ToList();

            return View(_contractviewModel);
        }

        public FileContentResult Download()
        {
            Dictionary<object, object> filter_Contracts = (Dictionary<object, object>)Session["filter_Contracts"];

            string filter_search = filter_Contracts["filter_search"] != null ? filter_Contracts["filter_search"].ToString() : "";
            DateTime? filter_date = filter_Contracts["filter_date"] != null ? Convert.ToDateTime(filter_Contracts["filter_date"]) : DateTime.Now;
            List<string> filter_depts = filter_Contracts["filter_depts"] != null ? ((ICollection<string>)filter_Contracts["filter_depts"]).ToList() : null;
            string filter_contractType = filter_Contracts["filter_contractType"] != null ? filter_Contracts["filter_contractType"].ToString() : "ALL";
            int filter_daynum = filter_Contracts["filter_daynum"] != null ? Convert.ToInt32(filter_Contracts["filter_daynum"]) : 365;

            bool currUserIsManager = _appUser.IsManager;
            var contracts = en.CONTRACTs.Where(c => c.DEL != true
                                     && c.PLANT == currUserPlantId
                                     && filter_date <= DbFunctions.AddDays(c.DATE, filter_daynum)).ToList();

            if (currUserIsManager == true)
            {
                string v_dept_string = "";
                if (filter_depts != null)
                {
                    foreach (var v_dept in filter_depts)
                    {
                        v_dept_string += v_dept.ToString() + " ";
                    }
                    contracts = contracts.Where(c => v_dept_string.Trim().Contains(c.DEPARTMENTID)).ToList();
                }
            }
            else
            {
                contracts = contracts.Where(c => c.USER_CREATE == CurrentUser.Instance.User.Emp_CJ).ToList();
            }


            if (filter_contractType != "ALL")
            {
                contracts = contracts.Where(c => c.CONTRACT_TYPE_ID == filter_contractType).ToList();
            }

            if (filter_search != "")
            {
                contracts = contracts.Where(c => c.CONTRACTNAME.Contains(filter_search)).ToList();
            }

            List<ContractViewModel.Excel> contractsForExport = ContractModel.Instance.GetContractsForExport(currUserDeptId, currUserPlantId, contracts.ToList());

            List<int> colDates = new List<int>()
            {
                9, 11, 16
            };

            var stream = ExcelHelper.Instance.CreateExcelFile(null, contractsForExport, ExcelTitle.Instance.Contracts(), colDates);
            var buffer = stream as MemoryStream;
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "Contract Report.xlsx";
            return File(buffer.ToArray(), contentType, fileName);
        }

        [HttpGet]
        public FileResult Get_file(Guid? con_id)
        {
            var contract = en.CONTRACTs.FirstOrDefault(c => c.ID == con_id);
            string contractFile = HostingEnvironment.MapPath(contract.FILE_PATH);
            var mimeType = MimeMapping.GetMimeMapping(contractFile);

            return File(contractFile, mimeType, Path.GetFileName(contractFile));
        }

        [HttpGet]
        public FileResult Get_sub(Guid? sub_id)
        {
            var sub = en.CONTRACT_SUB.FirstOrDefault(c => c.ID == sub_id);
            string subFile = HostingEnvironment.MapPath(sub.FILE_PATH);
            var mimeType = MimeMapping.GetMimeMapping(subFile);

            return File(subFile, mimeType, Path.GetFileName(subFile));
        }
    }
}


