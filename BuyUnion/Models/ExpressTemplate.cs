using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class ExpressTemplate
    {
        public int ID { get; set; }

        [Display(Name = "店铺")]
        public string ShopID { get; set; }


        [Display(Name = "分组")]
        public string Group { get; set; }

        [Display(Name = "省份")]
        public string Provinces { get; set; }

        [Display(Name = "起重")]
        public decimal Weight { get; set; }

        [Display(Name = "首重价格")]
        public decimal First { get; set; }

        [Display(Name = "续重价格")]
        public decimal Additional { get; set; }
    }
}