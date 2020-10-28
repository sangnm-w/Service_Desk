using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class ShoppingCartController : Controller
    {
        //
        // GET: /ShoppingCart/
        ServiceDeskEntities en = new ServiceDeskEntities();
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            //string empno = Session["employee_id"].ToString();
            //var cart = en.Carts.Where(i => i.CartId == empno);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            // Return the view
            return View(viewModel);
        }

        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;
        public ActionResult Index2()
        {
            var cart = en.Carts.Where(i => i.CartId == session_emp).ToList();
            return View(cart);
        }

        //
        // GET: /ShoppingCart/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult AddToCart(int id)
        {

            // Retrieve the album from the database
            var addedAlbum = en.Albums
                .Single(album => album.AlbumId == id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

           // cart.AddToCart(addedAlbum); 

            // Go back to the main store page for more shopping

            return RedirectToAction("Index");
        }

        //
        // GET: /ShoppingCart/Create

        public ActionResult Create()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            List<Cart> CartItems = cart.GetCartItems();

            // Return the view
            return View(CartItems);
        } 

        //
        // POST: /ShoppingCart/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /ShoppingCart/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ShoppingCart/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ShoppingCart/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ShoppingCart/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost, ActionName("RemoveFromCart")]
        public ActionResult RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = en.Carts.Single(
            cart => cart.CartId == session_emp
            && cart.RecordId == id);

            //int itemCount = 0;

            /*if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = (int)cartItem.Count;
                }
                else
                {*/
            en.Carts.Remove(cartItem);
            //}

            // Save changes
            en.SaveChanges();

            return RedirectToAction("Index2", "ShoppingCart");
            //return RedirectToAction("Index");
        }

        [HttpPost, ActionName("RemoveFromCart2")]
        public ActionResult RemoveFromCart2(IEnumerable<int> carts)
        {
            int cart_id = 0;
            foreach (var cart in carts)
            {
                cart_id = Convert.ToInt32(Session[cart.ToString()]);
            }

            // Get the cart
            var cartItem = en.Carts.Single(
            cart => cart.CartId == session_emp
            && cart.RecordId == cart_id);

            en.Carts.Remove(cartItem);

            en.SaveChanges();

            return RedirectToAction("Index2", "ShoppingCart");
        }

        public ActionResult RemoveFromCart3(int cart_id)
        {
            // Get the cart
            var cartItem = en.Carts.Single(
            cart => cart.CartId == session_emp
            && cart.RecordId == cart_id);

            en.Carts.Remove(cartItem);

            en.SaveChanges();

            return RedirectToAction("Index2", "ShoppingCart");
        }
    }
}
