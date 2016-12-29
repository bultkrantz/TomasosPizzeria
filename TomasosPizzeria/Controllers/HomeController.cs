using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly TomasosContext _tomasosContext;
        public Kund InloggadKund { get; set; }

        public HomeController(TomasosContext tomasosContext)
        {
            _tomasosContext = tomasosContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.pizza = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 1).ToList();
            ViewBag.pasta = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 2).ToList();
            ViewBag.sallad = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 3).ToList();
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Kund kund)
        {
            var listOfPizzaFoodModels = new List<FoodModel>();
            var listOfPastaFoodModels = new List<FoodModel>();
            var listOfSalladFoodModels = new List<FoodModel>();

            var pizzaList = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 1).ToList();
            var pastaList = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 2).ToList();
            var salladList = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 3).ToList();

            var pizzaModelList = FoodModels(pizzaList, listOfPizzaFoodModels);
            var pastaModelList = FoodModels(pastaList, listOfPastaFoodModels);
            var salladModelList = FoodModels(salladList, listOfSalladFoodModels);

            var inloggadKund = _tomasosContext.Kund.FirstOrDefault(x => x.AnvandarNamn == kund.AnvandarNamn && x.Losenord == kund.Losenord);

            var loggedInModel = new LoggedInModel()
            {
                Kund = inloggadKund,
                Pizza = pizzaModelList,
                Pasta = pastaModelList,
                Sallad = salladModelList
            };

            if (inloggadKund != null)
            {
                ViewBag.shoppingcart = loggedInModel.ShoppingCart;
                return View("Customer", loggedInModel);
            }

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Kund kund)
        {
            if (ModelState.IsValid)
            {
                var userNameTaken = _tomasosContext.Kund.FirstOrDefault(x => x.AnvandarNamn.ToLower() == kund.AnvandarNamn.ToLower());
                var emailRegistered = _tomasosContext.Kund.FirstOrDefault(x => x.Email.ToLower() == kund.Email.ToLower());

                ViewBag.username = userNameTaken != null ? "Användarnamnet är tyvär upptaget" : null;

                ViewBag.email = emailRegistered != null ? "Emailen är redan registrerad" : null;

                if (userNameTaken == null && emailRegistered == null)
                {
                    ViewBag.username = null;
                    ViewBag.email = null;
                    _tomasosContext.Kund.Add(kund);
                    _tomasosContext.SaveChanges();
                    ModelState.Clear();
                    TempData["success"] = "Användare skapad";
                }
            }
            return View();
        }


        public IActionResult Customer()
        {
            var vmList = new List<FoodModel>();
            var foodList = _tomasosContext.Matratt.ToList();

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
                var vm = new FoodModel()
                {
                    Matratt = matratt,
                    Produkter = listOfProdukter
                };
                vmList.Add(vm);
            }
            return View("test", vmList);
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
