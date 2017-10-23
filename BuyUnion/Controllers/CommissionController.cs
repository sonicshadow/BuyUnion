using BuyUnion.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BuyUnion.Controllers
{
    [Authorize]
    public class CommissionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        private string UserID
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        private void Sidebar()
        {
            ViewBag.Sidebar = "佣金管理";
        }

        // GET: Commission
        [Authorize(Roles = SysRole.CommissionManageRead)]
        public ActionResult Index()
        {
            Sidebar();
            var orderLogs = db.OrderLogs.Where(s => s.Type == Enums.OrderLogType.Pay);
            var list = new List<CommissionViewModel>();
            foreach (var item in orderLogs)
            {
                if (!string.IsNullOrWhiteSpace(item.ExData))
                {
                    var obj = (JObject)JsonConvert.DeserializeObject(item.ExData);
                    list.Add(new CommissionViewModel()
                    {
                        Amount = decimal.Parse(obj["ProxyAmount"].ToString()),
                        ProxyUserID = obj["ProxyID"].ToString(),
                        Time = item.CreateDateTime
                    });
                    if (!string.IsNullOrWhiteSpace(obj["ProxyID"].ToString()))
                    {
                        list.Add(new CommissionViewModel()
                        {
                            Amount = decimal.Parse(obj["ChildProxyAmount"].ToString()),
                            ProxyUserID = obj["ChildProxyID"].ToString(),
                            Time = item.CreateDateTime
                        });
                    }
                }
            }
            var model = list.GroupBy(s => s.ProxyUserID).Select(s => new CommissionViewModel()
            {
                ProxyUserID = s.Key,
                Amount = s.Sum(x => x.Amount),
            });
            return View(model);
        }

        [Authorize(Roles = SysRole.CommissionManageRead)]
        public ActionResult Details(string proxyUserID, int page = 1)
        {
            Sidebar();
            var orders = db.Orders.Include(s => s.Details)
                .Where(s => s.ProxyID == proxyUserID || s.ChildProxyID == proxyUserID);
            var orderIds = orders.Select(s => s.ID);
            var orderLogs = db.OrderLogs.Where(s => s.Type == Enums.OrderLogType.Pay
                && orderIds.Contains(s.OrderID));
            var list = new List<CommissionViewModel>();
            foreach (var item in orderLogs)
            {
                if (!string.IsNullOrWhiteSpace(item.ExData))
                {
                    var obj = (JObject)JsonConvert.DeserializeObject(item.ExData);
                    list.Add(new CommissionViewModel()
                    {
                        Amount = decimal.Parse(obj["ProxyAmount"].ToString()),
                        ProxyUserID = obj["ProxyID"].ToString(),
                        Time = item.CreateDateTime
                    });
                    if (!string.IsNullOrWhiteSpace(obj["ProxyID"].ToString()))
                    {
                        list.Add(new CommissionViewModel()
                        {
                            Amount = decimal.Parse(obj["ChildProxyAmount"].ToString()),
                            ProxyUserID = obj["ChildProxyID"].ToString(),
                            Time = item.CreateDateTime
                        });
                    }
                }
            }
            return View(list.AsQueryable().OrderBy(s => s.Time).ToPagedList(page));
        }
    }
}