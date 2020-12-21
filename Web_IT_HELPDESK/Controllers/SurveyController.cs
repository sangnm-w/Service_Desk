using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.Controllers
{
    public class SurveyController : Controller
    {
        private ServiceDeskEntities db = new ServiceDeskEntities();

        // GET: Survey/SecuritySurvey
        public ActionResult SecuritySurvey()
        {
            return View();
        }

        // POST: Survey/SecuritySurvey
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecuritySurvey(IEnumerable<string> answertexts, IEnumerable<string> answerRadios)
        {
            bool result = true;
            int maxQuestionNo = db.QUESTIONs.Where(q => q.NOTE == "SecuritySurvey").Count();
            int radioNo = 0;
            int textNo = 0;

            List<EMP_ANSWER> emp_answerlist = new List<EMP_ANSWER>();

            for (int questionNo = 0; questionNo < maxQuestionNo; questionNo++)
            {
                EMP_ANSWER emp_answer = new EMP_ANSWER();

                emp_answer.ID = Guid.NewGuid();
                emp_answer.QUESTION_ID = db.QUESTIONs.FirstOrDefault(q => q.No == questionNo && q.NOTE == "SecuritySurvey").ID;
                emp_answer.EMPLOYEEID = CurrentUser.Instance.User.Emp_CJ;
                emp_answer.DATE = DateTime.Now;

                if (questionNo == 0)
                    emp_answer.NOTE = null;
                else if (questionNo <= 16)
                {
                    emp_answer.NOTE = answerRadios.ElementAt(radioNo).ToString();
                    radioNo++;
                }
                else
                {
                    emp_answer.NOTE = answertexts.ElementAt(textNo).ToString();
                    textNo++;
                }

                emp_answerlist.Add(emp_answer);
            }

            try
            {
                db.EMP_ANSWER.AddRange(emp_answerlist);

                db.SaveChanges();
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

        // GET: Survey/CanteenSurvey
        [Authorize]
        public ActionResult CanteenSurvey()
        {
            var questions = db.QUESTIONs.Where(i => i.DEL != true && i.NOTE == "CanteenSurvey");
            ViewBag.Switch = 7;
            return View(questions);
        }

        // POST: Survey/CanteenSurvey
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CanteenSurvey(IEnumerable<string> answertexts, IEnumerable<string> answerRadios)
        {
            bool result = true;
            int maxQuestionNo = db.QUESTIONs.Where(q => q.NOTE == "CanteenSurvey").Count();
            int radioNo = 0;
            int textNo = 0;

            List<EMP_ANSWER> emp_answerlist = new List<EMP_ANSWER>();

            for (int questionNo = 0; questionNo < maxQuestionNo; questionNo++)
            {
                EMP_ANSWER emp_answer = new EMP_ANSWER();

                emp_answer.ID = Guid.NewGuid();
                emp_answer.QUESTION_ID = db.QUESTIONs.FirstOrDefault(q => q.No == questionNo && q.NOTE == "CanteenSurvey").ID;
                emp_answer.EMPLOYEEID = CurrentUser.Instance.User.Emp_CJ;
                emp_answer.DATE = DateTime.Now;

                if (questionNo < 0) //exclude (question no need answer) 1/3/4/8
                    emp_answer.NOTE = null;
                else if (questionNo < 7)
                {
                    emp_answer.NOTE = answerRadios.ElementAt(radioNo).ToString();
                    radioNo++;
                }
                else
                {
                    emp_answer.NOTE = answertexts.ElementAt(textNo).ToString();
                    textNo++;
                }

                emp_answerlist.Add(emp_answer);
            }

            try
            {
                db.EMP_ANSWER.AddRange(emp_answerlist);

                db.SaveChanges();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
