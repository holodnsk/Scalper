using System;
using System.Linq;
using StockSharp.BusinessEntities;

namespace Scalper.Strategies
{
    public class AFLT
    {
        private static AFLT Instanse = new AFLT();
        private MarketDepth _currentMarketDepth;
        private DepthView _depthView;

        public AFLT()
        {
            _depthView = new DepthView();
            _depthView.Title = "AFLT";
            _depthView.Show();
        }

        public static void NewMarketDepth(MarketDepth changedMarketDepth)
        {
            Instanse.CurrentMarketDepth = changedMarketDepth;
        }

        public MarketDepth CurrentMarketDepth
        {
            get => _currentMarketDepth;
            set { 
            _currentMarketDepth = value;
            checkSignals();
            
            string depthTable = "";
            foreach (var quote in _currentMarketDepth.Asks.Reverse())
                depthTable += quote+"\n";
            
            foreach (var quote in _currentMarketDepth.Bids)
                depthTable += quote+"\n";
            depthTable += _currentMarketDepth.LastChangeTime;
            
            _depthView.depthContent.Text = depthTable;
            }
        }

        private void checkSignals()
        {
            // check all stoppers
            checkBigLowValue();
            checkBigLowIceberg();
            checkBigLowHiddenIceberg();
            checkBigHighValue();
            checkBigHighIceberg();
            checkBigHighHiddenIceberg();
            checkMarketMakerHighValue();
            checkMarketMakerLowValue();
            // TODO more stoppers
            
            // check all vectors
            checkPreFinalBigLowValue();
            checkPreFinalBigHighValue();
            // TODO more vectors

        }

        private void checkMarketMakerLowValue()
        {
            
        }

        private void checkMarketMakerHighValue()
        {
            
        }

        private void checkPreFinalBigHighValue()
        {
            
        }

        private void checkPreFinalBigLowValue()
        {
            
        }

        private void checkBigHighHiddenIceberg()
        {
            
        }

        private void checkBigHighIceberg()
        {
            
        }

        private void checkBigLowHiddenIceberg()
        {
            
        }

        private void checkBigLowIceberg()
        {
            
        }

        private void checkBigHighValue()
        {
            
        }

        private void checkBigLowValue()
        {
            
        }
    }
}