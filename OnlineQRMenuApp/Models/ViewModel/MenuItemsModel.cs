namespace OnlineQRMenuApp.Models.ViewModel
{
    public class MenuItemsModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public int CoffeeShopId { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public List<CustomizationGroup> CustomizationGroups { get; set; }
    }

    public class CustomizationModelGroup
    {
        public int CustomizationGroupId { get; set; }
        public string Name { get; set; }
        public List<MenuItemCustomizationModel> Customizations { get; set; }
    }

    public class MenuItemCustomizationModel
    {
        public int MenuItemCustomizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal AdditionalCost { get; set; }
        public bool IsSelected { get; set; }
    }

}
