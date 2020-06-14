using System;
using System.Collections.Generic;
using Ecng.Reflection;
using StockSharp.BusinessEntities;

namespace Scalper
{
    public class DensitiesContainer
    {
        public DensitiesContainer()
        {
            Densities = new Dictionary<decimal, Density>();
        }

        private Dictionary<decimal,Density> Densities;
        private const Decimal SignificantVolume = 2000; // TODO config this

        public void HandleValue(Quote quote)
        {
            var IsShouldRenewMaxVolume = Densities.ContainsKey(quote.Price) && Densities[quote.Price].MaxVolume < quote.Volume;
            var isDensityBeingTooSmall = Densities.ContainsKey(quote.Price) && (quote.Volume<Densities[quote.Price].MaxVolume/3 && quote.Volume<SignificantVolume);
            var IsShouldRenewVolumeOfDensity = Densities.ContainsKey(quote.Price) && !(quote.Volume<Densities[quote.Price].MaxVolume/3 && quote.Volume<SignificantVolume);
            var IsNewDensity = quote.Volume>=SignificantVolume && !Densities.ContainsKey(quote.Price);
            
            if (IsShouldRenewMaxVolume)
                Densities[quote.Price].MaxVolume = quote.Volume;

            if (isDensityBeingTooSmall)
            {
                Densities.Remove(quote.Price);
                // TODO event for opened position and order if have for action                        
            }

            if (IsShouldRenewVolumeOfDensity)
                Densities[quote.Price].Volume = quote.Volume;

            if (IsNewDensity)
                Densities.Add(quote.Price,new Density(quote.Price,quote.Volume,quote.Volume,quote.OrderDirection));
        }
    }
}