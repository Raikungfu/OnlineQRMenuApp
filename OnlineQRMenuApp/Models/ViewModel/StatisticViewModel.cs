using System.Collections.Generic;

namespace OnlineQRMenuApp.Models.ViewModel
{
    public class DataPoint
    {
        public string x { get; set; }
        public decimal y { get; set; }
    }

    public class ChartData
    {
        public List<DataStatistic> DataStatistic { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class DataStatistic
    {
        public string id { get; set; }
        public List<DataPoint> data { get; set; }
    }

    public class DailyRevenue
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }



    public class MonthlyRevenue
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
    }


    public class YearlyRevenue
    {
        public int Year { get; set; }
        public decimal Revenue { get; set; }
    }

}
