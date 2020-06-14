using StockSharp.Messages;

namespace Scalper
{
    public class Density
    {
        public Density(decimal price, decimal volume, decimal maxVolume, Sides direction)
        {
            Price = price;
            Volume = volume;
            MaxVolume = maxVolume;
            _direction = direction;
        }

        private readonly Sides _direction;
        private decimal Price { get; }
        public decimal Volume { get; set; }
        public decimal MaxVolume { get; set; }


        public override bool Equals(object density)
        {
            if ((density == null) || this.GetType() != density.GetType())
                return false;

            return density is Density density1 && (Price == density1.Price && _direction == density1._direction);
        }

        public override int GetHashCode()
        {
            return Price.GetHashCode();
        }
    }
}