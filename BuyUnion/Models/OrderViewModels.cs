using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace BuyUnion.Models
{
    [NotMapped]
    public class SubmitOrderViewModel : Order, IDistrict
    {
        public SubmitOrderViewModel() { }

        public SubmitOrderViewModel(Order order, List<Product> products)
        {
            ID = order.ID;
            Address = order.Address;
            Amount = order.Amount;
            State = order.State;
            Consignee = order.Consignee;
            Type = order.Type;
            PayType = order.PayType;
            PaidAmount = order.PaidAmount;
            PayCode = order.PayCode;
            Free = order.Free;
            Code = order.Code;
            PhoneNumber = order.PhoneNumber;
            CreateDateTime = order.CreateDateTime;
            UpdateDateTime = order.UpdateDateTime;
            Details = order.Details.Select(s =>
            {
                var image = products.FirstOrDefault(x => x.ID == s.ProductID).Image;
                var item = new OrderDetailViewModel(s, image);
                return item;
            }).ToList();
        }

        public string City { get; set; }

        public string District { get; set; }

        public string Province { get; set; }

        [Display(Name ="详情")]
        public new List<OrderDetailViewModel> Details { get; set; }
    }

    [NotMapped]
    public class OrderDetailViewModel : OrderDetail
    {
        public OrderDetailViewModel() { }

        public OrderDetailViewModel(OrderDetail detail, string image)
        {
            Count = detail.Count;
            ID = detail.ID;
            Image = image;
            Name = detail.Name;
            Price = detail.Price;
            ProductID = detail.ProductID;
        }

        public string Image { get; set; }

    }

}