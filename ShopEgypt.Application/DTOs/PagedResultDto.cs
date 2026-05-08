using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; } 
    }
}
