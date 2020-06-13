using System.Collections.Generic;
using AD.Common.DataStructures;
using DevExpress.Mvvm.Native;

namespace Scalper
{
    public class Density
    {
        public decimal Price { get; set; }
        public List<decimal> Values { get; set; }
        public decimal MaxValue { get; set; }
        
        public OrderDirection Direction { get; set; }
        
        public bool Equals(Density density)
        {
            if (density == null)
                return false;
            return Price==density.Price && Direction==density.Direction;
        }

        public int GetHashCode()
        {
            return Price.GetHashCode()+Values.GetHashCode()+MaxValue.GetHashCode();
        }
    }
}