using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;
using Microsoft.AspNetCore.Identity;
using TomasosPizzeria.Infrastructure;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    [Authorize(Roles = "RegularUser")]
    public class CustomerController : Controller
    {
        private readonly TomasosContext _tomasosContext;
        private UserManager<AppUser> _userManager;

        public CustomerController(TomasosContext tomasosContext, UserManager<AppUser> userManager)
        {
            _tomasosContext = tomasosContext;
            _userManager = userManager;
        }

        public IActionResult Index(AppUser user)
        {
            var vm = CreateLoggedInModel(user);
            vm.Cart = GetCart();
            return View(vm);
        }

        public IActionResult ToIndex(string id)
        {
            var userInfo = GetUser(id);
            var vm = CreateLoggedInModel(userInfo);
            return View("Index", vm);
        }

        public IActionResult Edit(string id)
        {
            var userInfo = GetUser(id);
            var vm = CreateLoggedInModel(userInfo);
            vm.Cart = GetCart();
            return View(vm);
        }

        public IActionResult LogOut()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SaveChanges(Kund customer)
        {
            if (ModelState.IsValid)
            {
                //Kollar om användarnamn eller email är upptagna
                var oldCustomerInfo = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == customer.KundId);
                var oldPassword = oldCustomerInfo.Losenord;
                var usernameTaken = _tomasosContext.Kund.Any(x => x.AnvandarNamn.ToLower() == customer.AnvandarNamn.ToLower());
                var emailRegistered = _tomasosContext.Kund.Any(x => x.Email.ToLower() == customer.Email.ToLower());
                var usernameNotChanged = customer.AnvandarNamn == oldCustomerInfo.AnvandarNamn;
                var emailNotChanged = customer.Email == oldCustomerInfo.Email;

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
                    var editedCustomer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == customer.KundId);

                    if (editedCustomer != null)
                    {
                        var oldUser = _userManager.Users.FirstOrDefault(x => x.CustomerId == editedCustomer.KundId);

                        editedCustomer.KundId = customer.KundId;
                        editedCustomer.AnvandarNamn = customer.AnvandarNamn;
                        editedCustomer.Bestallning = customer.Bestallning;
                        editedCustomer.Email = customer.Email;
                        editedCustomer.Gatuadress = customer.Gatuadress;
                        editedCustomer.Losenord = customer.Losenord;
                        editedCustomer.Namn = customer.Namn;
                        editedCustomer.Postnr = customer.Postnr;
                        editedCustomer.Postort = customer.Postort;
                        editedCustomer.Telefon = customer.Telefon;

                        oldUser.Email = editedCustomer.Email;
                        oldUser.UserName = editedCustomer.AnvandarNamn;
                        await _userManager.ChangePasswordAsync(oldUser, oldPassword, editedCustomer.Losenord);
                        await _userManager.UpdateAsync(oldUser);

                        _tomasosContext.Kund.Update(editedCustomer);
                        _tomasosContext.SaveChanges();

                        var loggedInModel = new LoggedInModel()
                        {
                            Customer = editedCustomer,
                            UserInfo = oldUser,
                            Pizza = null,
                            Pasta = null,
                            Sallad = null
                        };

                        TempData["success"] = "Ändringar sparade";
                        return View("Edit", loggedInModel);
                    }
                }
            }
            return View("Edit");
        }


        [Authorize(Roles = "PremiumUser")]
        public IActionResult Bonus(string id)
        {
            AppUser user = GetUser(id);
            var vm = CreateLoggedInModel(user);
            return View(vm);
        }

        //Metod för att skapa en LoggedInModel
        public LoggedInModel CreateLoggedInModel(AppUser user)
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

            var vm = new LoggedInModel()
            {
                UserInfo = user,
                Customer = _tomasosContext.Kund.FirstOrDefault(x => x.Email == user.Email),
                Pizza = pizzaModelList,
                Pasta = pastaModelList,
                Sallad = salladModelList,
                Cart = GetCart()
            };

            return vm;
        }

        private AppUser GetUser(string id)
        {
            AppUser user = _userManager.Users.FirstOrDefault(x => x.Id == id);
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

        private Cart GetCart()
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
            return cart;
        }
    }
}
