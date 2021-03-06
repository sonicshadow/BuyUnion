﻿using BuyUnion.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Index(int page = 1)
        {
            Sidebar();
            var proxyAmountLogs = db.ProxyAmountLogs.Where(s => s.Type != Enums.AmountLogType.Withdraw && s.ProxyID != "").ToList()
                .GroupBy(s => s.ProxyID)
                .Select(s => new ProxyAmountLog()
                {
                    Amount = s.Sum(x => x.Amount),
                    ProxyID = s.Key,
                    Type = Enums.AmountLogType.Income
                }).AsQueryable().OrderBy(s => s.ProxyID).ToPagedList(page);
            return View(proxyAmountLogs);
        }

        [Authorize(Roles = SysRole.CommissionManageRead)]
        public ActionResult Details(string ProxyID, int page = 1)
        {
            Sidebar();
            var proxyAmountLogs = db.ProxyAmountLogs
                .Where(s => s.Type != Enums.AmountLogType.Withdraw && s.ProxyID == ProxyID)
                .OrderByDescending(s => s.CreateDateTime)
                .ToPagedList(page);
            return View(proxyAmountLogs);
        }
    }
}