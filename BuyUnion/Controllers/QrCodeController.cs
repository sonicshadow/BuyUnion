using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Text;


namespace BuyUnion.Controllers
{
    public class QrCodeController : Controller
    {
        [HttpGet]
        public FileResult Index(string data)
        {
            var steam = new System.IO.MemoryStream();
            Image image = QrCode.Generate(data);
            image.Save(steam, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
            return File(steam.ToArray(), "image/jpeg");
        }
    }
}