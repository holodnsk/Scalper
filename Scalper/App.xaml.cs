
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using Ecng.Common;
using Ecng.Xaml;
using NPOI.SS.Formula.Functions;
using OEC.Data;
using StockSharp.Algo;
using StockSharp.BusinessEntities;
using StockSharp.Localization;
using StockSharp.Messages;
using StockSharp.SmartCom;
using StockSharp.SmartCom.Native;
using Order = StockSharp.BusinessEntities.Order;
using Position = StockSharp.BusinessEntities.Position;


namespace Scalper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Connector Trader;


        public App()
        {
            Trader = new SmartTrader()
            {
                Login = "Y1K5D0D3",
                Password = "D8YYAP",
                Address = SmartComAddresses.Demo
                
            };

            // инициализируем механизм переподключения
            Trader.ReConnectionSettings.WorkingTime = ExchangeBoard.Forts.WorkingTime;
            Trader.Restored += () => this.GuiAsync(() =>  restoredEventHandler());
            Trader.Connected += () => this.GuiAsync(() =>  connectedEventHandler());
            Trader.Disconnected += () => this.GuiAsync(() => disconnectedEventHandler());
            Trader.ConnectionError += error => this.GuiAsync(() => connectionErrorEventHandler(error));
            Trader.Error += error => this.GuiAsync(() => transactionErrorEventHandler(error));
            Trader.MarketDataSubscriptionFailed += (security, msg, error) => this.GuiAsync(() => marketDataSubscriptionFailedEventHandler(security, msg, error));
            Trader.NewSecurity += security => this.GuiAsync(() => newSecurityEventHandler(security));
            Trader.MarketDepthChanged += changedMarketDepth => this.GuiAsync(() => marketDepthChangedEventHandler(changedMarketDepth));
            Trader.MarketDepthsChanged += changedMarketDepths => this.GuiAsync(() => marketDepthsChangedEventHandler(changedMarketDepths));
            Trader.NewMyTrade += newMyTrade =>  this.GuiAsync(() => newMyTradeEventHandler(newMyTrade));
            Trader.NewTrade += newTrade =>  this.GuiAsync(() => newTradeEventHandler(newTrade));
            Trader.NewOrder += newOrder => this.GuiAsync(() => newOrderEventHandler(newOrder));
            Trader.NewStopOrder += newStopOrder => this.GuiAsync(() => newStopOrderEventHandler(newStopOrder));
            Trader.NewPortfolio += newPortfolio => this.GuiAsync(() => newPortfolioEventHandler(newPortfolio));
            Trader.NewPosition += newPosition => this.GuiAsync(() => newPositionEventHandler(newPosition));
            Trader.OrderRegisterFailed += orderRegisterFailed =>this.GuiAsync(() => orderRegisterFailedEventHandler(orderRegisterFailed));
            Trader.OrderCancelFailed += orderCancelFailed => this.GuiAsync(() => orderCancelFailedEventHandler(orderCancelFailed));
            Trader.StopOrderRegisterFailed += stopOrderRegisterFailed => this.GuiAsync(() => stopOrderRegisterFailedEventHandler(stopOrderRegisterFailed));
            Trader.StopOrderCancelFailed += stopOrderCancelFailed => this.GuiAsync(() => stopOrderCancelFailedEventHandler(stopOrderCancelFailed));
            Trader.MassOrderCancelFailed += (transId, error) => this.GuiAsync(() => massOrderCancelFailedEventHandler(transId,error));
            
            Trader.Connect();

        }



        private void newMyTradeEventHandler(MyTrade newMyTrade)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private void marketDataSubscriptionFailedEventHandler(Security security, MarketDataMessage msg, Exception error)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private void disconnectedEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void massOrderCancelFailedEventHandler(long transId, Exception error)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void stopOrderCancelFailedEventHandler(OrderFail stopOrderCancelFailed)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void stopOrderRegisterFailedEventHandler(OrderFail stopOrderRegisterFailed)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void orderCancelFailedEventHandler(OrderFail orderCancelFailed)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void orderRegisterFailedEventHandler(OrderFail orderRegisterFailed)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newPositionEventHandler(Position newPosition)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newPortfolioEventHandler(Portfolio newPortfolio)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newStopOrderEventHandler(Order newStopOrder)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newOrderEventHandler(Order newOrder)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newTradeEventHandler(Trade newTrade)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void restoredEventHandler()
        {
            // разблокируем кнопку Экспорт (соединение было восстановлено)
            // ChangeConnectStatus(true);
            // MessageBox.Show(this, LocalizedStrings.Str2958);
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private void connectionErrorEventHandler(Exception error)
        {
            // заблокируем кнопку Экспорт (так как соединение было потеряно)
            //ChangeConnectStatus(false);

            //MessageBox.Show(this, error.ToString(), LocalizedStrings.Str2959);
            Console.WriteLine(this + "\n" + error.ToString());
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void connectedEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        // подписываемся на ошибку обработки данных (транзакций и маркет)
        private static void transactionErrorEventHandler(Exception error)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }
        
        private void marketDepthsChangedEventHandler(IEnumerable<MarketDepth> changedMarketDepths)
        {
            throw new NotImplementedException();
        }

        private void marketDepthChangedEventHandler(MarketDepth changedMarketDepth)
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private void newSecurityEventHandler(Security security)
        {
            if (security.Code.Contains("EUR_RUB_TOM") 
                || security.Code.Contains("AFLT")
                || security.Code.Contains("PLZL")
                || security.Code.Contains("ALRS")
                || security.Code.Contains("USD000UTSTOM"))
            {
                Trader.RegisterMarketDepth(security);
                
            }
        }

    }
}
