using System.Collections.Generic;
using StockSharp.BusinessEntities;

namespace Scalper
{
    public class DensitiesContainer
    {
        private readonly Dictionary<decimal, Density> _densities = new Dictionary<decimal, Density>();
        private const decimal SignificantVolume = 2000; // TODO config this

        public void HandleValue(Quote quote)
        {
            var containsDensity = _densities.ContainsKey(quote.Price);
            var isShouldRenewMaxVolume = containsDensity && _densities[quote.Price].MaxVolume < quote.Volume;
            var isDensityBeingTooSmall = containsDensity && quote.Volume < _densities[quote.Price].MaxVolume / 3 &&
                                         quote.Volume < SignificantVolume;
            var isShouldRenewVolumeOfDensity = containsDensity && quote.Volume != _densities[quote.Price].Volume;
            var isNewDensity = quote.Volume >= SignificantVolume && !containsDensity;

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
                _densities.Add(quote.Price, new Density(quote.Price, quote.Volume, quote.Volume, quote.OrderDirection));
        }
    }
}