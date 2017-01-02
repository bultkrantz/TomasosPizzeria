using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;
using Microsoft.AspNetCore.Identity;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly TomasosContext _tomasosContext;
        private UserManager<AppUser> _userManager;

        public CustomerController(TomasosContext tomasosContext, UserManager<AppUser> UserManager)
        {
            _tomasosContext = tomasosContext;
            _userManager = UserManager;
        }

        [Authorize(Roles = "RegularUser")]
        public async Task<IActionResult> Index(AppUser user)
        {
            ViewBag.userdata = GetData(nameof(Index));
            AppUser customer = await _userManager.FindByIdAsync(user.Id);
            //var vm = CreateLoggedInModel(kundId, new List<int>());
            var vm = CreateLoggedInModel(customer, new List<int>());
            return View(vm);
        }


        //public IActionResult Index(int id)
        //{
        //    var listOfPizzaFoodModels = new List<FoodModel>();
        //    var listOfPastaFoodModels = new List<FoodModel>();
        //    var listOfSalladFoodModels = new List<FoodModel>();

        //    var pizzaList = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 1).ToList();
        //    var pastaList = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 2).ToList();
        //    var salladList = _tomasosContext.Matratt.Where(x => x.MatrattTyp == 3).ToList();

        //    var pizzaModelList = FoodModels(pizzaList, listOfPizzaFoodModels);
        //    var pastaModelList = FoodModels(pastaList, listOfPastaFoodModels);
        //    var salladModelList = FoodModels(salladList, listOfSalladFoodModels);

        //    var customer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == id);

        //    var loggedInModel = new LoggedInModel()
        //    {
        //        Kund = customer,
        //        Pizza = pizzaModelList,
        //        Pasta = pastaModelList,
        //        Sallad = salladModelList,
        //    };

        //    ViewBag.shoppingcart = loggedInModel.ShoppingCart;

        //    return View(loggedInModel);
        //}

        public IActionResult Edit(int id)
        {
            var customer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == id);
            var loggedInModel = new LoggedInModel()
            {
                //User = customer,
                Pizza = null,
                Pasta = null,
                Sallad = null
            };
            return View(loggedInModel);
        }

        public IActionResult LogOut()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SaveChanges(Kund kund)
        {
            if (ModelState.IsValid)
            {
                //Kollar om användarnamn eller email är upptagna
                var oldCustomerInfo = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == kund.KundId);
                var usernameTaken = _tomasosContext.Kund.Any(x => x.AnvandarNamn.ToLower() == kund.AnvandarNamn.ToLower());
                var emailRegistered = _tomasosContext.Kund.Any(x => x.Email.ToLower() == kund.Email.ToLower());
                var usernameNotChanged = kund.AnvandarNamn == oldCustomerInfo.AnvandarNamn;
                var emailNotChanged = kund.Email == oldCustomerInfo.Email;

                ViewBag.username = usernameTaken ? "Användarnamnet är tyvär upptaget" : null;

                ViewBag.email = emailRegistered ? "Emailen är redan registrerad" : null;

                if (usernameNotChanged)
                {
                    ViewBag.username = null;
                }
                if (emailNotChanged)
                {
                    ViewBag.email = null;
                }

                if (usernameTaken == false && emailRegistered == false || usernameNotChanged && emailNotChanged)
                {
                    var editedCustomer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == kund.KundId);

                    if (editedCustomer != null)
                    {
                        editedCustomer.KundId = kund.KundId;
                        editedCustomer.AnvandarNamn = kund.AnvandarNamn;
                        editedCustomer.Bestallning = kund.Bestallning;
                        editedCustomer.Email = kund.Email;
                        editedCustomer.Gatuadress = kund.Gatuadress;
                        editedCustomer.Losenord = kund.Losenord;
                        editedCustomer.Namn = kund.Namn;
                        editedCustomer.Postnr = kund.Postnr;
                        editedCustomer.Postort = kund.Postort;
                        editedCustomer.Telefon = kund.Telefon;

                        _tomasosContext.Kund.Update(editedCustomer);
                        _tomasosContext.SaveChanges();

                        var loggedInModel = new LoggedInModel()
                        {
                            //Kund = editedCustomer,
                            Pizza = null,
                            Pasta = null,
                            Sallad = null
                        };

                        TempData["success"] = "Ändringar sparade";
                        return View("Edit", loggedInModel);
                    }
                }
            }
            //TODO måste returnera en LoggedInModel
            return View("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string customerId, int foodId, List<int> shoppingCart)
        {
            AppUser user = await _userManager.FindByIdAsync(customerId);
            var vm = CreateLoggedInModel(user, shoppingCart);
            var orderedFood = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == foodId);

            vm.ShoppingCart.Add(orderedFood);
            TempData["success"] = "Du lade till " + orderedFood.MatrattNamn + " i varukorgen";

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int foodId, string customerId, List<int> shoppingCart)
        {
            AppUser user = await _userManager.FindByIdAsync(customerId);
            var vm = CreateLoggedInModel(user, shoppingCart);
            var foodToRemove = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == foodId);

            vm.ShoppingCart.Remove(foodToRemove);
            TempData["success"] = "Du tog bort " + foodToRemove.MatrattNamn + " från varukorgen";

            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult SendOrder(int customerId, List<int> shoppingCart)
        {
            var orderList = new List<Matratt>();
            foreach (var i in shoppingCart)
            {
                var food = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == i);
                orderList.Add(food);
            }

            var order = new Bestallning()
            {
                BestallningDatum = DateTime.Now,
                Totalbelopp = orderList.Sum(food => food.Pris),
                KundId = customerId,
                Levererad = false
            };

            _tomasosContext.Bestallning.Add(order);
            _tomasosContext.SaveChanges();

            var orderFoodList = _tomasosContext.BestallningMatratt;

            foreach (var matratt in orderList)
            {
                var orderFood = new BestallningMatratt()
                {
                    BestallningId = order.BestallningId,
                    MatrattId = matratt.MatrattId,
                    Antal = orderList.Count(x => x.MatrattId == matratt.MatrattId)
                };
                orderFoodList.Add(orderFood);
                _tomasosContext.SaveChanges();
            }
            var customer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == customerId);
            var userInfo = _userManager.Users.FirstOrDefault(x => x.Email == customer.Email);
            var vm = CreateLoggedInModel(userInfo, shoppingCart);
            vm.ShoppingCart.Clear();
            TempData["success"] = "Beställning skickad!";

            return View("Index", vm);
        }

        //Metod för att skapa en LoggedInModel
        public LoggedInModel CreateLoggedInModel(AppUser user, List<int> shoppingCart)
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

            var foodsToOrder = new List<Matratt>();

            foreach (var i in shoppingCart)
            {
                var food = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == i);
                foodsToOrder.Add(food);
            }

            var vm = new LoggedInModel()
            {
                UserInfo = user,
                Customer = _tomasosContext.Kund.FirstOrDefault(x => x.Email == user.Email),
                Pizza = pizzaModelList,
                Pasta = pastaModelList,
                Sallad = salladModelList,
                ShoppingCart = foodsToOrder
            };

            return vm;
        }

        public async Task<AppUser> GetUser(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            return user;
        }

        //Metod för att skapa en lista med FoodModels
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

        private Dictionary<string, object> GetData(string actionName) => new Dictionary<string, object>
        {
            ["Action"] = actionName,
            ["User"] = HttpContext.User.Identity.Name,
            ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
            ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
            ["In Users Role"] = HttpContext.User.IsInRole("Users")
        };
    }
}
