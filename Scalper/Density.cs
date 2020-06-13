using System.Collections.Generic;
using AD.Common.DataStructures;
using DevExpress.Mvvm.Native;

namespace Scalper
{
    public class Density
    {
        public Density(decimal price, decimal volume, decimal maxVolume)
        {
            Price = price;
            Volume = volume;
            MaxVolume = maxVolume;
        }

        private OrderDirection Direction;
        public decimal Price { get; }
        public decimal Volume { get; set; }
        public decimal MaxVolume { get; set; }


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