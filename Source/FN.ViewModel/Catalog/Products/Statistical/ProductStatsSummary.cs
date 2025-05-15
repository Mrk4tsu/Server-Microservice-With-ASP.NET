namespace FN.ViewModel.Catalog.Products.Statistical
{
    public class ProductStatsViewModel 
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public ProductStatsSummary Stats { get; set; } = new();
    }

    public class ProductStatsSummary
    {
        public int TodayViews { get; set; }
        public int Last3DaysViews { get; set; }
        public int Last7DaysViews { get; set; }
        public int Last30DaysViews { get; set; }
    }
}
