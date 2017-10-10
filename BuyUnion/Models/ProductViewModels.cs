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
        [Required]
        [Display(Name = "商品名")]
        public new string Name { get; set; }

        [Display(Name = "主图片")]
        public new FileUpload Image { get; set; } = new FileUpload { Max = 1, Type = FileType.Image, Name = "Image" };

        [Display(Name = "次图片")]
        public new FileUpload DetailsImage { get; set; } = new FileUpload { Max = 10, Type = FileType.Image, Name = "Image" };

        [AllowHtml]
        [Display(Name = "商品简介")]
        public new string Remark { get; set; }

    }
}