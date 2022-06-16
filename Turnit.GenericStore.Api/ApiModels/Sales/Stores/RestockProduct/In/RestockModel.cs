using System;
using System.Collections.Generic;

namespace Turnit.GenericStore.Api.ApiModels.Sales.Stores.RestockProduct.In
{
    public class RestockModel
    {
        public Dictionary<Guid, int> Products { get; set; }
    }
}