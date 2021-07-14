using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShipmentApi.Models
{
    public class Parcel
    {
        public int ParcelId { get; set; }
        public string ParcelNo { get; set; }
        public string RecipientName { get; set; }
        public string DestinationCountry { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }

    }
}
