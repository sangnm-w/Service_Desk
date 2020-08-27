using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Web_IT_HELPDESK.Controllers
{
    public class EventController : ApiController
    {
        //
        // GET: /Event/
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        private Web_IT_HELPDESKEntities db = new Web_IT_HELPDESKEntities();

        // GET api/Event
        public IQueryable GetEvents()
        {
            return db.Events;
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
