﻿using System.Collections.Generic;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class LoggedInModel
    {
        public AppUser UserInfo { get; set; }
        public Kund Customer { get; set; }
        public List<FoodModel> Pizza { get; set; }
        public List<FoodModel> Pasta { get; set; }
        public List<FoodModel> Sallad { get; set; }
        public Cart Cart { get; set; }
    }
}
