using System;

namespace Turnit.GenericStore.Api.ApiModels.Sales.Products.AllProducts.Out
{
    public class ProductCategoryModel
    {
        public Guid? CategoryId { get; set; }

        public ProductModel[] Products { get; set; }
    }
}