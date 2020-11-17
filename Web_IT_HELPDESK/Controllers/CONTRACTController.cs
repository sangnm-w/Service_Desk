using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class CONTRACTController : Controller
    {
        //
        // GET: /CONTRACT/
        ServiceDeskEntities en = new ServiceDeskEntities();
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

            string curr_plantId = CurrentUser.Instance.User.Plant_Id;

            var depts = en.Departments.Where(d => d.Plant_Id == curr_plantId && d.Deactive == false).Select(d => new DepartmentViewModel
            {
                Department_Id = d.Department_Id,
                Department_Name = d.Department_Name
            }).OrderByDescending(d => d.Department_Name).ToList();
            ViewBag.Departments = depts;

            var contracts = en.CONTRACTs.Where(c => c.DEL == false);
            return View(contracts);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ContractIndex(string search_, DateTime? date_, ICollection<string> v_depts, string v_contract_type, int daynum_)//, DateTime fromdate_, DateTime todate_)
        {
            string v_dept_string = "";
            string curr_plantId = CurrentUser.Instance.User.Plant_Id;
            IEnumerable<CONTRACT> contract_list;

            if (v_depts != null)
            {
                if ((search_ == "" || search_ == "search") && v_depts.Count == 12 && v_contract_type == "ALL")
                {
                    contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                              && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                              && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
                }
                else if ((search_ == "" || search_ == "search") && v_depts.Count == 12 && v_contract_type != "ALL")
                {
                    contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                             && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
                                                             && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                             && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
                }
                else if (search_ == "" || search_ == "search" && v_contract_type == "ALL")
                {
                    foreach (var v_dept in v_depts)
                    {
                        v_dept_string += v_dept.ToString() + " ";
                    }

                    contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                                && v_dept_string.Trim().Contains(o.DEPARTMENTID)
                                                                && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                                && v_dept_string.Trim().Contains(o.DEPARTMENTID)
                                                                && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
                }
                else if (search_ == "" || search_ == "search" && v_contract_type != "ALL")
                {
                    foreach (var v_dept in v_depts)
                    {
                        v_dept_string += v_dept.ToString() + " ";
                    }

                    contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                             && v_dept_string.Trim().Contains(o.DEPARTMENTID)
                                                             && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
                                                             && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                             && o.PLANT == curr_plantId
                                                             && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
                }
                else if (v_contract_type == "ALL" && (search_ != "" || search_ != "search"))
                {
                    foreach (var v_dept in v_depts)
                    {
                        v_dept_string += v_dept.ToString() + " ";
                    }

                    contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                             && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
                                                             && o.CONTRACTNAME.Contains(search_) == true && v_dept_string.Trim().Contains(o.DEPARTMENTID)
                                                             && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                             && o.PLANT == curr_plantId
                                                             && o.CONTRACTNAME.Contains(search_) == true && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
                }
                else if (v_contract_type == "ALL")
                {
                    foreach (var v_dept in v_depts)
                    {
                        v_dept_string += v_dept.ToString() + " ";
                    }

                    contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                             && o.CONTRACT_TYPE.CONTRACT_TYPE_NAME == v_contract_type
                                                             && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                             && o.PLANT == curr_plantId
                                                             && o.CONTRACTNAME.Contains(search_) == true && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
                }
                else
                {
                    foreach (var v_dept in v_depts)
                    {
                        v_dept_string += v_dept.ToString() + " ";
                    }

                    contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                             && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                             && o.CONTRACTNAME.Contains(search_) == true
                                                             && o.PLANT == curr_plantId
                                                             && v_dept_string.Trim().Contains(o.DEPARTMENTID)).OrderBy(i => i.DATE);
                }
            }
            else
            {
                contract_list = en.CONTRACTs.Where(o => o.DEL != true
                                                              && date_ <= DbFunctions.AddDays(o.DATE, daynum_)
                                                              && o.PLANT == curr_plantId).OrderBy(i => i.DATE);
            }

            var depts = en.Departments.Where(d => d.Plant_Id == curr_plantId && d.Deactive == false).Select(d => new DepartmentViewModel
            {
                Department_Id = d.Department_Id,
                Department_Name = d.Department_Name
            }).OrderByDescending(d => d.Department_Name).ToList();
            ViewBag.Departments = depts;

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

        //GetPlant_id
        private string GetPlant_id(string v_emp)
        {
            string plant_id = en.Employees.Where(f => (f.Emp_CJ == v_emp)).Select(f => f.Plant_Id).SingleOrDefault();
            return plant_id;
        }


        public Guid contract_id = Guid.NewGuid();
        //Create Import Model
        public ActionResult CreateCONTRACTViewModel2()
        {
            ViewBag.User_create = CurrentUser.Instance.User.Emp_CJ;
            ViewBag.curr_deptId = CurrentUser.Instance.User.Department_Id;

            ContractViewModel _contractviewModel = new ContractViewModel();
            var c = new CONTRACT();
            c.ID = contract_id;
            {
                _contractviewModel.CONTRACT = c;
            }
            var ct = new CONTRACT_SUB();
            ct.CONTRACTID = contract_id;
            ct.ID = Guid.NewGuid();
            {
                _contractviewModel.CONTRACT_SUB = ct;
            }
            ViewBag.PERIOD_ID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", en.PERIODs.First().PERIOD_ID);
            ViewBag.PERIODID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", en.PERIODs.First().PERIOD_ID);
            ViewBag.CONTRACT_TYPE_ID = new SelectList(en.CONTRACT_TYPE, "CONTRACT_TYPE_ID", "CONTRACT_TYPE_NAME", en.CONTRACT_TYPE.First().CONTRACT_TYPE_ID);
            return View("CreateCONTRACTViewModel2", _contractviewModel);
        }

        [HttpPost]
        public ActionResult CreateCONTRACTViewModel2(List<CONTRACT_SUB> CONTRACT_SUB, string[] PERIOD_ID,
                                                        string PERIODID, string USER_CREATE, string DEPARTMENTID, string CONTRACT_TYPE_ID,
                                                        CONTRACT contract, HttpPostedFileBase image, List<HttpPostedFileBase> subcontent)
        {
            image = image ?? Request.Files["image"];
            if (image != null && image.ContentLength > 0)
            {
                try
                {
                    //foreach (string upload in Request.Files)
                    //{
                    //create byte array of size equal to file input stream
                    byte[] fileData = new byte[Request.Files["image"].InputStream.Length];
                    //add file input stream into byte array
                    Request.Files["image"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["image"].InputStream.Length));
                    //create system.data.linq object using byte array
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                    //initialise object of FileDump LINQ to sql class passing values to be inserted

                    contract.CONTENT = binaryFile.ToArray();
                    contract.NOTE = System.IO.Path.GetFileName(Request.Files["image"].FileName);
                    contract.ID = contract_id;
                    contract.CONTRACT_TYPE_ID = CONTRACT_TYPE_ID;
                    contract.PERIODID = PERIODID;
                    contract.USER_CREATE = USER_CREATE;
                    contract.DEPARTMENTID = DEPARTMENTID;
                    contract.USER_CREATE = System.Web.HttpContext.Current.User.Identity.Name;
                    contract.PLANT = GetPlant_id(System.Web.HttpContext.Current.User.Identity.Name);
                    en.CONTRACTs.Add(contract);
                    en.SaveChanges();
                    //}
                }
                catch { }
            }
            else return new JavaScriptResult { Script = "alert('Not allow null attached file');" };

            //if (ModelState.IsValid)
            //{
            using (ServiceDeskEntities dc = new ServiceDeskEntities())
            {
                try
                {
                    int j = 0;

                    foreach (var i in CONTRACT_SUB)
                    {
                        if (subcontent != null)
                        {
                            int a = 0;
                            if (a == subcontent[j].InputStream.Length)// check lengh
                            {
                                try
                                {
                                    //foreach (string upload in Request.Files)
                                    //{
                                    //create byte array of size equal to file input stream
                                    byte[] fileData = new byte[subcontent[j].InputStream.Length];
                                    //add file input stream into byte array
                                    subcontent[j].InputStream.Read(fileData, 0, Convert.ToInt32(subcontent[j].InputStream.Length));
                                    //create system.data.linq object using byte array
                                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                                    //initialise object of FileDump LINQ to sql class passing values to be inserted

                                    i.CONTENT = binaryFile.ToArray();
                                    i.NOTE = System.IO.Path.GetFileName(subcontent[j].FileName);
                                    i.CONTRACTID = contract_id;
                                    i.USER_CREATE = System.Web.HttpContext.Current.User.Identity.Name;
                                    i.PLANT = GetPlant_id(System.Web.HttpContext.Current.User.Identity.Name);
                                    i.ID = Guid.NewGuid();
                                    i.PERIODID = PERIOD_ID[j].ToString();
                                    dc.CONTRACT_SUB.Add(i);
                                    if (i.DATE != null)
                                    {
                                        dc.SaveChanges();
                                    }
                                    j += 1;
                                    //}
                                }
                                catch { }
                                ViewBag.Message = "Data successfully saved!";
                            }
                        }
                    }
                }
                catch { }
            }
            // }
            return View("ContractIndex", en.CONTRACTs.Where(o => o.DEL != true));
        }

        //Edit IMPORT MODEL 
        public PartialViewResult EditCONTRACTViewModel(Guid id)
        {
            ContractViewModel _contractviewmodel = new ContractViewModel();
            var c = en.CONTRACTs.Where(i => i.DEL != true && i.ID == id).FirstOrDefault();
            {
                _contractviewmodel.CONTRACT = c;
                ViewBag.User_create = c.USER_CREATE;
                ViewBag.DEPT_ID = GetDept_id(c.USER_CREATE);
                ViewBag.PERIODID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", c.PERIODID);
                ViewBag.CONTRACT_TYPE_ID = new SelectList(en.CONTRACT_TYPE, "CONTRACT_TYPE_ID", "CONTRACT_TYPE_NAME", c.CONTRACT_TYPE_ID);
            }
            var ct = en.CONTRACT_SUB.Where(o => o.CONTRACTID == id).ToList();

            if (ct.Count > 0)
            {
                foreach (var i in ct)
                {
                    _contractviewmodel.CONTRACT_SUB = i;
                    ViewBag.PERIOD_ID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", i.PERIODID);
                }
            }
            return PartialView("EditCONTRACTViewModel", _contractviewmodel);
        }

        [HttpPost]
        public ActionResult EditCONTRACTViewModel(List<CONTRACT_SUB> CONTRACT_SUB, string v_asset_id, string[] PERIOD_ID,
                                            string PERIODID, string USER_CREATE, string DEPARTMENTID, string CONTRACT_TYPE_ID,
                                            CONTRACT contract, HttpPostedFileBase image, List<HttpPostedFileBase> subcontent)
        {
            image = image ?? Request.Files["image"];
            if (image != null && image.ContentLength > 0)
            {
                try
                {
                    //-----------------------------------------------------------------------
                    byte[] fileData = new byte[Request.Files["image"].InputStream.Length];
                    Request.Files["image"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["image"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                    contract.CONTENT = binaryFile.ToArray();
                    contract.NOTE = System.IO.Path.GetFileName(Request.Files["image"].FileName);
                    //-----------------------------------------------------------------------
                    contract.CONTRACT_TYPE_ID = CONTRACT_TYPE_ID;
                    contract.PERIODID = PERIODID;
                    contract.USER_CREATE = USER_CREATE;
                    contract.DEPARTMENTID = DEPARTMENTID;
                    contract.PLANT = GetPlant_id(System.Web.HttpContext.Current.User.Identity.Name);
                    var existingCart = en.CONTRACTs.Find(contract.ID);
                    if (existingCart != null)
                    {
                        //entity is already in the context
                        var attachedEntry = en.Entry(existingCart);
                        attachedEntry.CurrentValues.SetValues(contract);
                    }
                    else
                    {
                        //Since we don't have it in db, this is a simple add.
                        en.CONTRACTs.Add(contract);
                    }
                    en.SaveChanges();
                }
                catch { }
            }
            else
            {
                var existingCart = en.CONTRACTs.Find(contract.ID);
                if (existingCart != null)
                {
                    var attachedEntry = en.Entry(existingCart);
                    contract.CONTENT = existingCart.CONTENT;
                    if (contract.PERIODID == "")
                        contract.PERIODID = existingCart.PERIODID;
                    contract.CONTRACT_TYPE_ID = CONTRACT_TYPE_ID;
                    contract.PERIODID = PERIODID;
                    contract.USER_CREATE = USER_CREATE;
                    contract.DEPARTMENTID = DEPARTMENTID;
                    attachedEntry.CurrentValues.SetValues(contract);
                    en.SaveChanges();
                }
            }
            ViewBag.PERIODID = new SelectList(en.PERIODs, "PERIOD_ID", "PERIOD_NAME", contract.PERIODID);
            ViewBag.CONTRACT_TYPE_ID = new SelectList(en.CONTRACT_TYPE, "CONTRACT_TYPE_ID", "CONTRACT_TYPE_NAME", contract.CONTRACT_TYPE_ID);

            //if (ModelState.IsValid)
            //{
            //kiểm tra lỗi ModelState.IsValid
            var errors = ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .Select(x => new { x.Key, x.Value.Errors })
                                .ToArray();
            using (ServiceDeskEntities dc = new ServiceDeskEntities())
            {
                try
                {
                    int j = 0;
                    foreach (var i in CONTRACT_SUB)
                    {
                        if (subcontent != null && subcontent[j].ContentLength > 0)
                        {
                            try
                            {
                                //create byte array of size equal to file input stream
                                byte[] fileData = new byte[subcontent[j].InputStream.Length];
                                //add file input stream into byte array
                                subcontent[j].InputStream.Read(fileData, 0, Convert.ToInt32(subcontent[j].InputStream.Length));
                                //create system.data.linq object using byte array
                                System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                                //initialise object of FileDump LINQ to sql class passing values to be inserted

                                var existing_importdetail = en.CONTRACT_SUB.Find(i.ID);
                                if (existing_importdetail != null)
                                {
                                    i.CONTENT = binaryFile.ToArray();
                                    i.NOTE = System.IO.Path.GetFileName(subcontent[j].FileName);
                                    //entity is already in the context
                                    var attachedEntry = en.Entry(existing_importdetail);
                                    i.PERIODID = PERIOD_ID[j].ToString();
                                    i.CONTRACTID = existing_importdetail.CONTRACTID;
                                    i.PLANT = GetPlant_id(System.Web.HttpContext.Current.User.Identity.Name);
                                    attachedEntry.CurrentValues.SetValues(i);
                                }
                                else
                                {
                                    i.CONTENT = binaryFile.ToArray();
                                    i.NOTE = System.IO.Path.GetFileName(subcontent[j].FileName);

                                    //Since we don't have it in db, this is a simple add.
                                    i.CONTRACTID = contract.ID;
                                    i.PERIODID = PERIOD_ID[j].ToString();
                                    i.ID = Guid.NewGuid();
                                    i.PLANT = GetPlant_id(System.Web.HttpContext.Current.User.Identity.Name);
                                    en.CONTRACT_SUB.Add(i);
                                }

                            }
                            catch { }
                            en.SaveChanges();
                            j += 1;
                        }
                    }
                    ViewBag.Message = "Data successfully saved!";
                }
                catch { }
            }
            //}
            return View("ContractIndex", en.CONTRACTs.Where(o => o.DEL != true));
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
                _contractviewmodel.CONTRACT_SUB = i;
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


