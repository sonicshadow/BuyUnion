using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
namespace BuyUnion.Models
{
    [NotMapped]
    public class SubmitOrderViewModel : IDistrict
    {
        public string City { get; set; }

        public string District { get; set; }

        public string Province { get; set; }

        public string Address { get; set; }

        public string Consignee { get; set; }

        public string PhoneNumber { get; set; }

    }
}