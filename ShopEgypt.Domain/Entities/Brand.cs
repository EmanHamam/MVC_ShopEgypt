using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ShopEgypt.Domain.Entities
{
    public class Brand
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}
