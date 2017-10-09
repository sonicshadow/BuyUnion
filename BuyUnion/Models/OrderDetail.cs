using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }

        public int OrderID { get; set; }

        public virtual Order Order { get; set; }

        public int ProductID { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }
    }
}