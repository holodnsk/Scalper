using StockSharp.BusinessEntities;

namespace Scalper.Strategies
{
    public class AFLT
    {
        private static AFLT Instanse = new AFLT();
        private MarketDepth _currentMarketDepth;

        public AFLT()
        {
            
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
                //MainWindow.
            }
        }
    }
}