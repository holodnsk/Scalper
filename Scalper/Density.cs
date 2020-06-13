using System.Collections.Generic;
using AD.Common.DataStructures;
using DevExpress.Mvvm.Native;

namespace Scalper
{
    public class Density
    {
        public Density(decimal price, decimal value)
        {
            Price = price;
            Values = new List<decimal> {value};
        }

        private decimal Price;
        private List<decimal> Values;
        private decimal MaxValue;
        private OrderDirection Direction;
        
        
        
        public override bool Equals(object density)
        {
            if ((density == null) || ! this.GetType().Equals(density.GetType()))
                return false;
            
            Density density1 = density as Density;
            
            return Price==density1.Price && Direction==density1.Direction;
        }

        public override int GetHashCode()
        {
            return Price.GetHashCode();
        }
    }
}