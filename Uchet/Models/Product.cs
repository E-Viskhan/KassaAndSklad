using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uchet.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int? QuantityStock { get; set; }
        public int? PurchasePrice { get; set; }
        public int? SalePrice { get; set; }
        public string Picture { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
