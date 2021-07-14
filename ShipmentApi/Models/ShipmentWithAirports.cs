using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShipmentApi.Models
{
    public class ShipmentWithAirports
    {
        public List<Shipment> Shipment { get; set; }
        public List<EnumValue> Airports { get; set; }
        public ShipmentWithAirports(List<Shipment> shipment, List<EnumValue> airports)
        {
            Shipment = shipment;
            Airports = airports;
        }
    }
}
