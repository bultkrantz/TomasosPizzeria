using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IUserValidator<AppUser> userValidator;
        private IPasswordValidator<AppUser> passwordValidator;
        private IPasswordHasher<AppUser> passwordHasher;
        private UserManager<AppUser> userManager;
        private TomasosContext _tomasosContext;

        public AdminController(UserManager<AppUser> usrMgr, IUserValidator<AppUser> userValid, IPasswordValidator<AppUser> passValid, IPasswordHasher<AppUser> passwordHash, TomasosContext tomasosContext)
        {
            userManager = usrMgr;
            userValidator = userValid;
            passwordValidator = passValid;
            passwordHasher = passwordHash;
            _tomasosContext = tomasosContext;
        }

        public IActionResult UserList() => View(userManager.Users);

        public IActionResult Index(AppUser user)
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

            ViewBag.user = HttpContext.User.Identity.Name;

            var vm = new LoggedInModelAdmin()
            {
                AppUser = user,
                Pizza = pizzaModelList,
                Pasta = pastaModelList,
                Sallad = salladModelList
            };
            return View(vm);
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email
                };

                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("UserList");
                }

                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserList");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("UserList", userManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("UserList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail = await userValidator.ValidateAsync(userManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager,
                        user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user,
                            password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
                if ((validEmail.Succeeded && validPass == null)
                    || (validEmail.Succeeded
                        && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserList");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            }
            return View(user);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

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
            return foodModels.OrderBy(x => x.Matratt.MatrattNamn).ToList();
        }

        public IActionResult Orders() => View(_tomasosContext.Bestallning.ToList());

        public IActionResult DeleteOrder(int id)
        {
            var order = _tomasosContext.Bestallning.FirstOrDefault(x => x.BestallningId == id);
            _tomasosContext.Bestallning.Remove(order);
            _tomasosContext.SaveChanges();
            return RedirectToAction("Orders");
        }

        public IActionResult EditOrder(int id)
        {
            var order = _tomasosContext.Bestallning.FirstOrDefault(x => x.BestallningId == id);
            if (order.Levererad)
            {
                order.Levererad = false;
                _tomasosContext.SaveChanges();
            }
            else
            {
                order.Levererad = true;
                _tomasosContext.SaveChanges();
            }
            return RedirectToAction("Orders");
        }
    }
}
