using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShipmentApi.Models
{
    public enum Types
    {
        Parcel,
        Letter
    }
    public class Bag
    {
        public int BagId { get; set; }
        public string BagNo { get; set; }
        public Types Type { get; set; }
        public int? LetterCount { get; set; }
        public double? Weight { get; set; }
        public double? Price { get; set; }
        public ICollection<Parcel> Parcels { get; set; }
        public bool Finalized { get; set; }
    }

}
