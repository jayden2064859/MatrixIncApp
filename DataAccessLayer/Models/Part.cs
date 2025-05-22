﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Part
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }

        public ICollection<Product> Products { get; } = new List<Product>();
    }
}
