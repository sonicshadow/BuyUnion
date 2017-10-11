using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace BuyUnion.Address
{
    public static class AddressList
    {
        private static string xmlProvincesUrl = "~/App_Data/Provinces.xml";
        private static string xmlCitiesUrl = "~/App_Data/Cities.xml";
        private static string xmlDistrictsUrl = "~/App_Data/Districts.xml";

        private static List<Province> _provinces = null;
        private static List<City> _cities = null;
        private static List<District> _district = null;


        static AddressList()
        {
            var server = HttpContext.Current.Server;
            if (_provinces == null)
            {
                _provinces = new List<Province>();
                IEnumerable<XElement> pList = XElement.Load(server.MapPath(xmlProvincesUrl)).Elements();
                foreach (var p in pList)
                {
                    _provinces.Add(new Province
                    {
                        ID = Convert.ToInt32(p.Attribute("ID").Value),
                        Name = p.Attribute("ProvinceName").Value,
                        Citys = null
                    });
                }
            }
            if (_cities == null)
            {
                _cities = new List<City>();
                IEnumerable<XElement> cList = XElement.Load(server.MapPath(xmlCitiesUrl)).Elements();
                foreach (var c in cList)
                {
                    _cities.Add(new City
                    {
                        ID = Convert.ToInt32(c.Attribute("ID").Value),
                        Name = c.Attribute("CityName").Value,
                        ProvinceID = Convert.ToInt32(c.Attribute("PID").Value),
                        ZipCode = c.Attribute("ZipCode").Value,
                        Districts = null,
                        Province = null
                    });
                }
            }
            if (_district == null)
            {
                _district = new List<District>();
                IEnumerable<XElement> aList = XElement.Load(server.MapPath(xmlDistrictsUrl)).Elements();
                foreach (var a in aList)
                {
                    _district.Add(new District
                    {
                        ID = Convert.ToInt32(a.Attribute("ID").Value),
                        Name = a.Attribute("DistrictName").Value,
                        CityID = Convert.ToInt32(a.Attribute("CID").Value),
                        City = null
                    });
                }

                foreach (var p in _provinces)
                {
                    p.Citys = _cities.Where(s => s.ProvinceID == p.ID).ToList();
                    foreach (var c in p.Citys)
                    {
                        c.Province = p;
                        c.Districts = _district.Where(s => s.CityID == c.ID).ToList();
                        foreach (var a in c.Districts)
                        {
                            a.City = c;
                        }
                    }
                }
            }
        }

        public static List<Province> Provinces
        {
            get
            {
                return _provinces;
            }
        }

        public static List<City> Cities
        {
            get
            {
                return _cities;
            }
        }

        public static List<District> Districts
        {
            get
            {
                return _district;
            }
        }
    }

    public class Province
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public List<City> Citys { get; set; }
    }

    public class City
    {
        public int ID { get; set; }

        public int ProvinceID { get; set; }

        public string Name { get; set; }

        public string ZipCode { get; set; }

        public Province Province { get; set; }

        public List<District> Districts { get; set; }
    }

    public class District
    {
        public int ID { get; set; }

        public int CityID { get; set; }

        public string Name { get; set; }

        public City City { get; set; }

        public Province Province
        {
            get { return City.Province; }
        }
    }
}