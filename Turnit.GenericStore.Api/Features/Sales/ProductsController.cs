using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using Turnit.GenericStore.Api.ApiModels.Sales.Products;
using Turnit.GenericStore.Api.ApiModels.Sales.Products.AllProducts.Out;
using Turnit.GenericStore.Api.ApiModels.Sales.Products.BookProduct.In;
using Turnit.GenericStore.Api.ApiModels.Sales.Products.ProductsByCategory.Out;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Features.Sales
{
    [Route("products")]
    public class ProductsController : ApiControllerBase
    {
        private readonly ISession _session;

        public ProductsController(ISession session)
        {
            _session = session;
        }
    
        [HttpGet, Route("by-category/{categoryId:guid}")]
        public async Task<ProductByCategoryModel[]> ProductsByCategory(Guid categoryId)
        {
            var products = await _session.QueryOver<ProductCategory>()
                .Where(x => x.Category.Id == categoryId)
                .Select(x => x.Product)
                .ListAsync<Product>();

            var result = new List<ProductByCategoryModel>();

            var productsIds = products.Select(t=>t.Id).ToList();    
            var availabilityRaw = await _session.QueryOver<ProductAvailability>()
                .WhereRestrictionOn(x=>x.Id)
                .IsIn(productsIds)
                    .ListAsync();

            
            // todo: can be improved with join. Example below in AllProducts. Have no time to fixe it 
            foreach (var product in products)
            {
                var model = new ProductByCategoryModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Availability = availabilityRaw
                        .Where(x => x.Product.Id == product.Id)
                        .Select(x => new ProductByCategoryModel.AvailabilityModel
                    {
                        StoreId = x.Store.Id,
                        Availability = x.Availability
                    }).ToArray()
                };
                result.Add(model);
            }
        
            return result.ToArray();
        }
    
        [HttpGet, Route("backup")]
        public async Task<ProductCategoryModel[]> AllProducts_Backup()
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
        
            return result.OrderBy(x=>x.CategoryId).ToArray();
        }

        [HttpPut, Route("{productId:guid}/category/{categoryId:guid}")]
        public async Task<ActionResult> PutProduct(Guid categoryId, Guid productId)
        {
            List<string> Validate(Product getProduct, Category getCategory,
                ProductCategory getProductCategory)
            {
                var errorMsg = new List<string>();
                if (getProduct == null)
                {
                    errorMsg.Add("Product Doesn't exist. ");
                }

                if (getCategory == null)
                {
                    errorMsg.Add("Category Doesn't exist. ");
                }

                if (getProductCategory != null)
                {
                    errorMsg.Add("Product already added to category. ");
                }

                return errorMsg;
            }

            using (var tx = _session.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                var product = await _session.GetAsync<Product>(productId);
                var category = await _session.GetAsync<Category>(categoryId);
                var productCategory = await _session
                    .QueryOver<ProductCategory>()
                    .Where(x => x.Product.Id == productId && x.Category.Id == categoryId).SingleOrDefaultAsync();


                var validationRes = Validate(product, category, productCategory);
                if (validationRes.Count > 0)
                {
                    var errMsg = string.Join(";", validationRes);
                    return BadRequest(errMsg);
                }

                var productCategoryB = new ProductCategory()
                {
                    Category = category,
                    Product = product
                };
                await _session.PersistAsync(productCategoryB);
                await tx.CommitAsync();
            }

            return Ok();
        }

        [HttpDelete, Route("{productId:guid}/category/{categoryId:guid}")]
        public async Task<IActionResult > DeleteProduct(Guid categoryId, Guid productId)
        {
            List<string> Validate(Product getProduct, Category getCategory, ProductCategory getProductCategory)
            {
                var errorMsg = new List<string>();
                if (getProduct == null)
                {
                    errorMsg.Add("Product Doesn't exist. ");
                }

                if (getCategory == null)
                {
                    errorMsg.Add("Category Doesn't exist. ");
                }

                if (getProductCategory == null)
                {
                    errorMsg.Add( "Product is not in the category. ");
                }

                return errorMsg;
            }
        
            using (var tx = _session.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                var product = await _session.GetAsync<Product>(productId);
                var category = await _session.GetAsync<Category>(categoryId);
                var productCategory = await _session
                    .QueryOver<ProductCategory>()
                    .Where(x => x.Product.Id == productId && x.Category.Id == categoryId).SingleOrDefaultAsync();


                var validationRes = Validate(product, category, productCategory);

                if (validationRes.Count > 0)
                {
                    var errMsg = string.Join(";",validationRes);
                    return BadRequest(errMsg);
                }
        
                await _session.DeleteAsync(productCategory);
                await tx.CommitAsync();
            }
        
            return Ok();
        }

        [HttpPost, Route("{productId:guid}/book")]
        public async Task<ActionResult> BookProduct([FromRoute]Guid productId, [FromBody] BookModelIn bookModelIn)
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

            using (var tx = _session.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                var products = await _session
                    .QueryOver<ProductAvailability>()
                    .Where(x => x.Product.Id == productId)
                    .ListAsync<ProductAvailability>();

                var validationRes = Validate(products, bookModelIn.Qty);
                if (validationRes.Count > 0)
                {
                    var errMsg = string.Join(";", validationRes);
                    return BadRequest(errMsg);
                }

                var bookQty = bookModelIn.Qty;
                for (var i = 0; i < products.Count && bookQty > 0; i++)
                {
                    if (products[i].Availability > bookQty)
                    {
                        products[i].Availability -= bookQty;
                        await _session.SaveOrUpdateAsync(products[i]);
                        bookQty = 0;
                        break;
                    }
                    else
                    {
                        bookQty -= products[i].Availability;
                        products[i].Availability = 0;
                        await _session.SaveOrUpdateAsync(products[i]);
                    }
                }

                await tx.CommitAsync();
            }

            return Ok();
        }

        [HttpGet, Route("")]
        public async Task<ProductCategoryModel[]> AllProducts()
        {
            IList<Product> products;
            IList<ProductCategory> productCategories;
            IList<ProductAvailability> productAvail;
            using (var tx = _session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                products = await _session.QueryOver<Product>().ListAsync<Product>();
                productCategories = await _session.QueryOver<ProductCategory>().ListAsync();
                productAvail = await _session.QueryOver<ProductAvailability>().ListAsync();
            }

            var productAvailabilityPairs = (
                from product in products
                join pa in productAvail
                    on product.Id equals pa.Product.Id into gj
                from subProd in gj.DefaultIfEmpty()
                select new
                {
                    product,
                    availability = subProd ?? null
                });
            var productAvailabilityList = productAvailabilityPairs
                .GroupBy(x => x.product.Id)
                .Select(x => new
                {
                    Product = x.FirstOrDefault()?.product,
                    Availability = x.Select(y => y.availability).Where(z => z != null).ToList()
                })
                .ToList();
            
            var productAvailabilityCategory =
                from p in productAvailabilityList
                join c in productCategories
                    on p.Product.Id equals c.Product.Id into pr
                from subPr in pr.DefaultIfEmpty()
                select new
                {
                    Product = p.Product,
                    Availability = p.Availability,
                    Category = subPr ?? null
                };
            
            var categoryProducts = productAvailabilityCategory
                .GroupBy(x => x?.Category?.Category.Id )
                .Select(y => new ProductCategoryModel
                {
                    CategoryId = y.Key,
                    Products = y.Select(z => new ProductModel()
                    {
                        Availability = z.Availability
                            .Select(k =>ProductModel.AvailabilityModel.From(k))
                            .ToArray(),
                        Id = z.Product.Id,
                        Name = z.Product.Name
                    }).ToArray()
                });
            var result = categoryProducts.OrderBy(x=>x.CategoryId).ToArray();
            
            return result;
        }
    }
}