﻿namespace OnlineQRMenuApp.Models.ViewModel
{
    public class OrderViewModel
    {
        public string Code { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Table { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class OrderListViewModel
    {
        public string Date { get; set; }
        public List<OrderViewModel> Children { get; set; }
    }

    public class OrderListDataViewModel
    {
        public int CountAll { get; set; }
        public int CountPENDING { get; set; }
        public int CountCONFIRMED { get; set; }
        public int CountPROCESSING { get; set; }
        public int CountPROCESSED { get; set; }
        public int CountCOMPLETED { get; set; }
        public int CountCANCELLED { get; set; }
        public List<OrderListViewModel> OrderList { get; set; }
    }
}
