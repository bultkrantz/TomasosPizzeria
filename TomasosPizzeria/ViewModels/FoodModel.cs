using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class FoodModel
    {
        public Matratt Matratt { get; set; }
        public List<Produkt> Produkter { get; set; }
    }
}
