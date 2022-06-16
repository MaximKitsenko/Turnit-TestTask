using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using Turnit.GenericStore.Api.ApiModels.Sales.Categories.AllCategories.Out;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Features.Sales
{
    [Route("categories")]
    public class CategoriesController : ApiControllerBase
    {
        private readonly ISession _session;

        public CategoriesController(ISession session)
        {
            _session = session;
        }
    
        [HttpGet, Route("")]
        public async Task<CategoryModelOut[]> AllCategories()
        {
            var categories = await _session.QueryOver<Category>().ListAsync();

            var result = categories
                .Select(x => new CategoryModelOut
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArray();
        
            return result;
        }
    }
}