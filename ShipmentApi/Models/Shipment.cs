using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShipmentApi.Models
{
    public class Shipment
    {
        public enum Airports
        {
            TLL,
            RIX,
            HEL
        }
        public int ShipmentId { get; set; }
        public string ShipmentNo { get; set; }
        public Airports Airport { get; set; }
        public string FlightNo { get; set; }
        public DateTime FlightDate { get; set; }
        public ICollection<Bag> Bags { get; set; }
        public bool Finalized { get; set; }
    }
}
