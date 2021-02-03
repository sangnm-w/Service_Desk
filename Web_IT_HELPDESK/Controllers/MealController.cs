using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.Controllers
{
    public class MealController : Controller
    {
        //
        // GET: /Meal/
        ServiceDeskEntities en = new ServiceDeskEntities();

        public ActionResult Index()
        {
            var album_list = en.Albums.Where(o => o.AlbumTypeId == 2).ToList();
            return View(album_list);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string searchString)
        {
            var album_list = en.Albums.Where(i => i.Title.Contains(searchString) && i.AlbumTypeId == 2).ToList();
            return View(album_list);
        }

        [HttpPost, ActionName("Submit")]
        public ActionResult Submit(IEnumerable<int> text_answers, IEnumerable<string> text_s)
        {
            string result = string.Empty;
            int qty = 0;

            int v_album_id = 0;

            for (int text_answer = 0; text_answer < text_answers.Count(); text_answer++)
            {
                if (text_answer.ToString() != "0")
                {
                    v_album_id = Convert.ToInt32(Session[(text_answer + 1).ToString()]);

                    if (qty_num(text_answers, text_answer) != 0)
                    {
                        qty = qty_num(text_answers, text_answer);

                        int album_id = en.Albums.Where(e => e.AlbumId == v_album_id//text_answer
                                                                        ).Single().AlbumId;

                        // đưa giá trị vào card
                        //string empno = Session["employee_id"].ToString();
                        var addedAlbum = en.Albums.Single(album => album.AlbumId == album_id);

                        var cart = ShoppingCart.GetCart(this.HttpContext);

                        string text = note_num(text_s, text_answer);

                        cart.AddToCart(addedAlbum, qty, text);
                    }
                }
            }

            return RedirectToAction("Index2", "ShoppingCart");
        }

        private int qty_num(IEnumerable<int> text_answers, int num)
        {
            int qty = 0; int i = 0;
            foreach (var text_answer in text_answers)
            {
                var a = text_answer;
                if (num == i)
                    qty = Convert.ToInt32(a);
                i = i + 1;
            }
            return qty;
        }

        private string note_num(IEnumerable<string> text_s, int num)
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


        private List<Album> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            return en.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToList();
        }

        //
        // GET: /HumanceResource/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /HumanceResource/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /HumanceResource/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /HumanceResource/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /HumanceResource/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /HumanceResource/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /HumanceResource/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
