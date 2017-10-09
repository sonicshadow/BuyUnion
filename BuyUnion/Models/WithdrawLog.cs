using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class WithdrawLog
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public decimal Amount { get; set; }

        public string Remark { get; set; }

        public Enums.WithdrawState State { get; set; }
    }
}