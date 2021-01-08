using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class CONTRACTController : Controller
    {
        ServiceDeskEntities en = new ServiceDeskEntities();
        // GET: Index
        [Authorize]
        public ActionResult Index()
        {
            #region closed - M$ 20201103
            //DateTime today = DateTime.Today;

            //int v_year = today.Year;
            //int v_month = today.Month;
            //string dept=GetDept_id(System.Web.HttpContext.Current.User.Identity.Name);
            //string v_plant = GetPlant_id(System.Web.HttpContext.Current.User.Identity.Name);
            //var contract_list = en.CONTRACTs.Where(o => o.DEL != true && today <= DbFunctions.AddDays(o.DATE, 45) && o.DEPARTMENTID == dept && o.PLANT == v_plant).OrderBy(i => i.DATE); 
            //                                                                    // EntityFunctions.AddDays lấy ngày hiện tại so sánh 30 ngày ngày trong quá khứ 
            ////int pageSize = 1;
            ////int pageNumber = (page ?? 1);
            #endregion

            string curr_plantId = CurrentUser.Instance.User.Plant_Id;
           
            var depts = en.Departments.Where(d => d.Plant_Id == curr_plantId && d.Deactive != true).Select(d => new DepartmentViewModel
            {
                Department_Id = d.Department_Id,
                Department_Name = d.Department_Name
            }).OrderByDescending(d => d.Department_Name).ToList();
            ViewBag.Departments = depts;

            var contract_types = en.CONTRACT_TYPE;
            ViewBag.Contract_Types = contract_types;

            var contracts = en.CONTRACTs.Where(c => c.DEL != true && DateTime.Now <= DbFunctions.AddDays(c.DATE, 30));
            return View(contracts);
        }

        // POST: Index
        [Authorize]
        [HttpPost]
        public ActionResult Index(string search_, DateTime? date_, ICollection<string> v_depts, string v_contract_type, int daynum_)
        {
            string curr_PlantId = CurrentUser.Instance.User.Plant_Id;
            string curr_DeptId = CurrentUser.Instance.User.Department_Id;
            bool currUserIsManager = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_PlantId && d.Department_Id == curr_DeptId && d.Manager_Id == CurrentUser.Instance.User.Emp_CJ) != null ? true : false;
            ViewBag.currUserIsManager = currUserIsManager;

            var depts = en.Departments.Where(d => d.Plant_Id == curr_PlantId && d.Deactive == false)
               .Select(d => new DepartmentViewModel
               {
                   Department_Id = d.Department_Id,
                   Department_Name = d.Department_Name
               }
               ).OrderByDescending(d => d.Department_Name).ToList();
            ViewBag.Departments = depts;

            var contract_types = en.CONTRACT_TYPE;
            ViewBag.Contract_Types = contract_types;

            IEnumerable<CONTRACT> contract_list;

            contract_list = en.CONTRACTs.Where(c => c.DEL != true
                                                 && c.PLANT == curr_PlantId
                                                 && date_ <= DbFunctions.AddDays(c.DATE, daynum_));



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
                contract_list = contract_list.Where(c => c.USER_CREATE == CurrentUser.Instance.User.Emp_CJ);
            }


            if (v_contract_type != "ALL")
            {
                contract_list = contract_list.Where(c => c.CONTRACT_TYPE_ID == v_contract_type);
            }

            if (search_ != "")
            {
                contract_list = contract_list.Where(c => c.CONTRACTNAME.Contains(search_));
            }

            #region hide
            //if (v_depts != null)
            //{
            //    if ((search_ == "" || search_ == "search") && v_depts.Count == 12 && v_contract_type == "ALL")
            //    {
            //        contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                  && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                  && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
            //    }
            //    else if ((search_ == "" || search_ == "search") && v_depts.Count == 12 && v_contract_type != "ALL")
            //    {
            //        contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                 && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
            //                                                 && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                 && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
            //    }
            //    else if (search_ == "" || search_ == "search" && v_contract_type == "ALL")
            //    {
            //        foreach (var v_dept in v_depts)
            //        {
            //            v_dept_string += v_dept.ToString() + " ";
            //        }

            //        contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                    && v_dept_string.Trim().Contains(o.DEPARTMENTID)
            //                                                    && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                    && v_dept_string.Trim().Contains(o.DEPARTMENTID)
            //                                                    && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
            //    }
            //    else if (search_ == "" || search_ == "search" && v_contract_type != "ALL")
            //    {
            //        foreach (var v_dept in v_depts)
            //        {
            //            v_dept_string += v_dept.ToString() + " ";
            //        }

            //        contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                 && v_dept_string.Trim().Contains(o.DEPARTMENTID)
            //                                                 && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
            //                                                 && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                 && o.PLANT == curr_plantId
            //                                                 && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
            //    }
            //    else if (v_contract_type == "ALL" && (search_ != "" || search_ != "search"))
            //    {
            //        foreach (var v_dept in v_depts)
            //        {
            //            v_dept_string += v_dept.ToString() + " ";
            //        }

            //        contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                 && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
            //                                                 && o.CONTRACTNAME.Contains(search_) == true && v_dept_string.Trim().Contains(o.DEPARTMENTID)
            //                                                 && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                 && o.PLANT == curr_plantId
            //                                                 && o.CONTRACTNAME.Contains(search_) == true && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
            //    }
            //    else if (v_contract_type == "ALL")
            //    {
            //        foreach (var v_dept in v_depts)
            //        {
            //            v_dept_string += v_dept.ToString() + " ";
            //        }

            //        contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                 && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
            //                                                 && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                 && o.PLANT == curr_plantId
            //                                                 && o.CONTRACTNAME.Contains(search_) == true && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
            //    }
            //    else
            //    {
            //        foreach (var v_dept in v_depts)
            //        {
            //            v_dept_string += v_dept.ToString() + " ";
            //        }

            //        contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                 && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                 && o.CONTRACTNAME.Contains(search_) == true
            //                                                 && o.PLANT == curr_plantId
            //                                                 && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
            //    }
            //}
            //else
            //{
            //    contract_list = en.CONTRACTs.Where(o => o.DEL != true
            //                                                  && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
            //                                                  && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
            //} 
            #endregion

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

                byte[] contentData = new byte[contractFile.InputStream.Length];
                contractFile.InputStream.Read(contentData, 0, Convert.ToInt32(contractFile.InputStream.Length));
                Binary contentBinary = new Binary(contentData);

                //var fileName = Path.GetFileName(_contractviewModel.ContractFile.FileName);
                //var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                //_contractviewModel.ContractFile.SaveAs(path);

                CONTRACT contract = _contractviewModel.CONTRACT;
                contract.ID = Guid.NewGuid();
                contract.CONTENT = contentBinary.ToArray();
                contract.NOTE = System.IO.Path.GetFileName(contractFile.FileName);
                contract.DEPARTMENTID = CurrentUser.Instance.User.Department_Id;
                contract.USER_CREATE = CurrentUser.Instance.User.Emp_CJ;
                contract.PLANT = CurrentUser.Instance.User.Plant_Id;
                contract.DATE_CREATE = DateTime.Now;
                en.CONTRACTs.Add(contract);

                Guid contract_id = contract.ID;

                foreach (var item in _contractviewModel.ContractSubViewModels)
                {
                    byte[] subcontentData = new byte[item.ContentSubFile.InputStream.Length];
                    item.ContentSubFile.InputStream.Read(subcontentData, 0, Convert.ToInt32(item.ContentSubFile.InputStream.Length));
                    Binary subcontentBinary = new Binary(subcontentData);

                    item.ContractSub.ID = Guid.NewGuid();
                    item.ContractSub.CONTRACTID = contract_id;
                    item.ContractSub.CONTENT = subcontentBinary.ToArray();
                    item.ContractSub.NOTE = Path.GetFileName(item.ContentSubFile.FileName);
                    item.ContractSub.USER_CREATE = CurrentUser.Instance.User.Emp_CJ;
                    item.ContractSub.PLANT = CurrentUser.Instance.User.Plant_Id;

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
            ContractViewModel.EditContractViewModel _contractviewmodel = new ContractViewModel.EditContractViewModel();
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
        public ActionResult Edit(ContractViewModel.EditContractViewModel _contractviewModel)
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
                if (contractFile != null)
                {
                    byte[] contentData = new byte[contractFile.InputStream.Length];
                    contractFile.InputStream.Read(contentData, 0, Convert.ToInt32(contractFile.InputStream.Length));
                    Binary contentBinary = new Binary(contentData);

                    //var fileName = Path.GetFileName(_contractviewModel.ContractFile.FileName);
                    //var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    //_contractviewModel.ContractFile.SaveAs(path);

                    modifyContract.CONTENT = contentBinary.ToArray();
                    modifyContract.NOTE = System.IO.Path.GetFileName(contractFile.FileName);
                }

                modifyContract.VENDOR = contract.VENDOR;
                modifyContract.PHONE = contract.PHONE;
                modifyContract.ADDRESS = contract.ADDRESS;
                modifyContract.CONTRACTNAME = contract.CONTRACTNAME;
                modifyContract.REPRESENTATION = contract.REPRESENTATION;
                modifyContract.CONTRACT_TYPE_ID = contract.CONTRACT_TYPE_ID;
                modifyContract.PERIODID = contract.PERIODID;
                modifyContract.DATE = contract.DATE;
                modifyContract.MONTHS = contract.MONTHS;

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
                    if (oldSubs.Contains(sub)) // newSub existed in database => modify
                    {
                        CONTRACT_SUB modifySub = en.CONTRACT_SUB.FirstOrDefault(c => c.ID == sub.ID);
                        if (subFile != null)
                        {
                            byte[] subcontentData = new byte[subFile.InputStream.Length];
                            subFile.InputStream.Read(subcontentData, 0, Convert.ToInt32(subFile.InputStream.Length));
                            Binary subcontentBinary = new Binary(subcontentData);

                            modifySub.CONTENT = subcontentBinary.ToArray();
                            modifySub.NOTE = Path.GetFileName(subFile.FileName);
                        }
                        modifySub.DATE = sub.DATE;
                        modifySub.SUBNAME = sub.SUBNAME;
                        modifySub.PERIODID = sub.PERIODID;
                        en.Entry(modifySub).State = EntityState.Modified;
                    }
                    else // newSub not existed in database => insert
                    {
                        if (subFile != null)
                        {
                            byte[] subcontentData = new byte[subFile.InputStream.Length];
                            subFile.InputStream.Read(subcontentData, 0, Convert.ToInt32(subFile.InputStream.Length));
                            Binary subcontentBinary = new Binary(subcontentData);

                            sub.CONTENT = subcontentBinary.ToArray();
                            sub.NOTE = Path.GetFileName(subFile.FileName);
                        }
                        else
                        {
                            ModelState.AddModelError("ContractSubViewModels[" + i + "].ContentSubFile", "File not found!");

                            _contractviewModel.Contract_Types = en.CONTRACT_TYPE;
                            _contractviewModel.Periods = en.PERIODs.ToList();

                            return View(_contractviewModel);
                        }

                        sub.ID = Guid.NewGuid();
                        sub.CONTRACTID = contract.ID;
                        sub.USER_CREATE = CurrentUser.Instance.User.Emp_CJ;
                        sub.PLANT = CurrentUser.Instance.User.Plant_Id;

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

        [HttpGet]
        public FileContentResult Get_file(Guid? con_id)
        {
            //CONTRACT con = en.CONTRACTs.Find(con_id);
            //string inc_code = con.Code;

            //declare byte array to get file content from database and string to store file name
            byte[] fileData;
            string fileName;
            //create object of LINQ to SQL class
            //using LINQ expression to get record from database for given id value
            var record = from p in en.CONTRACTs
                         where p.ID == con_id
                         select p;
            //only one record will be returned from database as expression uses condtion on primary field
            //so get first record from returned values and retrive file content (binary) and filename
            fileData = (byte[])record.First().CONTENT.ToArray();
            fileName = record.First().NOTE;
            //return file and provide byte file content and file name
            return File(fileData, "text", fileName);
        }

        [HttpGet]
        public FileContentResult Get_sub(Guid? sub_id)
        {
            //CONTRACT con = en.CONTRACTs.Find(con_id);
            //string inc_code = con.Code;

            //declare byte array to get file content from database and string to store file name
            byte[] fileData;
            string fileName;
            //create object of LINQ to SQL class
            //using LINQ expression to get record from database for given id value
            var record = from p in en.CONTRACT_SUB
                         where p.ID == sub_id
                         select p;
            //only one record will be returned from database as expression uses condtion on primary field
            //so get first record from returned values and retrive file content (binary) and filename
            fileData = (byte[])record.First().CONTENT.ToArray();
            fileName = record.First().NOTE;
            //return file and provide byte file content and file name
            return File(fileData, "text", fileName);
        }
    }
}


