using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly TomasosContext _tomasosContext;
        private UserManager<AppUser> _userManagager;

        public HomeController(TomasosContext tomasosContext, UserManager<AppUser> userManagager)
        {
            _tomasosContext = tomasosContext;
            _userManagager = userManagager;
        }

        public IActionResult Index()
        {
            ViewBag.pizza = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 1).ToList();
            ViewBag.pasta = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 2).ToList();
            ViewBag.sallad = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 3).ToList();
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ViewResult> Create(Kund kund)
        {
            if (ModelState.IsValid)
            {
                var userNameTaken = _tomasosContext.Kund.FirstOrDefault(x => x.AnvandarNamn.ToLower() == kund.AnvandarNamn.ToLower());
                var emailRegistered = _tomasosContext.Kund.FirstOrDefault(x => x.Email.ToLower() == kund.Email.ToLower());

                ViewBag.username = userNameTaken != null ? "Användarnamnet är tyvär upptaget" : null;

                ViewBag.email = emailRegistered != null ? "Emailen är redan registrerad" : null;

                if (userNameTaken == null && emailRegistered == null)
                {
                    AppUser user = new AppUser()
                    {
                        UserName = kund.AnvandarNamn,
                        Email = kund.Email,
                        CustomerId = kund.KundId
                    };
                    IdentityResult useResult = await _userManagager.CreateAsync(user, kund.Losenord);
                    IdentityResult roleResult = await _userManagager.AddToRoleAsync(user, "RegularUser");

                    if (useResult.Succeeded && roleResult.Succeeded)
                    {
                        ViewBag.username = null;
                        ViewBag.email = null;
                        _tomasosContext.Kund.Add(kund);
                        _tomasosContext.SaveChanges();
                        ModelState.Clear();
                        TempData["success"] = "Användare skapad";
                    }
                }
            }
            return View();
        }

        //Metod för att returnera en lista med foodmodels (varje foodmodel innehåller en maträtt och en lista med dess produkter)
        public List<FoodModel> FoodModels(List<Matratt> foodList, List<FoodModel> foodModels)
        {
            foreach (var matratt in foodList)
            {
                var matrattProduktLista = _tomasosContext.MatrattProdukt.Where(x => x.MatrattId == matratt.MatrattId).ToList();
                var listOfProdukter = new List<Produkt>();
                foreach (var matrattProdukt in matrattProduktLista)
                {
                    var productList = _tomasosContext.Produkt.Where(x => x.ProduktId == matrattProdukt.ProduktId).ToList();
                    foreach (var produkt in productList)
                    {
                        listOfProdukter.Add(produkt);
                    }
                }
                var foodModel = new FoodModel()
                {
                    Matratt = matratt,
                    Produkter = listOfProdukter
                };
                foodModels.Add(foodModel);
            }
            return foodModels;
        }
    }
}
