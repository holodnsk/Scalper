using System.Linq;
using StockSharp.Algo;
using StockSharp.BusinessEntities;
using StockSharp.Messages;

namespace Scalper
{
    public class AFLT
    {
        
        private readonly DepthView _depthView;
        private readonly DensitiesContainer _lowDensitiesContainer = new DensitiesContainer();
        private readonly DensitiesContainer _highDensitiesContainer = new DensitiesContainer();
        private readonly Connector _trader;

        public AFLT(Connector trader)
        {
            _lowDensitiesContainer.NewDensityEvent += quote => NewDensityEventHandler(quote);
            _lowDensitiesContainer.DensityRemovedEvent += quote => DensityRemovedEventHandler(quote);
            _trader = trader;
            SetStrategyParameters();
            _depthView = new DepthView {Title = "AFLT"};
            _depthView.Show();
        }

        private void NewDensityEventHandler(Quote quote)
        {
            var securityPriceStep = quote.Security.PriceStep.Value;
            var quotePriceBuy = quote.Price+securityPriceStep;
            decimal quotePriceSell = quote.Price-securityPriceStep;
            _trader.RegisterOrder(new Order()
            {
                Direction = quote.OrderDirection,
                Security = quote.Security,
                Volume = 1,
                Price = quote.OrderDirection==Sides.Buy?quotePriceBuy:quotePriceSell
            });
        }

        private void DensityRemovedEventHandler(Quote quote)
        {
            
        }

        private void SetStrategyParameters()
        {
            // todo 
        }

        public void NewMarketDepth(MarketDepth changedMarketDepth)
        {
            CurrentMarketDepth = changedMarketDepth;
        }

        private MarketDepth _currentMarketDepth;

        private MarketDepth CurrentMarketDepth
        {
            get => _currentMarketDepth;
            set
            {
                _currentMarketDepth = value;
                CheckSignals();

                string depthTable = "";
                foreach (var quote in _currentMarketDepth.Asks.Reverse())
                    depthTable += quote + "\n";

                foreach (var quote in _currentMarketDepth.Bids)
                    depthTable += quote + "\n";
                depthTable += _currentMarketDepth.LastChangeTime;

                _depthView.depthContent.Text = depthTable;
            }
        }

        private void CheckSignals()
        {
            SetStoppersLowBigValue();
            SetStoppersHighBigValue();

            CheckStopperLowIceberg();
            checkStopperHighIceberg();

            CheckStopperLowHiddenIceberg();
            CheckStopperHighHiddenIceberg();

            CheckStopperLowMarketMakerValue();
            CheckStopperHighMarketMakerValue();
            // TODO more stoppers

            CheckVectorLowBigValuePreFinal();
            CheckVectorHighBigValuePreFinal();
            // TODO more vectors
        }

        private void SetStoppersLowBigValue()
        {
            foreach (Quote quote in CurrentMarketDepth)
            {
                if (quote.OrderDirection == Sides.Buy)
                {
                    _lowDensitiesContainer.HandleValue(quote);
                }
            }
        }

        private void SetStoppersHighBigValue()
        {
            foreach (Quote quote in CurrentMarketDepth)
            {
                if (quote.OrderDirection == Sides.Sell)
                {
                    _highDensitiesContainer.HandleValue(quote);
                }
            }
        }

        private void CheckStopperLowIceberg()
        {
        }

        private void checkStopperHighIceberg()
        {
        }

        private void CheckStopperLowHiddenIceberg()
        {
        }

        private void CheckStopperHighHiddenIceberg()
        {
        }

        private void CheckStopperLowMarketMakerValue()
        {
        }

        private void CheckStopperHighMarketMakerValue()
        {
        }

        private void CheckVectorHighBigValuePreFinal()
        {
        }

        private void CheckVectorLowBigValuePreFinal()
        {
        }
    }
}