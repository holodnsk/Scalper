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
            if (Densities.ContainsKey(quote.Price))
            {
                if (IsShouldRenewMaxVolume(quote))
                    RenewMaxVolume(quote);

                if (IsDensityBeingTooSmall(quote))
                {
                    RemoveDensityEntity(quote);
                    // TODO event for opened position and order if have for action                        
                }
                else
                {
                    RenewCurrentVolume(quote);
                }
            }
            
            if (IsNewDensity(quote))
            {
                AddNewDensityEntity(quote);
            }                
            
        }

        private bool RemoveDensityEntity(Quote quote)
        {
            return Densities.Remove(quote.Price);
        }

        private void AddNewDensityEntity(Quote quote)
        {
            Densities.Add(quote.Price,new Density(quote.Price,quote.Volume,quote.Volume));
        }

        private bool IsNewDensity(Quote quote)
        {
            return quote.Volume>=SignificantVolume && !Densities.ContainsKey(quote.Price);
        }

        private decimal RenewCurrentVolume(Quote quote)
        {
            return Densities[quote.Price].Volume = quote.Volume;
        }

        private decimal RenewMaxVolume(Quote quote)
        {
            return Densities[quote.Price].MaxVolume = quote.Volume;
        }

        private bool IsDensityBeingTooSmall(Quote quote)
        {
            return quote.Volume<Densities[quote.Price].MaxVolume/3 && quote.Volume<SignificantVolume;
        }

        private bool IsShouldRenewMaxVolume(Quote quote)
        {
            return Densities[quote.Price].MaxVolume < quote.Volume;
        }
    }
}