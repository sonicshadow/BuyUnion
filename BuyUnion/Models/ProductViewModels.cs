using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class ProductCreateEditViewModel : Product
    {
        [Display(Name = "主图片")]
        public new FileUpload Image { get; set; } = new FileUpload { Max = 1, Type = FileType.Image };

        [Display(Name = "次图片")]
        public new FileUpload DetailsImage { get; set; } = new FileUpload { Max = 0, Type = FileType.Image };
    }
}