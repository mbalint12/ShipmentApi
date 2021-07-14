using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipmentApi.Models;

namespace ShipmentApi.Data
{
    public class DbInitializer
    {
        public static void Initialize(ShipmentContext context)
        {
            context.Database.EnsureCreated();
            if (context.Shipments.Any())
            {
                return;
            }
            var parcel_1 = new Parcel { ParcelNo = "AA000001XX", RecipientName = "John Smith", DestinationCountry = "UK", Weight = 10.5, Price = 20.0 };
            var parcel_2 = new Parcel { ParcelNo = "AA000002YY", RecipientName = "Elisha Cline", DestinationCountry = "DE", Weight = 0.592, Price = 3.55 };
            var parcel_3 = new Parcel { ParcelNo = "AA000003ZZ", RecipientName = "Ben Ramsay", DestinationCountry = "AU", Weight = 3.45, Price = 117.2 };
            var bag_1 = new Bag { BagNo = "B0001", Type = Types.Letter, LetterCount = 5, Weight = 0.5, Price = 10.0 };
            var bag_2 = new Bag { BagNo = "B0002", Type = Types.Letter, LetterCount = 1, Weight = 0.105, Price = 0.9 };
            var bag_3 = new Bag { BagNo = "B0003", Type = Types.Letter, LetterCount = 3, Weight = 1.95, Price = 2.59 };
            var bag_4 = new Bag { BagNo = "B0004", Type = Types.Letter, LetterCount = 11, Weight = 10.3, Price = 15.73 };
            var bag_5 = new Bag { BagNo = "B0005", Type = Types.Letter, LetterCount = 20, Weight = 22.22, Price = 90.0 };
            var bag_6 = new Bag { BagNo = "B0006", Type = Types.Letter, LetterCount = 2, Weight = 0.57, Price = 4.7 };
            var bag_7 = new Bag { BagNo = "B0007", Type = Types.Parcel, Parcels = new List<Parcel> { parcel_1 } };
            var bag_8 = new Bag { BagNo = "B0008", Type = Types.Parcel, Parcels = new List<Parcel> { parcel_2, parcel_3 } };
            var bag_9 = new Bag { BagNo = "B0009", Type = Types.Parcel };
            var shipment_1 = new Shipment
            {
                ShipmentNo = "AAA-000001",
                Airport = Shipment.Airports.HEL,
                FlightNo = "FF0001",
                FlightDate = DateTime.Today.AddDays(1),
                Bags = new List<Bag> { bag_1, bag_7 },
                Finalized = true
            };
            var shipment_2 = new Shipment
            {
                ShipmentNo = "AAA-000002",
                Airport = Shipment.Airports.RIX,
                FlightNo = "FF0002",
                FlightDate = DateTime.Today.AddDays(-7),
                Bags = new List<Bag> { bag_2, bag_3 }
            };
            var shipment_3 = new Shipment
            {
                ShipmentNo = "AAA-000003",
                Airport = Shipment.Airports.HEL,
                FlightNo = "FF0003",
                FlightDate = DateTime.Today.AddDays(7),
                Bags = new List<Bag> { bag_4, bag_5, bag_8 }
            };
            var shipment_4 = new Shipment
            {
                ShipmentNo = "AAA-000004",
                Airport = Shipment.Airports.TLL,
                FlightNo = "FF0004",
                FlightDate = DateTime.Today.AddDays(11)
            };
            var shipment_5 = new Shipment
            {
                ShipmentNo = "AAA-000005",
                Airport = Shipment.Airports.RIX,
                FlightNo = "FF0005",
                FlightDate = DateTime.Today.AddDays(7),
                Bags = new List<Bag> { bag_6, bag_9 }
            };

            context.Add(shipment_1);
            context.Add(shipment_2);
            context.Add(shipment_3);
            context.Add(shipment_4);
            context.Add(shipment_5);

            context.SaveChanges();
        }
    }
}
