﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TomasosPizzeria.Models
{
    public class AppUser : IdentityUser
    {
        public int CustomerId { get; set; }
    }
}
