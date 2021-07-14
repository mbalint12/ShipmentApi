using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShipmentApi.Models
{
    public class BagWithShipmentId
    {
        public Bag Bag { get; set; }
        public int ShipmentId { get; set; }
        public BagWithShipmentId(Bag bag, int shipmentid)
        {
            Bag = bag;
            ShipmentId = shipmentid;
        }
    }
}
