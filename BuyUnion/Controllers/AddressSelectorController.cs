using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace BuyUnion.Controllers
{
    public class AddressSelectorController : Controller
    {
        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult GetCity(string name)
        {
            return Json(Address.AddressList.Provinces.FirstOrDefault(s => s.Name == name).Citys.Select(s => new
            {
                s.ID,
                s.Name,
                s.ZipCode,
                s.ProvinceID
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult GetDistrict(string name)
        {
            var d = Address.AddressList.Cities.FirstOrDefault(s => s.Name == name);
            var data = d.Districts.Select(s => new
            {
                s.ID,
                s.Name,
                s.CityID
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}