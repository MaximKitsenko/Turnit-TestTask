using System;

namespace Turnit.GenericStore.Api.ApiModels.Sales.Products.AllProducts.Out
{
    public class ProductModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AvailabilityModel[] Availability { get; set; }
    
        public class AvailabilityModel
        {
            public Guid StoreId { get; set; }
        
            public int Availability { get; set; }
            public static AvailabilityModel From(Turnit.GenericStore.Api.Entities.ProductAvailability pa)
            {
                return new AvailabilityModel()
                {
                    Availability = pa.Availability,
                    StoreId = pa.Store.Id
                };
            }
        }

    }
}