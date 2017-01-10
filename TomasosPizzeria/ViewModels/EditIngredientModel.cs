using System.Collections.Generic;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class EditIngredientModel
    {
        public Produkt Produkt { get; set; }
        public List<Produkt> Produkter { get; set; }
    }
}
