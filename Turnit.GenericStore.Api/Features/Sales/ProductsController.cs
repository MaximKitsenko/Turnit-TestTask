using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Features.Sales;

[Route("products")]
public class ProductsController : ApiControllerBase
{
    private readonly ISession _session;

    public ProductsController(ISession session)
    {
        _session = session;
    }
    
    [HttpGet, Route("by-category/{categoryId:guid}")]
    public async Task<ProductModel[]> ProductsByCategory(Guid categoryId)
    {
        var products = await _session.QueryOver<ProductCategory>()
            .Where(x => x.Category.Id == categoryId)
            .Select(x => x.Product)
            .ListAsync<Product>();

        var result = new List<ProductModel>();

        foreach (var product in products)
        {
            var availability = await _session.QueryOver<ProductAvailability>()
                .Where(x => x.Product.Id == product.Id)
                .ListAsync();
            
            var model = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Availability = availability.Select(x => new ProductModel.AvailabilityModel
                {
                    StoreId = x.Store.Id,
                    Availability = x.Availability
                }).ToArray()
            };
            result.Add(model);
        }
        
        return result.ToArray();
    }
    
    [HttpGet, Route("")]
    public async Task<ProductCategoryModel[]> AllProducts()
    {
        var products = await _session.QueryOver<Product>().ListAsync<Product>();
        var productCategories = await _session.QueryOver<ProductCategory>().ListAsync();

        var productModels = new List<ProductModel>();
        foreach (var product in products)
        {
            var availability = await _session.QueryOver<ProductAvailability>()
                .Where(x => x.Product.Id == product.Id)
                .ListAsync();
            
            var model = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Availability = availability.Select(x => new ProductModel.AvailabilityModel
                {
                    StoreId = x.Store.Id,
                    Availability = x.Availability
                }).ToArray()
            };
            productModels.Add(model);
        }
        
        var result = new List<ProductCategoryModel>();
        foreach (var category in productCategories.GroupBy(x => x.Category.Id))
        {
            var productIds = category.Select(x => x.Product.Id).ToHashSet();
            result.Add(new ProductCategoryModel
            {
                CategoryId = category.Key,
                Products = productModels
                    .Where(x => productIds.Contains(x.Id))
                    .ToArray()
            });
        }

        var uncategorizedProducts = productModels.Except(result.SelectMany(x => x.Products));
        if (uncategorizedProducts.Any())
        {
            result.Add(new ProductCategoryModel
            {
                Products = uncategorizedProducts.ToArray()
            });
        }
        
        return result.ToArray();
    }
    
    [HttpPut, Route("{productId:guid}/category/{categoryId:guid}")]
    public async Task<OkResult> PutProduct(Guid categoryId,Guid productId)
    {
        void Validate(Task<Product> getProduct, Task<Category> getCategory, Task<ProductCategory> getProductCategory)
        {
            var errorMsg = "";
            if (getProduct.Result == null)
            {
                errorMsg += "Product Doesn't exist. ";
            }

            if (getCategory.Result == null)
            {
                errorMsg += "Category Doesn't exist. ";
            }

            if (getProductCategory.Result != null)
            {
                errorMsg += "Product already added to category. ";
            }

            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                throw new Exception(errorMsg);
            }
        }

        var productTask = _session.GetAsync<Product>(productId);
        var categoryTask = _session.GetAsync<Category>(categoryId);
        var productCategoryTask = _session
            .QueryOver<ProductCategory>()
            .Where(x => x.Product.Id == productId && x.Category.Id == categoryId).SingleOrDefaultAsync();

        await Task.WhenAll(productTask, categoryTask,productCategoryTask);

        Validate(productTask, categoryTask, productCategoryTask);

        var productCategory = new ProductCategory()
        {
            Category = categoryTask.Result,
            Product = productTask.Result
        };
        await _session.PersistAsync(productCategory);
        
        return Ok();
    }
}