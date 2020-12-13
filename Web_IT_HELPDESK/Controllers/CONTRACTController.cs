﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.Linq;
using System.IO;
using System.Linq;
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
        // GET: ContractIndex
        [Authorize]
        public ActionResult ContractIndex()//int? page)
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

            string curr_PlantId = CurrentUser.Instance.User.Plant_Id;
            string curr_DeptId = CurrentUser.Instance.User.Department_Id;
            bool currUserIsManager = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_PlantId && d.Department_Id == curr_DeptId && d.Manager_Id == CurrentUser.Instance.User.Emp_CJ) != null ? true : false;
            ViewBag.currUserIsManager = currUserIsManager;

            var depts = en.Departments.Where(d => d.Plant_Id == curr_PlantId && d.Deactive == false).Select(d => new DepartmentViewModel
            {
                Department_Id = d.Department_Id,
                Department_Name = d.Department_Name
            }).OrderByDescending(d => d.Department_Name).ToList();

            ViewBag.Departments = depts;

            var contract_types = en.CONTRACT_TYPE;
            ViewBag.Contract_Types = contract_types;

            var contracts = en.CONTRACTs.Where(c => c.DEL != true && c.PLANT == curr_PlantId && c.DEPARTMENTID == curr_DeptId);

            if (currUserIsManager == false)
            {
                contracts = contracts.Where(c => c.USER_CREATE == CurrentUser.Instance.User.Emp_CJ);
            }

            return View(contracts);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ContractIndex(string search_, DateTime? date_, ICollection<string> v_depts, string v_contract_type, int daynum_)//, DateTime fromdate_, DateTime todate_)
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

        //public ActionResult ContractCreate()
        //{
        //    return View();
        //}

        public PartialViewResult ContractCreate()
        {
            CONTRACT _contract = new CONTRACT();
            ViewBag.User_create = System.Web.HttpContext.Current.User.Identity.Name;
            ViewBag.PERIOD_ID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", en.PERIODs.First().PERIOD_ID);
            ViewBag.CONTRACT_TYPE_ID = new SelectList(en.CONTRACT_TYPE, "CONTRACT_TYPE_ID", "CONTRACT_TYPE_NAME", en.CONTRACT_TYPE.First().CONTRACT_TYPE_ID);
            var dept = from i in en.Employees where i.Emp_CJ == System.Web.HttpContext.Current.User.Identity.Name select i.Department_Id;
            ViewBag.DEPT_ID = dept.ToString();
            return PartialView("partial_create_new_asset", _contract);
        }

        [HttpPost]
        public ActionResult ContractCreate(CONTRACT _contract, HttpPostedFileBase image)
        {
            ViewBag.PERIODID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", _contract.PERIODID);
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                }
                try
                {
                    en.CONTRACTs.Add(_contract);
                    en.SaveChanges();
                }
                catch { }
            }
            var view = en.CONTRACTs.Where(i => i.DEL != true);
            return View("Asset_Import_List", view);
        }

        private string GetDept_id(string v_emp)
        {
            string dept_id = en.Employees.Where(f => (f.Emp_CJ == v_emp)).Select(f => f.Department_Id).SingleOrDefault();
            return dept_id;
        }

        // GET: CONTRACT/Create
        public ActionResult CreateCONTRACTViewModel2()
        {
            ContractViewModel _contractviewModel = new ContractViewModel();
            _contractviewModel.Contract_Types = en.CONTRACT_TYPE;
            _contractviewModel.Periods = en.PERIODs.ToList();
            List<ContractSubViewModel> cslist = new List<ContractSubViewModel>();
            for (int i = 0; i < 10; i++)
            {
                cslist.Add(new ContractSubViewModel());
            }
            _contractviewModel.ContractSubViewModels = cslist;
            return View("CreateCONTRACTViewModel2", _contractviewModel);
        }

        // POST: CONTRACT/Create
        [HttpPost]
        public ActionResult CreateCONTRACTViewModel2(ContractViewModel _contractviewModel)
        {
            List<string> modelStateKeys = ModelState.Select(m => m.Key).ToList();
            int countContractSub = _contractviewModel.countContractSubModel;
            if (countContractSub != 10)
            {
                for (int i = 9; i >= countContractSub; i--)
                {
                    List<string> keynames = modelStateKeys.Where(m => m.Contains(i.ToString())).ToList();
                    foreach (string key in keynames)
                    {
                        ModelState[key].Errors.Clear();
                    }
                }
            }

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
                for (int i = 0; i < countContractSub; i++)
                {
                    ContractSubViewModel sub = _contractviewModel.ContractSubViewModels[i];
                    byte[] subcontentData = new byte[sub.ContentSubFile.InputStream.Length];
                    sub.ContentSubFile.InputStream.Read(subcontentData, 0, Convert.ToInt32(sub.ContentSubFile.InputStream.Length));
                    Binary subcontentBinary = new Binary(subcontentData);

                    sub.ContractSub.ID = Guid.NewGuid();
                    sub.ContractSub.CONTRACTID = contract_id;
                    sub.ContractSub.CONTENT = subcontentBinary.ToArray();
                    sub.ContractSub.NOTE = Path.GetFileName(sub.ContentSubFile.FileName);
                    sub.ContractSub.USER_CREATE = CurrentUser.Instance.User.Emp_CJ;
                    sub.ContractSub.PLANT = CurrentUser.Instance.User.Plant_Id;

                    en.CONTRACT_SUB.Add(sub.ContractSub);
                }

                try
                {
                    en.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return RedirectToAction("ContractIndex");
            }

            _contractviewModel.Contract_Types = en.CONTRACT_TYPE;
            _contractviewModel.Periods = en.PERIODs;
            return View("CreateCONTRACTViewModel2", _contractviewModel);
        }

        // GET: CONTRACT/Edit
        public ActionResult EditCONTRACTViewModel(Guid id)
        {
            ContractViewModel _contractviewmodel = new ContractViewModel();
            _contractviewmodel.Contract_Types = en.CONTRACT_TYPE;
            _contractviewmodel.Periods = en.PERIODs.ToList();

            var c = en.CONTRACTs.FirstOrDefault(i => i.DEL != true && i.ID == id);
            _contractviewmodel.CONTRACT = c;

            var ct = en.CONTRACT_SUB.Where(o => o.CONTRACTID == id).ToList();
            _contractviewmodel.countContractSubModel = ct.Count;

            for (int i = 0; i < ct.Count; i++)
            {
                _contractviewmodel.ContractSubViewModels[i].ContractSub = ct[i];
            }
            return View("EditCONTRACTViewModel", _contractviewmodel);
        }

        // POST: CONTRACT/Edit
        [HttpPost]
        public ActionResult EditCONTRACTViewModel(ContractViewModel _contractviewModel)
        {
            //Clear error of Contract File
            ModelState["ContractFile"].Errors.Clear();

            //Clear error of contractSubs
            List<string> modelStateKeys = ModelState.Select(m => m.Key).ToList();
            int countContractSub = _contractviewModel.countContractSubModel;
            //Clear error of contractSubs file
            for (int i = 0; i < countContractSub; i++)
            {
                string subFileKey = "ContractSubViewModel[" + i + "].ContentSubFile";
                ModelState[subFileKey].Errors.Clear();
            }
            if (countContractSub != 10)
            {
                for (int i = 9; i >= countContractSub; i--)
                {
                    List<string> keynames = modelStateKeys.Where(m => m.Contains(i.ToString())).ToList();
                    foreach (string key in keynames)
                    {
                        ModelState[key].Errors.Clear();
                    }
                }
            }

            if (ModelState.IsValid)
            {
                CONTRACT contract = _contractviewModel.CONTRACT;
                HttpPostedFileBase contractFile = _contractviewModel.ContractFile;

                if (contractFile.ContentLength > 0)
                {
                    byte[] contentData = new byte[contractFile.InputStream.Length];
                    contractFile.InputStream.Read(contentData, 0, Convert.ToInt32(contractFile.InputStream.Length));
                    Binary contentBinary = new Binary(contentData);

                    //var fileName = Path.GetFileName(_contractviewModel.ContractFile.FileName);
                    //var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    //_contractviewModel.ContractFile.SaveAs(path);

                    contract.CONTENT = contentBinary.ToArray();
                }

                contract.ID = Guid.NewGuid();
                contract.NOTE = System.IO.Path.GetFileName(contractFile.FileName);
                contract.DEPARTMENTID = CurrentUser.Instance.User.Department_Id;
                contract.USER_CREATE = CurrentUser.Instance.User.Emp_CJ;
                contract.PLANT = CurrentUser.Instance.User.Plant_Id;
                contract.DATE_CREATE = DateTime.Now;

                en.Entry(contract).State = EntityState.Modified;

                Guid contract_id = contract.ID;
                List<CONTRACT_SUB> subOfContract = en.CONTRACT_SUB.Where(s => s.CONTRACTID == contract_id).ToList();
                for (int i = 0; i < countContractSub; i++)
                {
                    ContractSubViewModel sub = _contractviewModel.ContractSubViewModels[i];

                    if (subOfContract.Contains(sub.ContractSub))
                    {
                        if (sub.ContentSubFile.ContentLength > 0)
                        {
                            byte[] subcontentData = new byte[sub.ContentSubFile.InputStream.Length];
                            sub.ContentSubFile.InputStream.Read(subcontentData, 0, Convert.ToInt32(sub.ContentSubFile.InputStream.Length));
                            Binary subcontentBinary = new Binary(subcontentData);
                            sub.ContractSub.CONTENT = subcontentBinary.ToArray();
                        }

                        sub.ContractSub.ID = Guid.NewGuid();
                        sub.ContractSub.CONTRACTID = contract_id;
                        sub.ContractSub.NOTE = Path.GetFileName(sub.ContentSubFile.FileName);
                        sub.ContractSub.USER_CREATE = CurrentUser.Instance.User.Emp_CJ;
                        sub.ContractSub.PLANT = CurrentUser.Instance.User.Plant_Id;

                        en.Entry(sub.ContractSub).State = EntityState.Modified;
                    }
                    else
                    {

                    }
                }

                try
                {
                    en.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return RedirectToAction("ContractIndex");
            }

            //ContractViewModel _contractviewmodel = new ContractViewModel();
            //_contractviewmodel.Contract_Types = en.CONTRACT_TYPE;
            //_contractviewmodel.Periods = en.PERIODs.ToList();

            //var c = en.CONTRACTs.FirstOrDefault(i => i.DEL != true && i.ID == id);
            //_contractviewmodel.CONTRACT = c;

            //var ct = en.CONTRACT_SUB.Where(o => o.CONTRACTID == id).ToList();
            //_contractviewmodel.countContractSubModel = ct.Count;

            //for (int i = 0; i < ct.Count; i++)
            //{
            //    _contractviewmodel.ContractSubViewModels[i].ContractSub = ct[i];
            //}
            return View("EditCONTRACTViewModel", _contractviewModel);
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

        public ActionResult Delete_contractSUB(Guid v_id, Guid v_contract_id)
        {
            //Guid id = new Guid(v_id);
            CONTRACT_SUB _contract_sub_del = en.CONTRACT_SUB.Where(i => i.ID == v_id && i.CONTRACTID == v_contract_id).FirstOrDefault();
            if (_contract_sub_del == null)
            {
                return HttpNotFound();
            }
            else
            {
                en.CONTRACT_SUB.Remove(_contract_sub_del);
                en.SaveChanges();
            }
            // call edit view again 
            ContractViewModel _contractviewmodel = new ContractViewModel();
            var c = en.CONTRACTs.Where(i => i.DEL != true && i.ID == v_contract_id).FirstOrDefault();
            {
                _contractviewmodel.CONTRACT = c;
            }
            var ct = en.CONTRACT_SUB.Where(o => o.CONTRACTID == c.ID).ToList();

            foreach (var i in ct)
            {
                //_contractviewmodel.ContractSubViewModel.ContractSub = i;
            }
            ViewBag.User_create = c.USER_CREATE;
            ViewBag.DEPT_ID = GetDept_id(c.USER_CREATE);
            ViewBag.PERIODID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", c.PERIODID);
            ViewBag.CONTRACT_TYPE_ID = new SelectList(en.CONTRACT_TYPE, "CONTRACT_TYPE_ID", "CONTRACT_TYPE_NAME", c.CONTRACT_TYPE_ID);
            return PartialView("EditCONTRACTViewModel", _contractviewmodel);
        }

        public ActionResult Delete_contract(Guid v_id)
        {
            //Guid id = new Guid(v_id);
            //CONTRACT _contract_del = en.CONTRACTs.Where(i => i.ID == v_id).FirstOrDefault();
            CONTRACT _contract_del = en.CONTRACTs.Find(v_id);
            if (_contract_del == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    //en.CONTRACT_SUB.Remove(_contract_sub_del);
                    _contract_del.DEL = true;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                    throw new DbEntityValidationException(errorMessages);
                }
            }


            DateTime today = DateTime.Today;

            int v_year = today.Year;
            int v_month = today.Month;
            string dept = GetDept_id(System.Web.HttpContext.Current.User.Identity.Name);
            var contract_list = en.CONTRACTs.Where(o => o.DEL != true && today <= DbFunctions.AddDays(o.DATE, 45) && o.DEPARTMENTID == dept).OrderBy(i => i.DATE);
            return View("ContractIndex", contract_list);
        }


    }
}


