using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FoodController : Controller
    {
        private TomasosContext _tomasosContext;

        public FoodController(TomasosContext TomasosContext)
        {
            _tomasosContext = TomasosContext;
        }

        public IActionResult Edit(int id)
        {
            var vm = new EditFoodModel()
            {
                Matratt = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == id),
                Produkt = new Produkt()
            };

            ViewBag.categories = CreateSelectList();
            ViewBag.products = _tomasosContext.Produkt.ToList();

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(Matratt matratt, List<int> productId)
        {
            var foodToEdit = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == matratt.MatrattId);
            foodToEdit.MatrattId = matratt.MatrattId;
            foodToEdit.MatrattNamn = matratt.MatrattNamn;
            foodToEdit.Pris = matratt.Pris;
            foodToEdit.MatrattTyp = matratt.MatrattTyp;

            _tomasosContext.Matratt.Update(foodToEdit);
            _tomasosContext.SaveChanges();

            var oldProductList = _tomasosContext.MatrattProdukt.Where(x => x.MatrattId == matratt.MatrattId).ToList();

            foreach (var matrattProdukt in oldProductList)
            {
                _tomasosContext.MatrattProdukt.Remove(matrattProdukt);
                _tomasosContext.SaveChanges();
            }

            foreach (var i in productId)
            {
                var matrattProdukt = new MatrattProdukt()
                {
                    MatrattId = matratt.MatrattId,
                    ProduktId = i
                };
                _tomasosContext.MatrattProdukt.Add(matrattProdukt);
                _tomasosContext.SaveChanges();
            }

            var vm = CreateFoodModel(matratt.MatrattId);
            ViewBag.categories = CreateSelectList().ToList();
            ViewBag.products = _tomasosContext.Produkt;

            return RedirectToAction("Index", "Admin", vm);
        }

        private FoodModel CreateFoodModel(int foodId)
        {
            var food = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == foodId);
            var products = _tomasosContext.Produkt.ToList();
            var vm = new FoodModel()
            {
                Matratt = food,
                Produkter = products
            };
            return vm;
        }

        private List<SelectListItem> CreateSelectList()
        {
            var categories = _tomasosContext.MatrattTyp.ToList();
            var selectList = new List<SelectListItem>();
            foreach (var category in categories)
            {
                var selectItem = new SelectListItem()
                {
                    Text = category.Beskrivning,
                    Value = category.MatrattTyp1.ToString()
                };
                selectList.Add(selectItem);
            }
            return selectList;
        }

        public IActionResult Delete(int id)
        {
            var foodToRemove = _tomasosContext.Matratt.FirstOrDefault(x => x.MatrattId == id);
            var ingredientsToRemove = _tomasosContext.MatrattTyp.Where(x => x.MatrattTyp1 == foodToRemove.MatrattTyp).ToList();

            _tomasosContext.Matratt.Remove(foodToRemove);
            _tomasosContext.SaveChanges();
            TempData["success"] = "Maträtt raderad";
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult CreateFood()
        {
            ViewBag.categories = CreateSelectList().ToList();
            ViewBag.products = _tomasosContext.Produkt;
            return View();
        }

        [HttpPost]
        public IActionResult CreateFood(Matratt matratt, List<int> productId)
        {
            var food = new Matratt()
            {
                MatrattNamn = matratt.MatrattNamn,
                Beskrivning = matratt.Beskrivning,
                MatrattTyp = matratt.MatrattTyp,
                Pris = matratt.Pris
            };
            _tomasosContext.Matratt.Add(food);
            _tomasosContext.SaveChanges();

            productId.ForEach(x =>
            {
                var matrattProdukt = new MatrattProdukt()
                {
                    MatrattId = food.MatrattId,
                    ProduktId = x
                };
                _tomasosContext.MatrattProdukt.Add(matrattProdukt);
                _tomasosContext.SaveChanges();
            });

            TempData["success"] = "Maträtt skapad";
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult CreateIngredient()
        {
            var vm = new EditIngredientModel()
            {
                Produkt = new Produkt(),
                Produkter = _tomasosContext.Produkt.ToList()
            };
            ViewBag.products = _tomasosContext.Produkt.ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Createingredient(Produkt produkt)
        {
            var ingredient = new Produkt()
            {
                ProduktNamn = produkt.ProduktNamn
            };
            _tomasosContext.Produkt.Add(ingredient);
            _tomasosContext.SaveChanges();
            ModelState.Clear();
            ViewBag.products = _tomasosContext.Produkt.ToList();

            TempData["success"] = "Ny ingrediens skapad";
            return View();
        }

        [HttpPost]
        public IActionResult DeleteIngredient(List<int> produkter)
        {
            var productList = produkter.Select(i => _tomasosContext.Produkt.FirstOrDefault(x => x.ProduktId == i)).ToList();
            productList.ForEach(x => _tomasosContext.Remove(x));
            _tomasosContext.SaveChanges();
            ModelState.Clear();
            TempData["success"] = "Ingredienser borttagna";
            return RedirectToAction("CreateIngredient");
        }
    }
}
