using System;

namespace Turnit.GenericStore.Api.Features.Sales;

public class ProductModel
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

public class ProductCategoryModel
{
    public Guid? CategoryId { get; set; }

    public ProductModel[] Products { get; set; }
}