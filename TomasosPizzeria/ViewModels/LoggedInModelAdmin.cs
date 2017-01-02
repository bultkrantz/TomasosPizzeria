using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class LoggedInModelAdmin
    {
        public AppUser AppUser { get; set; }
        public List<FoodModel> Pizza { get; set; }
        public List<FoodModel> Pasta { get; set; }
        public List<FoodModel> Sallad { get; set; }
    }
}
