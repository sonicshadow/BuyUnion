using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;
using Microsoft.AspNet.Identity;

namespace BuyUnion.Controllers
{
    [Authorize]
    public class WithdrawController : Controller
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
            ViewBag.Sidebar = "提现管理";
        }

        // GET: Withdraw
        public ActionResult Index(int page = 1)
        {
            Sidebar();
            var shopAmountLogs = db.ShopAmountLogs.OrderByDescending(s => s.CreateDateTime).ToPagedList(page);
            return View(shopAmountLogs);
        }

        public ActionResult ApplicationIndex(Enums.WithdrawState? state, int page = 1)
        {
            Sidebar();
            var w = db.WithdrawLogs.AsQueryable();
            if (state.HasValue)
            {
                w = w.Where(s => s.State == state);
            }
            var paged = w.OrderByDescending(s => s.ID).ToPagedList(page);
            return View(paged);
        }

        public ActionResult Application()
        {
            Sidebar();
            var withdrawLog = new WithdrawLog()
            {
                Amount = GetMaxAmount(),
                UserID = UserID,
            };
            return View(withdrawLog);
        }

        [HttpPost]
        public ActionResult Application(WithdrawLog model)
        {
            if (model.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "申请金额大于0");
            }
            var maxAmount = GetMaxAmount();
            if (model.Amount > maxAmount)
            {
                ModelState.AddModelError("Amount", $"申请金额小于或等于{maxAmount}");
            }
            if (string.IsNullOrWhiteSpace(model.PayNumber))
            {
                ModelState.AddModelError("PayNumber", "填写收款帐号");
            }
            if (ModelState.IsValid)
            {
                model.State = Enums.WithdrawState.NoCheck;
                db.WithdrawLogs.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Sidebar();
            return View(model);
        }

        public decimal GetMaxAmount()
        {
            var dateTime = DateTime.Now;
            var shopAmountLogs = db.ShopAmountLogs.Where(s => s.CreateDateTime.Month < dateTime.Month).ToList();
            var income = shopAmountLogs.Where(s => s.Type == Enums.AmountLogType.Income).Sum(s => s.Amount);
            var withdraw = shopAmountLogs.Where(s => s.Type == Enums.AmountLogType.Withdraw).Sum(s => s.Amount);
            return (income - withdraw);
        }

        public ActionResult Check(int id)
        {
            Sidebar();
            var withdrawLog = db.WithdrawLogs.FirstOrDefault(s => s.ID == id);
            return View(withdrawLog);
        }

        [HttpPost]
        public ActionResult Check(int id,bool result)
        {
            Sidebar();
            var withdrawLog = db.WithdrawLogs.FirstOrDefault(s => s.ID == id);
            withdrawLog.State = result ? Enums.WithdrawState.Pass : Enums.WithdrawState.NoPass;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}