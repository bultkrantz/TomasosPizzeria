using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using Remotion.Linq.Clauses.ResultOperators;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TomasosContext _tomasosContext;

        public CustomerController(TomasosContext tomasosContext)
        {
            _tomasosContext = tomasosContext;
        }

        // GET: /<controller>/

        public IActionResult Index(int id)
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

            var customer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == id);

            var loggedInModel = new LoggedInModel()
            {
                Kund = customer,
                Pizza = pizzaModelList,
                Pasta = pastaModelList,
                Sallad = salladModelList,
            };

            ViewBag.shoppingcart = loggedInModel.ShoppingCart;

            return View(loggedInModel);
        }

        public IActionResult Edit(int id)
        {
            var customer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == id);
            var loggedInModel = new LoggedInModel()
            {
                Kund = customer,
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
                            Kund = editedCustomer,
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
        public IActionResult AddToCart(int customerId, int foodId, List<int> shoppingCart)
        {
            var vm = CreateLoggedInModel(customerId, shoppingCart);
            var orderedFood = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == foodId);

            vm.ShoppingCart.Add(orderedFood);
            TempData["success"] = "Du lade till " + orderedFood.MatrattNamn + " i varukorgen";

            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int foodId, int customerId, List<int> shoppingCart)
        {
            var vm = CreateLoggedInModel(customerId, shoppingCart);
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
                orderFoodList.AddAsync(orderFood);
                _tomasosContext.SaveChanges();
            }

            var vm = CreateLoggedInModel(customerId, shoppingCart);
            vm.ShoppingCart.Clear();
            TempData["success"] = "Beställning skickad!";

            return View("Index", vm);
        }

        //Metod för att skapa en LoggedInModel
        public LoggedInModel CreateLoggedInModel(int customerId, List<int> shoppingCart)
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
                Kund = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == customerId),
                Pizza = pizzaModelList,
                Pasta = pastaModelList,
                Sallad = salladModelList,
                ShoppingCart = foodsToOrder
            };

            return vm;
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
    }
}
