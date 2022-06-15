using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
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
    public async Task<ActionResult> PutProduct(Guid categoryId, Guid productId)
    {
        List<string> Validate(Task<Product> getProduct, Task<Category> getCategory, Task<ProductCategory> getProductCategory)
        {
            var errorMsg = new List<string>();
            if (getProduct.Result == null)
            {
                errorMsg.Add("Product Doesn't exist. "); 
            }

            if (getCategory.Result == null)
            {
                errorMsg.Add("Category Doesn't exist. ");
            }

            if (getProductCategory.Result != null)
            {
                errorMsg.Add("Product already added to category. ");
            }

            return errorMsg;
        }

        var productTask = _session.GetAsync<Product>(productId);
        var categoryTask = _session.GetAsync<Category>(categoryId);
        var productCategoryTask = _session
            .QueryOver<ProductCategory>()
            .Where(x => x.Product.Id == productId && x.Category.Id == categoryId).SingleOrDefaultAsync();

        await Task.WhenAll(productTask, categoryTask,productCategoryTask);

        var validationRes = Validate(productTask, categoryTask, productCategoryTask);
        if (validationRes.Count > 0)
        {
            var errMsg = string.Join(";",validationRes);
            return BadRequest(errMsg);
        }

        var productCategory = new ProductCategory()
        {
            Category = categoryTask.Result,
            Product = productTask.Result
        };
        await _session.PersistAsync(productCategory);
        await _session.FlushAsync();
        
        return Ok();
    }
    
    [HttpDelete, Route("{productId:guid}/category/{categoryId:guid}")]
    public async Task<IActionResult > DeleteProduct(Guid categoryId, Guid productId)
    {
        List<string> Validate(Task<Product> getProduct, Task<Category> getCategory, Task<ProductCategory> getProductCategory)
        {
            var errorMsg = new List<string>();
            if (getProduct.Result == null)
            {
                errorMsg.Add("Product Doesn't exist. ");
            }

            if (getCategory.Result == null)
            {
                errorMsg.Add("Category Doesn't exist. ");
            }

            if (getProductCategory.Result == null)
            {
                errorMsg.Add( "Product is not in the category. ");
            }

            return errorMsg;
        }

        var productTask = _session.GetAsync<Product>(productId);
        var categoryTask = _session.GetAsync<Category>(categoryId);
        var productCategoryTask = _session
            .QueryOver<ProductCategory>()
            .Where(x => x.Product.Id == productId && x.Category.Id == categoryId).SingleOrDefaultAsync();

        await Task.WhenAll(productTask, categoryTask,productCategoryTask);

        var validationRes = Validate(productTask, categoryTask, productCategoryTask);

        if (validationRes.Count > 0)
        {
            var errMsg = string.Join(";",validationRes);
            return BadRequest(errMsg);
        }
        
        await _session.DeleteAsync(productCategoryTask.Result);
        await _session.FlushAsync();
        
        return Ok();
    }
    
    [HttpPost, Route("{productId:guid}/book")]
    public async Task<ActionResult> BookProduct([FromRoute]Guid productId, [FromBody] BookModel bookModelInfo)
    {
        List<string> Validate(IList<ProductAvailability> getProduct, int bookQty)
        {
            var errorMsg = new List<string>();
            if (bookQty <1)
            {
                errorMsg.Add("We can book only 1 or more. ");
                return errorMsg;
            }

            if (getProduct != null && getProduct.Any())
            {
                var sum = getProduct.Sum(x => x.Availability);
                if (sum < bookQty)
                {
                    errorMsg.Add($"There are {sum} products but, you are trying to book {bookQty}"); 
                }
            }
            else
            {
                errorMsg.Add("Product Doesn't exist in stores. "); 
            }
            

            return errorMsg;
        }

        var tr = _session.BeginTransaction(IsolationLevel.RepeatableRead);
        
        var productTask = _session
            .QueryOver<ProductAvailability>()
            .Where(x => x.Product.Id == productId)
            .ListAsync<ProductAvailability>();
        
        await Task.WhenAll(productTask);

        var validationRes = Validate(productTask.Result, bookModelInfo.Qty);
        if (validationRes.Count > 0)
        {
            var errMsg = string.Join(";",validationRes);
            return BadRequest(errMsg);
        }

        var bookQty = bookModelInfo.Qty;
        for (var i = 0; i < productTask.Result.Count && bookQty > 0; i++)
        {
            if (productTask.Result[i].Availability > bookQty)
            {
                productTask.Result[i].Availability -= bookQty;
                await _session.SaveOrUpdateAsync(productTask.Result[i]);
                bookQty = 0;
                break;
            }
            else
            {
                bookQty -= productTask.Result[i].Availability;
                productTask.Result[i].Availability = 0;
                await _session.SaveOrUpdateAsync(productTask.Result[i]);
            }
        }

        await _session.FlushAsync();
        await tr.CommitAsync();
        return Ok();
    }
}