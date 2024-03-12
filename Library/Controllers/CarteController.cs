using Library.Models.ViewModels;
using Library.Models;
using Library.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Authorize(Roles = ShopConstants.Admin)]
    public class CarteController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CarteController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Carte> objList = _db.Carte;
            return View(objList);
        }
        //get-upsert
        public IActionResult Upsert(int? id)
        {
            CarteViewModel productViewModel = new()
            {
                Carte = new Carte()
            };


            if (id == null)
            {
                return View(productViewModel);
            }
            else
            {
                productViewModel.Carte = _db.Carte.Find(id);
                if (productViewModel.Carte == null)
                {
                    return NotFound();
                }
                return View(productViewModel);
            }
        }

        //post-upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CarteViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = webHostEnvironment.WebRootPath;

                if (productViewModel.Carte.Id == 0)
                {
                    string upload = webRootPath + ShopConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extention = Path.GetExtension(files[0].FileName);


                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productViewModel.Carte.Image = fileName + extention;

                    _db.Carte.Add(productViewModel.Carte);
                }
                else
                {
                    var objFromBb = _db.Carte.AsNoTracking().FirstOrDefault(u => u.Id == productViewModel.Carte.Id);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + ShopConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extention = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromBb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productViewModel.Carte.Image = fileName + extention;

                    }
                    else
                    {
                        productViewModel.Carte.Image = objFromBb.Image;
                    }
                    _db.Carte.Update(productViewModel.Carte);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productViewModel);
        }

        //get-delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Carte? product = _db.Carte.FirstOrDefault(u => u.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //post-delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            Carte? obj = _db.Carte.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            string upload = webHostEnvironment.WebRootPath + ShopConstants.ImagePath;


            var oldFile = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _db.Carte.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
