using ShipmentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShipmentApi.Data
{
    public static class EnumExtension
    {
        public static List<EnumValue> GetValues<T>()
        {
            List<EnumValue> values = new List<EnumValue>();
            foreach (var itemType in Enum.GetValues(typeof(T)))
            {
                values.Add(new EnumValue()
                {
                    Name = Enum.GetName(typeof(T), itemType),
                    Value = (int)itemType
                });
            }
            return values;
        }
    }
}
