using System;
using System.Collections.Generic;
using StockSharp.BusinessEntities;

namespace Scalper
{
    public class DensitiesContainer
    {
        public DensitiesContainer()
        {
            Densities = new HashSet<Density>();
        }

        public HashSet<Density> Densities { get; set; }
        private const Decimal SignificantValue = 2000; // TODO config this

        public void AddValue(Quote quote)
        {
            if (quote.Volume>=SignificantValue)
            {
                Density density = new Density(quote.Price,quote.Volume);
                if (Densities.Contains(density))
                {
                    
                }
                else
                {
                    Densities.Add(density);
                }

                
                
            }
            
        }
    }
}