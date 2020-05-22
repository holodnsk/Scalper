
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using Ecng.Common;
using Ecng.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        enum TrafficMode
        {
            WRITE,
            READ
        }

        private TrafficMode trafficMode = TrafficMode.WRITE;
        
        private Connector Trader;
        StreamWriter traficFile;
        private string dateTime;


        public App()
        {
            
            dateTime = DateTime.Now.ToString("yyyy MM dd HH mm ss");

            Trader = new SmartTrader()
            {
                Login = "Y1K5D0D3",
                Password = "D8YYAP",
                Address = SmartComAddresses.Demo
        
            };


            /*trader = new PlazaTrader()
            {
                Login = "tgFZcm_0002a",
                Password = "12345",
                Address = new DnsEndPoint("spectra-t1.moex.com",3001)
            };*/

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
            
            
            if (trafficMode==TrafficMode.WRITE)
            {
                Trader.Connect();
                //writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,Trader);
            }

        }



        private void newMyTradeEventHandler(MyTrade newMyTrade)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,newMyTrade);
            }
        }

        private void marketDataSubscriptionFailedEventHandler(Security security, MarketDataMessage msg, Exception error)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,security,msg,error);
            }
        }

        private void disconnectedEventHandler()
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        private void massOrderCancelFailedEventHandler(long transId, Exception error)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,transId,error);
            }
        }

        private void stopOrderCancelFailedEventHandler(OrderFail stopOrderCancelFailed)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,stopOrderCancelFailed);
            }
        }

        private void stopOrderRegisterFailedEventHandler(OrderFail stopOrderRegisterFailed)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,stopOrderRegisterFailed);
            }
        }

        private void orderCancelFailedEventHandler(OrderFail orderCancelFailed)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,orderCancelFailed);
            }
        }

        private void orderRegisterFailedEventHandler(OrderFail orderRegisterFailed)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,orderRegisterFailed);
            }
        }

        private void newPositionEventHandler(Position newPosition)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,newPosition);
            }
        }

        private void newPortfolioEventHandler(Portfolio newPortfolio)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,newPortfolio);
            }
        }

        private void newStopOrderEventHandler(Order newStopOrder)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,newStopOrder);
            }
        }

        private void newOrderEventHandler(Order newOrder)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,newOrder);
            }
        }

        private void newTradeEventHandler(Trade newTrade)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,newTrade);
            }
        }

        private void restoredEventHandler()
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        private void connectionErrorEventHandler(Exception error)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,error);
            }
        }

        private void connectedEventHandler()
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }


        // подписываемся на ошибку обработки данных (транзакций и маркет)

        private void transactionErrorEventHandler(Exception error)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,error);
            }
        }

        private void marketDepthsChangedEventHandler(IEnumerable<MarketDepth> changedMarketDepths)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,changedMarketDepths);
            }
        }

        private void marketDepthChangedEventHandler(MarketDepth changedMarketDepth)
        {
            if (trafficMode==TrafficMode.WRITE)
            {
                writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,changedMarketDepth);
            }
        }

        private void newSecurityEventHandler(Security security)
        {
            if (security.Code.Contains("EUR_RUB_TOM") 
                || security.Code.Contains("AFLT")
                || security.Code.Contains("PLZL")
                || security.Code.Contains("ALRS")
                || security.Code.Contains("USD000UTSTOM"))
            {
                if (trafficMode==TrafficMode.WRITE)
                {
                    writeTraffic(System.Reflection.MethodInfo.GetCurrentMethod().Name,security);
                }
                Trader.RegisterMarketDepth(security);
                
            }
        }

        private void writeTraffic(string trafficEventHandlerName)
        {
            traficFile = new StreamWriter(@"traficFile " + dateTime + "txt", true);
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);
            jsonWriter.WriteEndObject();
            traficFile.Write(textWriter);
            traficFile.Close();
        }

        private void writeTraffic(string trafficEventHandlerName, object  objectForWrite)
        {
            traficFile = new StreamWriter(@"traficFile " + dateTime + "txt", true);
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);
            jsonWriter.WritePropertyName("serializedObject");
            new JsonSerializer().Serialize(jsonWriter, objectForWrite);

            jsonWriter.WriteEndObject();
            traficFile.Write(textWriter);
            traficFile.Close();
        }

        private void writeTraffic(string trafficEventHandlerName, object objectForWrite1, object objectForWrite2)
        {
            traficFile = new StreamWriter(@"traficFile " + dateTime + "txt", true);
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);
            jsonWriter.WritePropertyName("serializedObject1");
            new JsonSerializer().Serialize(jsonWriter, objectForWrite1);
            jsonWriter.WritePropertyName("serializedObject2");
            new JsonSerializer().Serialize(jsonWriter, objectForWrite2);

            jsonWriter.WriteEndObject();
            traficFile.Write(textWriter);
            traficFile.Close();
        }

        private void writeTraffic(string trafficEventHandlerName, object objectForWrite1, object objectForWrite2, object objectForWrite3)
        {
            traficFile = new StreamWriter(@"traficFile " + dateTime + "txt", true);
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);
            jsonWriter.WritePropertyName("serializedObject1");
            new JsonSerializer().Serialize(jsonWriter, objectForWrite1);
            jsonWriter.WritePropertyName("serializedObject2");
            new JsonSerializer().Serialize(jsonWriter, objectForWrite2);
            jsonWriter.WritePropertyName("serializedObject3");
            new JsonSerializer().Serialize(jsonWriter, objectForWrite3);

            jsonWriter.WriteEndObject();
            traficFile.Write(textWriter);
            traficFile.Close();
        }
    }
}
