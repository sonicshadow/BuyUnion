using BuyUnion.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

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
            var productProxys = db.ProductProxys.ToList();
            var ppids = productProxys.Select(s => s.ProxyUserID).Distinct();
            var orders = db.Orders.Include(s => s.Details)
                .Where(s => s.State == Enums.OrderState.Cancel && ppids.Contains(s.ProxyID));
            var counts = orders.GroupBy(s => s.ProxyID).Select(s => new { ProxyID = s.Key, List = s.Select(x => x.ID).ToList(), Count = s.Count() }).ToList();

            //查询产品
            var details = orders.Select(s => s.Details);
            var pids = new List<int>();
            foreach (var item in details)
            {
                pids.AddRange(item.Select(s => s.ProductID));
            }
            pids.Distinct();
            var products = db.Products.Where(s => pids.Contains(s.ID)).ToList();
            //查询产品

            var list = new List<CommissionViewModel>();

            //代理佣金
            var proxyUsers = orders.GroupBy(s => s.ProxyID)
                .Select(s => new { ProxyID = s.Key, List = s.ToList() });
            foreach (var item in proxyUsers)
            {
                var productProxy = productProxys.FirstOrDefault(s => s.ProxyUserID == item.ProxyID);
                var count = counts.FirstOrDefault(s => s.ProxyID == item.ProxyID);
                var orderDetails = item.List.Select(s => s.Details);
                //计算佣金
                List<decimal> amount = new List<decimal>();
                foreach (var detailItem in orderDetails)
                {
                    amount.AddRange(detailItem.Select(s =>
                    {
                        var product = products.FirstOrDefault(x => x.ID == s.ProductID);
                        var commission = count.List.IndexOf(s.OrderID) + 1 <= productProxy.Max ? 1 : decimal.Multiply(product.Commission, (decimal)0.7);
                        return decimal.Multiply((s.Count * s.Price), commission);
                    }).ToList());
                }
                list.Add(new CommissionViewModel()
                {
                    ProxyUserID = item.ProxyID,
                    Amount = amount.Sum(),
                });
            }
            //代理佣金

            //二级代理佣金
            var childUsers = orders.Where(s => s.ChildProxyID != "" && s.ChildProxyID != null)
                .GroupBy(s => s.ChildProxyID)
               .Select(s => new { ChildProxyID = s.Key, List = s.ToList() });
            foreach (var item in childUsers)
            {
                var orderDetails = item.List.Select(s => s.Details);
                //计算佣金
                List<decimal> amount = new List<decimal>();
                foreach (var detailItem in orderDetails)
                {
                    amount.AddRange(detailItem.Select(s =>
                    {
                        var product = products.FirstOrDefault(x => s.ID == s.ProductID);
                        var gailv = decimal.Multiply(product.Commission, (decimal)0.3);
                        return decimal.Multiply((s.Count * s.Price), gailv);
                    }).ToList());
                }
                list.Add(new CommissionViewModel()
                {
                    ProxyUserID = item.ChildProxyID,
                    Amount = amount.Sum(),
                });
            }
            //二级代理佣金

            return View(list);
        }

        [Authorize(Roles = SysRole.CommissionManageRead)]
        public ActionResult Details(string proxyUserID, int page = 1)
        {
            Sidebar();
            var orders = db.Orders.Include(s => s.Details)
               .Where(s => s.State == Enums.OrderState.Cancel &&
                (s.ProxyID == proxyUserID || s.ChildProxyID == proxyUserID))
                .OrderBy(s => s.UpdateDateTime).ToPagedList(page);
            ViewBag.Paged = orders;
            //查询产品
            var details = orders.Select(s => s.Details);
            var pids = new List<int>();
            foreach (var item in details)
            {
                pids.AddRange(item.Select(s => s.ProductID));
            }
            pids.Distinct();
            var products = db.Products.Where(s => pids.Contains(s.ID)).ToList();
            //查询产品

            var list = new List<CommissionViewModel>();

            if (orders.Any(s => s.ChildProxyID == proxyUserID))
            {
                //二级代理佣金
                foreach (var item in orders)
                {
                    list.AddRange(item.Details.Select(s =>
                    {
                        var product = products.FirstOrDefault(x => s.ID == s.ProductID);
                        var gailv = decimal.Multiply(product.Commission, (decimal)0.3);
                        decimal amount = decimal.Multiply((s.Count * s.Price), gailv);
                        var viewmodel = new CommissionViewModel()
                        {
                            ProxyUserID = proxyUserID,
                            Time = item.UpdateDateTime,
                            Amount = amount,
                        };
                        return viewmodel;
                    }).ToList());
                }
            }
            else
            {
                //代理佣金
                var productProxy = db.ProductProxys.FirstOrDefault(s => s.ProxyUserID == proxyUserID);
                foreach (var item in orders)
                {
                    list.AddRange(item.Details.Select(s =>
                    {
                        var product = products.FirstOrDefault(x => s.ID == s.ProductID);
                        var commission = orders.ToList().IndexOf(item) + 1 <= productProxy.Max ? 1 : decimal.Multiply(product.Commission, (decimal)0.7);
                        decimal amount = decimal.Multiply((s.Count * s.Price), commission);
                        var viewmodel = new CommissionViewModel()
                        {
                            ProxyUserID = proxyUserID,
                            Time = item.UpdateDateTime,
                            Amount = amount,
                        };
                        return viewmodel;
                    }).ToList());
                }
            }
            return View(list);
        }
    }
}