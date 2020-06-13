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
        private const Decimal SignificantValue = 2000; // TODO config this

        public void HandleValue(Quote quote)
        {
            Density potentialDensity = new Density(quote.Price,quote.Volume);
            if (Densities.ContainsKey(potentialDensity.Price))
            {
                Density prevDensity = Densities[potentialDensity.Price];

                if (quote.Volume<prevDensity.MaxValue/3) // change to logic based maxvalue of density instead constant SignificantValue
                {
                    // TODO close position and order if have                        
                }
                else
                {
                    // TODO renew 
                }
            }
            else
            {
                if (quote.Volume>=SignificantValue)
                {
                    Densities.Add(potentialDensity.Price,potentialDensity);
                }                
            }

        }
    }
}