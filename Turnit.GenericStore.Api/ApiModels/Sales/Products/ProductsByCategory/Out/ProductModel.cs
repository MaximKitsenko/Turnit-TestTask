using System;

namespace Turnit.GenericStore.Api.ApiModels.Sales.Products.ProductsByCategory.Out
{
    public class ProductByCategoryModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AvailabilityModel[] Availability { get; set; }
    
        public class AvailabilityModel
        {
            public Guid StoreId { get; set; }
        
            public int Availability { get; set; }
        }
    }
}