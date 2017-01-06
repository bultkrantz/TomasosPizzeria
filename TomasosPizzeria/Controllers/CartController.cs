using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TomasosPizzeria.Infrastructure;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeria.Controllers
{
    public class CartController : Controller
    {
        private TomasosContext _tomasosContext;
        private UserManager<AppUser> _userManager;
        public CartController(TomasosContext tomasosContext, UserManager<AppUser> userManager)
        {
            _tomasosContext = tomasosContext;
            _userManager = userManager;
        }
        public RedirectToActionResult AddToCart(int matrattId, string customerId)
        {
            Matratt matratt = _tomasosContext.Matratt
            .FirstOrDefault(p => p.MatrattId == matrattId);
            if (matratt != null)
            {
                Cart cart = GetCart();
                cart.AddItem(matratt, 1);
                SaveCart(cart);
            }
            var customer = _userManager.Users.FirstOrDefault(x => x.Id == customerId);
            TempData["success"] = $"{matratt.MatrattNamn} Lades till i varukorgen";
            return RedirectToAction("Index", "Customer", customer);
        }

        public RedirectToActionResult RemoveFromCart(int foodId, string customerId)
        {
            Matratt matratt = _tomasosContext.Matratt
            .FirstOrDefault(p => p.MatrattId == foodId);
            if (matratt != null)
            {
                Cart cart = GetCart();
                cart.RemoveLine(matratt);
                SaveCart(cart);
            }
            var user = _userManager.Users.FirstOrDefault(x => x.Id == customerId);
            TempData["success"] = $"{matratt.MatrattNamn} borttagen från varukorgen";
            return RedirectToAction("Index", "Customer", user);
        }

        public IActionResult CreateOrder(int id)
        {
            Cart cart = GetCart();
            Kund customer = _tomasosContext.Kund.FirstOrDefault(x => x.KundId == id);
            AppUser user = _userManager.Users.FirstOrDefault(x => x.Email == customer.Email);

            if (cart.Lines.Any())
            {
                TempData["discount"] = Convert.ToDouble(cart.ComputeTotalValue()) * DiscountAmount(cart);

                var order = new Bestallning()
                {
                    BestallningDatum = DateTime.Now,
                    KundId = id,
                    Totalbelopp = Convert.ToInt32(cart.ComputeTotalValue()) * Convert.ToInt32(CountDiscount(cart)),
                    Levererad = false
                };
                _tomasosContext.Bestallning.Add(order);
                _tomasosContext.SaveChanges();
                var bonus = GetsBonus(customer, cart, order);

                foreach (var cartLine in cart.Lines.ToList())
                {
                    var orderFood = new BestallningMatratt()
                    {
                        MatrattId = cartLine.Matratt.MatrattId,
                        Antal = cartLine.Quantity,
                        BestallningId = order.BestallningId
                    };
                    _tomasosContext.BestallningMatratt.Add(orderFood);
                    _tomasosContext.SaveChanges();
                    //if (bonus)
                    //{
                    //    TempData["success"] = $"Order skickad {order.BestallningDatum:t}! Du fick en maträtt gratis med bonussystemet. Totalsumma: {order.Totalbelopp}";
                    //}
                    //else
                    //{
                    //    TempData["success"] = $"Order skickad {order.BestallningDatum:t}! Totalsumma: {order.Totalbelopp}";
                    //}
                }
                TempData["time"] = order.BestallningDatum.ToString("f");
                TempData["ordertotal"] = cart.ComputeTotalValue();
                TempData["adress"] = customer.Gatuadress;
                TempData["orderid"] = order.BestallningId;
                TempData["delivery"] = order.BestallningDatum.AddMinutes(45);
                cart.Clear();
                SaveCart(cart);

                return RedirectToAction("Index", "Order", user);
            }
            else
            {
                TempData["success"] = "Din varukorg är tom, går ej att skicka beställning";
                return RedirectToAction("Index", "Customer", user);
            }
        }


        private Cart GetCart()
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
            return cart;
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetJson("Cart", cart);
        }

        private bool GetsBonus(Kund kund, Cart cart, Bestallning order)
        {
            var bonus = 10;
            var isPremium = HttpContext.User.IsInRole("PremiumUser");
            if (isPremium)
            {
                foreach (var cartLine in cart.Lines)
                {
                    kund.Bonus += bonus * cartLine.Quantity;
                    if (kund.Bonus >= 100)
                    {
                        kund.GratisPizza++;
                        kund.Bonus -= 100;
                        _tomasosContext.Kund.Update(kund);
                        _tomasosContext.SaveChanges();
                        SaveCart(cart);
                        return true;
                    }
                }
            }
            return false;
        }

        private double CountDiscount(Cart cart)
        {
            var isPremium = HttpContext.User.IsInRole("PremiumUser");
            if (isPremium)
            {
                var numberOfDished = cart.Lines.Sum(cartLine => cartLine.Quantity);
                if (numberOfDished >= 3)
                {
                    return 0.8;
                }
            }
            return 1;
        }

        private double DiscountAmount(Cart cart)
        {
            var isPremium = HttpContext.User.IsInRole("PremiumUser");
            if (isPremium)
            {
                var numberOfDished = cart.Lines.Sum(cartLine => cartLine.Quantity);
                if (numberOfDished >= 3)
                {
                    return 0.2;
                }
            }
            return 0;
        }

        public IActionResult UseFreePizza(string id)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            var customer = _tomasosContext.Kund.FirstOrDefault(x => x.Email == user.Email);
            customer.GratisPizza--;
            _tomasosContext.Kund.Update(customer);
            _tomasosContext.SaveChanges();
            var cart = GetCart();
            cart.Lines.FirstOrDefault(x => x.Matratt.MatrattTyp == 1).Matratt.Pris = 0;
            SaveCart(cart);
            return RedirectToAction("Index", "Customer", user);
        }
    }
}
