using System.Collections.Generic;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class FoodModel
    {
        public Matratt Matratt { get; set; }
        public List<Produkt> Produkter { get; set; }
    }
}
