
using System;
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



namespace Scalper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        


        public App()
        {
            
            
            Connector Trader = new SmartTrader()
            {
                Login = "Y1K5D0D3",
                Password = "D8YYAP",
                Address = SmartComAddresses.Demo
                
            };
            

            // инициализируем механизм переподключения
            Trader.ReConnectionSettings.WorkingTime = ExchangeBoard.Forts.WorkingTime;

            Trader.Restored += () => this.GuiAsync(() =>  restoredEventHandler());

            // подписываемся на событие успешного соединения
            Trader.Connected += () => this.GuiAsync(() =>  connectedEventHandler());
            
            // Trader.Disconnected += () => this.GuiAsync(() => ChangeConnectStatus(false));

            // подписываемся на событие разрыва соединения
            Trader.ConnectionError += error => this.GuiAsync(() => connectionErrorEventHandler(error));

            // подписываемся на ошибку обработки данных (транзакций и маркет)
            Trader.Error += error => this.GuiAsync(() => transactionErrorEventHandler());

            // подписываемся на ошибку подписки маркет-данных
            //Trader.MarketDataSubscriptionFailed += (security, msg, error) => this.GuiAsync(() => MessageBox.Show(this, error.ToString(), LocalizedStrings.Str2956Params.Put(msg.DataType, security)));

            // Trader.NewSecurity += _securitiesWindow.SecurityPicker.Securities.Add;
            Trader.NewSecurity += security => this.GuiAsync(() => newSecurityEventHandler(security));
           

            //Trader.NewMyTrade += _myTradesWindow.TradeGrid.Trades.Add;
            //Trader.NewTrade += _tradesWindow.TradeGrid.Trades.Add;
            Trader.NewTrade += trade =>  this.GuiAsync(() => newTradeEventHandler());

            //Trader.NewOrder += _ordersWindow.OrderGrid.Orders.Add;
            Trader.NewOrder += order => this.GuiAsync(() => newOrderEventHandler());

            //Trader.NewStopOrder += _stopOrdersWindow.OrderGrid.Orders.Add;
            Trader.NewStopOrder += stoporder => this.GuiAsync(() => newStopOrder());

            //Trader.NewPortfolio += _portfoliosWindow.PortfolioGrid.Portfolios.Add;
            Trader.NewPortfolio += NewPortfolio => this.GuiAsync(() => newPortfolioEventHandler());

            //Trader.NewPosition += _portfoliosWindow.PortfolioGrid.Positions.Add;
            Trader.NewPosition += NewPosition => this.GuiAsync(() => newPositionEventHandler());

            // подписываемся на событие о неудачной регистрации заявок
            //Trader.OrderRegisterFailed += _ordersWindow.OrderGrid.AddRegistrationFail;
            Trader.OrderRegisterFailed += OrderRegisterFailed =>this.GuiAsync(() => orderRegisterFailedEventHandler());

            // подписываемся на событие о неудачном снятии заявок
            //Trader.OrderCancelFailed += OrderFailed;
            Trader.OrderCancelFailed += OrderCancelFailed => this.GuiAsync(() => orderCancelFailedEventHandler());

            // подписываемся на событие о неудачной регистрации стоп-заявок
            //Trader.StopOrderRegisterFailed += _stopOrdersWindow.OrderGrid.AddRegistrationFail;
            Trader.StopOrderRegisterFailed += StopOrderRegisterFailed => this.GuiAsync(() => stopOrderRegisterFailedEventHandler());

            // подписываемся на событие о неудачном снятии стоп-заявок
            //Trader.StopOrderCancelFailed += OrderFailed;
            Trader.StopOrderCancelFailed += StopOrderCancelFailed => this.GuiAsync(() => stopOrderCancelFailedEventHandler());

            //Trader.MassOrderCancelFailed += (transId, error) =>this.GuiAsync(() => MessageBox.Show(this, error.ToString(), LocalizedStrings.Str716));
            Trader.MassOrderCancelFailed += (transId, error) => this.GuiAsync(() => massOrderCancelFailedEventHandler());

            // устанавливаем поставщик маркет-данных
            //_securitiesWindow.SecurityPicker.MarketDataProvider = Trader;

            //ShowSecurities.IsEnabled = ShowTrades.IsEnabled =
            //ShowMyTrades.IsEnabled = ShowOrders.IsEnabled =
            //ShowPortfolios.IsEnabled = ShowStopOrders.IsEnabled = true;


            //Trader.Login = Login.Text;
            //Trader.Password = Password.Password;
            //Trader.Address = Address.SelectedAddress;

            // применить нужную версию SmartCOM

            //Trader.Version = IsSmartCom4.IsChecked == true ? SmartComVersions.V4 : SmartComVersions.V3;

            // очищаем из текстового поля в целях безопасности
            //Password.Clear();

            Trader.Connect();

        }

        private static void massOrderCancelFailedEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void stopOrderCancelFailedEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void stopOrderRegisterFailedEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void orderCancelFailedEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void orderRegisterFailedEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newPositionEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newPortfolioEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newStopOrder()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newOrderEventHandler()
        {
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void newTradeEventHandler()
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
            // возводим флаг, что соединение установлено
            // _isConnected = true;

            // разблокируем кнопку Подключиться
            // this.GuiAsync(() => ChangeConnectStatus(true));
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private static void transactionErrorEventHandler()
        {
            //MessageBox.Show((Window) this, error.ToString(), LocalizedStrings.Str2955);
            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private void newSecurityEventHandler(Security security)
        {
            {
                //Console.WriteLine(security.Name);
                Console.WriteLine(security.Code);
                
                //Console.WriteLine(security.Class);

                Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            };
        }
    }
}
