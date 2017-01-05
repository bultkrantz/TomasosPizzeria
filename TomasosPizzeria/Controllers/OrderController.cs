using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TomasosPizzeria.Infrastructure;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

namespace TomasosPizzeria.Controllers
{
    public class OrderController : Controller
    {
        private TomasosContext _tomasosContext;

        public OrderController(TomasosContext tomasosContext)
        {
            _tomasosContext = tomasosContext;
        }

        public IActionResult Index(AppUser user)
        {
            var vm = CreateLoggedInModel(user);
            return View(vm);
        }



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