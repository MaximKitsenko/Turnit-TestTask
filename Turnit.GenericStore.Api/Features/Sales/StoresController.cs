using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using Turnit.GenericStore.Api.ApiModels.Sales.Stores.RestockProduct.In;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Features.Sales
{
    [Route("store")]
    public class StoreController : ApiControllerBase
    {
        private readonly ISession _session;

        public StoreController(ISession session)
        {
            _session = session;
        }

        [HttpPost, Route("{storeId:guid}/restock")]
        public async Task<ActionResult> RestockProduct([FromRoute] Guid storeId, [FromBody] RestockModel restockModel)
        {
            List<string> Validate(Store store, IList<Product> products, RestockModel model)
            {
                var errorMsg = new List<string>();
                if (store == null)
                {
                    errorMsg.Add("Store not found");
                }

                if (products == null || !products.Any())
                {
                    errorMsg.Add("Products not found");
                }

                errorMsg.AddRange(
                    from product in model.Products
                    where product.Value < 0
                    select $"Product {product.Key} has qty {product.Value}, negative deprecated. "
                );


                return errorMsg;
            }

            using (var tx = _session.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                var getStoreTask = _session
                    .GetAsync<Store>(storeId);

                var productsIds = restockModel.Products.Select(x => x.Key).ToList();
                var getProductsTask = _session.QueryOver<Product>()
                    .WhereRestrictionOn(x=>x.Id)
                    .IsIn(productsIds)
                    .ListAsync<Product>();

                await Task.WhenAll(getStoreTask, getProductsTask);

                var validationRes = Validate(getStoreTask.Result, getProductsTask.Result, restockModel);
                if (validationRes.Count > 0)
                {
                    var errMsg = string.Join("; ", validationRes);
                    return BadRequest(errMsg);
                }

                var productsToRestock = getProductsTask.Result
                    .Join(
                        restockModel.Products,
                        x => x.Id,
                        z => z.Key,
                        (p, kv) => (product: p, qty: kv.Value))
                    .ToList();

                for (var i = 0; i < productsToRestock.Count; i++)
                {
                    await _session.PersistAsync(new ProductAvailability()
                    {
                        Availability = productsToRestock[i].qty,
                        Product = productsToRestock[i].product,
                        Store = getStoreTask.Result
                    });

                    // https://nhibernate.info/doc/nhibernate-reference/batch.html
                    if (i % 20 == 0)
                    {
                        // flush a batch of inserts and release memory:
                        await _session.FlushAsync();
                        _session.Clear();
                    }
                }

                await tx.CommitAsync();
            }

            return Ok();
        }
    }
}
