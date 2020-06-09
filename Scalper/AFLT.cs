using System.Linq;
using StockSharp.BusinessEntities;

namespace Scalper
{
    public class AFLT
    {
        private static AFLT Instanse = new AFLT();
        private DepthView _depthView;

        private AFLT()
        {
            _depthView = new DepthView {Title = "AFLT"};
            _depthView.Show();
        }

        public static void NewMarketDepth(MarketDepth changedMarketDepth)
        {
            Instanse.CurrentMarketDepth = changedMarketDepth;
        }

        private MarketDepth _currentMarketDepth;
        private MarketDepth CurrentMarketDepth
        {
            get => _currentMarketDepth;
            set { 
            _currentMarketDepth = value;
            CheckSignals();
            
            string depthTable = "";
            foreach (var quote in _currentMarketDepth.Asks.Reverse())
                depthTable += quote+"\n";
            
            foreach (var quote in _currentMarketDepth.Bids)
                depthTable += quote+"\n";
            depthTable += _currentMarketDepth.LastChangeTime;
            
            _depthView.depthContent.Text = depthTable;
            }
        }

        private void CheckSignals()
        {
            CheckStopperLowBigValue();
            CheckStopperHighBigValue();

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

        private void CheckStopperLowBigValue()
        {
            
        }

        private void CheckStopperHighBigValue()
        {
            
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