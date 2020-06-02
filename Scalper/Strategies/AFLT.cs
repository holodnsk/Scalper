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
            string depthTable = "";
            foreach (var quote in _currentMarketDepth.Asks.Reverse())
                depthTable += quote+"\n";
            
            foreach (var quote in _currentMarketDepth.Bids)
                depthTable += quote+"\n";
            
            _depthView.depthContent.Text = depthTable;
            }
        }
    }
}