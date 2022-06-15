using System;
using FluentNHibernate.Mapping;

namespace Turnit.GenericStore.Api.Entities;

public class ProductAvailability
{
    public virtual Guid Id { get; set; }

    public virtual Product Product { get; set; }

    public virtual Store Store { get; set; }

    public virtual int Availability { get; set; }
}

public class ProductAvailabilityMap : ClassMap<ProductAvailability>
{
    public ProductAvailabilityMap()
    {
        Schema("public");
        Table("product_availability");

        Id(x => x.Id, "id");
        Map(x => x.Availability, "availability");
        References(x => x.Store, "store_id");
        References(x => x.Product, "product_id");
    }
}