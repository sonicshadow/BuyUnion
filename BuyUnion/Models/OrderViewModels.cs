using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace BuyUnion.Models
{
    [NotMapped]
    public class SubmitOrderViewModel : Order, IDistrict
    {
        public SubmitOrderViewModel()
        {

        }

    

        public string City { get; set; }

        public string District { get; set; }

        public string Province { get; set; }

        public new List<OrderDetailViewModel> Details { get; set; }
    }

    [NotMapped]
    public class OrderDetailViewModel : OrderDetail
    {
        public string Image { get; set; }


    }

}