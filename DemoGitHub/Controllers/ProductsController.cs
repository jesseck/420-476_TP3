using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TP3.Models;
using System.IO;

namespace DemoGitHub.Controllers
{
    public class ProductsController : Controller
    {
        private NorthWindsEntities db = new NorthWindsEntities();

        public ActionResult Index(string pseudo, string password, string check)
        {
            Session["FirstNameUser"] = null;
            Session["LastNameUser"] = null;

            if (pseudo != null && password != null)
            {
                var Users = db.Users;

                foreach (var u in Users)
                {
                    if (pseudo == u.Login && password == u.Password)
                    {
                        Session["FirstNameUser"] = u.FirstName;
                        Session["LastNameUser"] = u.LastName;
                        return RedirectToAction("Products");
                    }
                    ViewBag.message = "Pseudo ou mot de passe incorect";
                }

            }

            return View();
        }

        // GET: Products
        public ActionResult Products()
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

            var products = db.Products.Include(p => p.Categories).Include(p => p.Suppliers);
            Create();
            return View(products.ToList());
        }

        [HttpPost]
        public ActionResult Products(String request, String category)
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

            Create();

            var products = db.Products.Where(p => p.CategoryID == 1);

            if (category != "all")
            {
                products = db.Products.Where(p => p.ProductName.Contains(request) && p.Categories.CategoryName == category).Include(p => p.Categories).Include(p => p.Suppliers);
            }
            else
            {
                products = db.Products.Where(p => p.ProductName.Contains(request)).Include(p => p.Categories).Include(p => p.Suppliers);
            }

            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,Photo")] Products products)
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var fileExtension = Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName + fileExtension);
                        file.SaveAs(path);
                        products.Photo = fileName + fileExtension;
                    }
                }
                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Products");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
            return View(products);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
            return View(products);
        }

        // POST: Products/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,Photo")] Products products)
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var fileExtension = Path.GetExtension(file.FileName);
                        if (fileExtension != "png" && fileExtension != "jpg") {
                            ViewBag.type = "Le fichier doit être un png ou un jpg";
                            return View();
                        }
                            var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName + fileExtension);
                            file.SaveAs(path);
                            products.Photo = fileName + fileExtension;
                        }
                    }
                    db.Entry(products).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Products");
                }
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
                return View(products);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((string)Session["FirstNameUser"] == null)
            {
                return RedirectToAction("Index");
            }

            Products products = db.Products.Find(id);
            db.Products.Remove(products);
            db.SaveChanges();
            return RedirectToAction("Products");
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