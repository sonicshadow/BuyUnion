using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
namespace BuyUnion.Models
{
    [NotMapped]
    public class ProductCreateEditViewModel : Product
    {
        public ProductCreateEditViewModel()
        {
        }

        public ProductCreateEditViewModel(Product product)
        {

            Commission = product.Commission;
            CreateDateTime = product.CreateDateTime;
            ID = product.ID;
            Name = product.Name;
            OriginalPrice = product.OriginalPrice;
            Price = product.Price;
            Remark = product.Remark;
            Stock = product.Stock;
            Sales = product.Sales;
            Weight = product.Weight;
            Image.Images = new string[] { product.Image };
            DetailsImage.Images = product.DetailsImage.SplitToArray<string>(',').ToArray();
        }

        [Required]
        [Display(Name = "商品名")]
        public new string Name { get; set; }

        [Display(Name = "主图")]
        public new FileUpload Image { get; set; } = new FileUpload { Max = 1, Type = FileType.Image, Name = "Image" };

        [Display(Name = "内页轮播图")]
        public new FileUpload DetailsImage { get; set; } = new FileUpload { Max = 10, Type = FileType.Image, Name = "Image" };

        [AllowHtml]
        [Display(Name = "商品简介")]
        public new string Remark { get; set; }

    }
}