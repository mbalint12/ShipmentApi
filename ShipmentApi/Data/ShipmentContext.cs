using Microsoft.EntityFrameworkCore;
using ShipmentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipmentApi.Data
{
    public class ShipmentContext : DbContext
    {
        public ShipmentContext(DbContextOptions<ShipmentContext> options) : base(options)
        {
        }

        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Bag> Bags { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

    }
}
