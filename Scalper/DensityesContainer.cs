using System;
using System.Collections.Generic;
using StockSharp.BusinessEntities;

namespace Scalper
{
    public class DensitiesContainer
    {
        private readonly Dictionary<decimal,Density> _densities = new Dictionary<decimal, Density>();
        private const Decimal SignificantVolume = 2000; // TODO config this

        public void HandleValue(Quote quote)
        {
            var isShouldRenewMaxVolume = _densities.ContainsKey(quote.Price) && _densities[quote.Price].MaxVolume < quote.Volume;
            var isDensityBeingTooSmall = _densities.ContainsKey(quote.Price) && (quote.Volume<_densities[quote.Price].MaxVolume/3 && quote.Volume<SignificantVolume);
            var isShouldRenewVolumeOfDensity = _densities.ContainsKey(quote.Price) && quote.Volume!=_densities[quote.Price].Volume;
            var isNewDensity = quote.Volume>=SignificantVolume && !_densities.ContainsKey(quote.Price);
            
            if (isShouldRenewMaxVolume)
                _densities[quote.Price].MaxVolume = quote.Volume;

            if (isDensityBeingTooSmall)
            {
                _densities.Remove(quote.Price);
                // TODO event for opened position and order if have for action                        
            }

            if (isShouldRenewVolumeOfDensity && !isDensityBeingTooSmall)
                _densities[quote.Price].Volume = quote.Volume;

            if (isNewDensity)
                _densities.Add(quote.Price,new Density(quote.Price,quote.Volume,quote.Volume,quote.OrderDirection));
        }
    }
}