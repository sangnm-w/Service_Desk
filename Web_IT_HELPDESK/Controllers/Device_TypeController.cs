using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK;

namespace Web_IT_HELPDESK.Controllers
{
    public class Device_TypeController : Controller
    {
        private ServiceDeskEntities db = new ServiceDeskEntities();

        // GET: Device_Type
        public ActionResult Index()
        {
            return View(db.Device_Type.ToList());
        }

        // GET: Device_Type/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device_Type device_Type = db.Device_Type.Find(id);
            if (device_Type == null)
            {
                return HttpNotFound();
            }
            return View(device_Type);
        }

        // GET: Device_Type/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Device_Type/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Device_Type_Id,Device_Type_Name")] Device_Type device_Type)
        {
            if (ModelState.IsValid)
            {
                db.Device_Type.Add(device_Type);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(device_Type);
        }

        // GET: Device_Type/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device_Type device_Type = db.Device_Type.Find(id);
            if (device_Type == null)
            {
                return HttpNotFound();
            }
            return View(device_Type);
        }

        // POST: Device_Type/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Device_Type_Id,Device_Type_Name")] Device_Type device_Type)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(device_Type).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(device_Type);
        }

        // GET: Device_Type/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device_Type device_Type = db.Device_Type.Find(id);
            if (device_Type == null)
            {
                return HttpNotFound();
            }
            return View(device_Type);
        }

        // POST: Device_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Device_Type device_Type = db.Device_Type.Find(id);
            db.Device_Type.Remove(device_Type);
            db.SaveChanges();
            return RedirectToAction("Index");
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
