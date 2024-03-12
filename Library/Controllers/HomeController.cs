using Library.Models;
using Library.Models.ViewModels;
using Library.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeViewModel()
            {
                Carti = _db.Carte,
            };

            return View(homeVM);
        }
        public ActionResult FiltrarePret()
        {
            var homeVM = new HomeViewModel()
            {
                Carti = _db.Carte.Where(carte => carte.Price >= 150 && carte.Price <= 500)
            };
            return View("Index", homeVM);
        }
        public ActionResult FiltrareAutor() 
        {
            var homeVM = new HomeViewModel()
            {
                Carti = _db.Carte.AsEnumerable().Where(carte =>
                {
                    var firstLetter = !"aeiouAEIOU".Contains(carte.Author[0].ToString());
                    return firstLetter;
                }).ToList()
            };
            return View("Index", homeVM);
        }
        public ActionResult CartePretMax()
        {
            var homeVM = new HomeViewModel()
            {
                Carti = _db.Carte.MaxsBy(carte => carte.Price)
            };
            return View("Index", homeVM);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
