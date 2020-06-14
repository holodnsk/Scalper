﻿using System;
using System.Collections.Generic;
using System.Linq;
using StockSharp.BusinessEntities;
using StockSharp.Messages;

namespace Scalper
{
    public class AFLT
    {
        private static AFLT Instanse = new AFLT();
        private DepthView _depthView;
        private DensitiesContainer lowDensitiesContainer = new DensitiesContainer();
        private DensitiesContainer highDensitiesContainer = new DensitiesContainer();

        private AFLT()
        {
            SetStrategyParameters();
            _depthView = new DepthView {Title = "AFLT"};
            _depthView.Show();
        }

        private decimal LowBigValue { get; set; }

        private void SetStrategyParameters()
        {
            // todo 
            LowBigValue = 10000;
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

        private Dictionary<Decimal,Decimal> bigValues = new Dictionary<decimal, decimal>();


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
                if (quote.OrderDirection==Sides.Buy)
                {
                    lowDensitiesContainer.HandleValue(quote);
                }
            }
        }

        private void SetStoppersHighBigValue()
        {
            foreach (Quote quote in CurrentMarketDepth)
            {
                if (quote.OrderDirection==Sides.Sell)
                {
                    highDensitiesContainer.HandleValue(quote);
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