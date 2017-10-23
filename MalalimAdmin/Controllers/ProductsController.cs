using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MalalimAdmin.Models;
using Microsoft.AspNet.Identity;

namespace MalalimAdmin.Controllers
{
    [CustomAuthorize]
    public class ProductsController : Controller
    {
        private MalalimEntities db = new MalalimEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.tbl_AdminUsers);
            return View(products.Where(p => p.IsClosed != true).ToList());
        }

        public ActionResult ClosedProducts()
        {
            var products = db.Products.Include(p => p.tbl_AdminUsers);
            return View(products.Where(p => p.IsClosed == true).ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.tbl_AdminUsers, "UserId", "Email");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Name,Price,CouponPrice,Image1,Image2,Image3,Image4,IsFeatured,IsVisible,CreatedBy,ExpiryDate,TotalCoupons,MaxCouponsPerOrder")] Product product)
        {
            var _userId = User.Identity.GetUserId();
            product.CreatedOn = DateTime.Now;
            product.CreatedBy = _userId;
            product.RemainingCoupons = product.TotalCoupons;
            if (ModelState.IsValid)
            {
                List<Coupon> Coupons = new List<Coupon>();
                Random rnd = new Random();
                string _code = "";
                for (int i = 0; i < product.TotalCoupons; i++)
                {
                    Coupon _coupon = new Coupon();
                    _code = rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + "-" +
                                rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + "-" +
                                   rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + "-" +
                                       rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "" + rnd.Next(1, 10) + "";

                    _coupon.Code = _code;
                    _coupon.Price = product.CouponPrice;
                    _coupon.ProductId = product.ProductId;
                    _coupon.CreatedOn = product.CreatedOn;
                    Coupons.Add(_coupon);
                }
                db.Coupons.AddRange(Coupons);
                db.Products.Add(product);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.tbl_AdminUsers, "UserId", "Email", product.CreatedBy);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            if (product.IsClosed == true)
            {
                return HttpNotFound();
            }
            
            ViewBag.CreatedBy = new SelectList(db.tbl_AdminUsers, "UserId", "Email", product.CreatedBy);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Name,Price,CouponPrice,Image1,Image2,Image3,Image4,IsFeatured,IsVisible,CreatedBy,ExpiryDate,TotalCoupons,MaxCouponsPerOrder")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.tbl_AdminUsers, "UserId", "Email", product.CreatedBy);
            return View(product);
        }
        public ActionResult ProductCoupon(int? ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == ProductId);
            string _productName = product.Name;
            ViewBag.ProductName = _productName;
            var coupons = db.Coupons.AsNoTracking().Where(c => c.ProductId == ProductId);
            return View(coupons.ToList());
        }
        public ActionResult RandomCoupon(int? productId)
        {

            if (productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var coupons = db.Coupons.Where(c => c.ProductId == productId && c.IsDrawed == true).ToList();
            if (coupons != null && coupons.Count > 0)
            {
                var SelectedArray = coupons.Select(m => m.CouponId).ToArray<long>();
                Random rnd = new Random();
                long RandomNumber = SelectedArray[rnd.Next(SelectedArray.Length)];
                var coupon = coupons.FirstOrDefault(c => c.CouponId == RandomNumber);
                coupon.IsWin = true;
                var product = db.Products.FirstOrDefault(p => p.ProductId == productId).IsClosed = true;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                    throw e;
                }
                return View(coupon);
            }
            else
            {
                return View();
            }
        }

        public ActionResult ChosenCoupon(int? productId)
        {

            if (productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var coupon = db.Coupons.FirstOrDefault(c => c.ProductId == productId && c.IsDrawed == true);
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId).IsClosed = true;
            coupon.IsWin = true;
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }
            return View(coupon);
        }
        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            //db.Products.Remove(product);
            product.IsClosed = true;
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
