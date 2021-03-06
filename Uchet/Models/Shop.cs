﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uchet.Models
{
    public class Shop
    {
        public int Id { get; set; }
        public string OwnerEmail { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public virtual List<Product> Products { get; set; }
    }
}
