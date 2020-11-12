using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;

        [HttpPost, ActionName("Submit")]
        public ActionResult Submit(IEnumerable<string> answers, IEnumerable<string> answersnote)
        {
            //var ans = en.Questions.Find(answer_id);
            string result = string.Empty;
            int index = 0;
            foreach (var answer in answers)
            {
                //string question_id = en.QUESTIONs.Where(e => e.AnswerId == answer).Single().Question_Id;
                //string correct = en.QUESTIONs.Where(e => e.AnswerId == answer).SingleOrDefault().Correct.ToString();

                //result += string.Format("Answer for question {0} is {1} and {2} <br />", index, answer, correct);
                EMP_ANSWER emp_answer = new EMP_ANSWER();
                emp_answer.ID = Guid.NewGuid();
                emp_answer.QUESTION_ID = en.QUESTIONs.Where(o => o.NO == index.ToString()).Select(i => i.ID).SingleOrDefault();
                emp_answer.EMPLOYEEID = session_emp;
                emp_answer.DATE = DateTime.Now;
                emp_answer.ANSWERID = Convert.ToInt32(answer.ToString());

                // Get IP May Tinh
                /*IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress addr in localIPs)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        //emp_answer.NOTE = note_ans_get(answersnote, index) + addr + "/" + Environment.; //Environment.MachineName : Ten may tinh
                        try
                        {
                            emp_answer.NOTE = note_ans_get(answersnote, index) + addr + "/" + Environment.MachineName + System.Net.Dns.GetHostEntry(Request.UserHostAddress).HostName + Request.UserHostAddress;
                        }
                        catch { emp_answer.NOTE = note_ans_get(answersnote, index); }
                    }
                }
                */
                try
                {
                    en.EMP_ANSWER.Add(emp_answer);

                    en.SaveChanges();
                    //ViewBag.Message = "CAC BAN DA HOAN THANH DANH GIA, XIN CHAN THANH CAM ON DANH GIA CUA BAN GIUP CHUNG TOI HOAN THIEN HON ./."; 
                    return View("~/Views/Shared/Messenger");
                }
                catch { }
                index++;
            }
            return View("Index");
        }

        [HttpPost, ActionName("Submit_")] // khảo sát canteen
        public ActionResult Submit_(IEnumerable<string> answertexts, IEnumerable<string> answerRadios, IEnumerable<Guid> questions)
        {
            bool result = true;
            int questionNo = 0;
            List<EMP_ANSWER> emp_answerlist = new List<EMP_ANSWER>();
            foreach (var answer in answertexts)
            {
                EMP_ANSWER emp_answer = new EMP_ANSWER();

                emp_answer.ID = Guid.NewGuid();
                emp_answer.QUESTION_ID = en.QUESTIONs.FirstOrDefault(q => q.NO == questionNo.ToString()).ID;
                emp_answer.EMPLOYEEID = session_emp;
                emp_answer.DATE = DateTime.Now;
                emp_answer.NOTE = answer.ToString();

                #region hide
                // Get IP May Tinh
                /*IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress addr in localIPs)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        //emp_answer.NOTE = note_ans_get(answersnote, index) + addr + "/" + Environment.; //Environment.MachineName : Ten may tinh
                        emp_answer.NOTE = note_ans_get(answersnote, index) + addr + "/" + Environment.MachineName + System.Net.Dns.GetHostEntry(Request.UserHostAddress).HostName + Request.UserHostAddress;
                    }
                }
                */
                #endregion

                emp_answerlist.Add(emp_answer);
                questionNo++;
            }

            foreach (var answer in answerRadios)
            {
                EMP_ANSWER emp_answer = new EMP_ANSWER();

                emp_answer.ID = Guid.NewGuid();
                emp_answer.QUESTION_ID = en.QUESTIONs.FirstOrDefault(q => q.NO == questionNo.ToString()).ID;
                emp_answer.EMPLOYEEID = session_emp;
                emp_answer.DATE = DateTime.Now;
                if (questionNo != 9)
                    emp_answer.NOTE = answer.ToString();

                emp_answerlist.Add(emp_answer);
                questionNo++;
            }

            try
            {
                en.EMP_ANSWER.AddRange(emp_answerlist);

                en.SaveChanges();
                //ViewBag.Message = "CAC BAN DA HOAN THANH DANH GIA, XIN CHAN THANH CAM ON DANH GIA CUA BAN GIUP CHUNG TOI HOAN THIEN HON ./."; 
            }
            catch (Exception ex)
            {
                result = false;
            }

            if (result)
            {
                return View("~/Views/Shared/Messenger.cshtml");
            }
            return View("Index");
        }

        private string GetDept_id()
        {
            string dept_id = en.Employees.Where(f => (f.Emp_CJ == session_emp)).Select(f => f.Department_Id).SingleOrDefault();
            return dept_id;
        }

        private string GetPlant_id()
        {
            string plant_id = en.Employees.Where(f => (f.Emp_CJ == session_emp)).Select(f => f.Plant_Id).SingleOrDefault();
            return plant_id;
        }

        private string note_ans_get(IEnumerable<string> text_s, int num)
        {
            string text_note = ""; int i = 0;
            foreach (var text_ in text_s)
            {
                var a = text_;
                if (num == i)
                    text_note = a.ToString();
                i = i + 1;
            }
            return text_note;
        }

        private string question_id_get(IEnumerable<Guid> questions, int num)
        {
            string text_note = ""; int i = 0;
            foreach (var text_ in questions)
            {
                var a = text_;
                if (num == i)
                    text_note = a.ToString();
                i = i + 1;
            }
            return text_note;
        }
    }
}
