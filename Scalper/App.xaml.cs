using System;
using System.Collections.Generic;
using System.IO;
using Ecng.Xaml;
using Newtonsoft.Json;
using StockSharp.Algo;
using StockSharp.BusinessEntities;
using StockSharp.Messages;
using StockSharp.SmartCom;
using Order = StockSharp.BusinessEntities.Order;
using Position = StockSharp.BusinessEntities.Position;


namespace Scalper
{
    public partial class App
    {
        private readonly TrafficMode _trafficMode = TrafficMode.Write;
        private readonly Connector _trader;
        private StreamWriter _trafficFile;
        private readonly string _dateTime;

        public App()
        {
            _dateTime = DateTime.Now.ToString("yyyy MM dd HH mm ss");
            _trader = new SmartTrader()
            {
                Login = "Y1K5D0D3",
                Password = "D8YYAP",
                Address = SmartComAddresses.Demo
            };

            if (_trafficMode == TrafficMode.Write)
            {
                SubscribeEvents();
                _trader.Connect();
            }

            if (_trafficMode ==TrafficMode.Read)
            {
                // TODO 
            }
        }

        private void SubscribeEvents()
        {
            _trader.ReConnectionSettings.WorkingTime = ExchangeBoard.Forts.WorkingTime;
            _trader.Restored += () => this.GuiAsync(RestoredEventHandler);
            _trader.Connected += () => this.GuiAsync(ConnectedEventHandler);
            _trader.Disconnected += () => this.GuiAsync(DisconnectedEventHandler);
            _trader.ConnectionError += error => this.GuiAsync(() => ConnectionErrorEventHandler(error));
            _trader.Error += error => this.GuiAsync(() => TransactionErrorEventHandler(error));
            _trader.MarketDataSubscriptionFailed += (security, msg, error) =>
                this.GuiAsync(() => MarketDataSubscriptionFailedEventHandler(security, msg, error));
            _trader.NewSecurity += security => this.GuiAsync(() => NewSecurityEventHandler(security));
            _trader.MarketDepthChanged += changedMarketDepth =>
                this.GuiAsync(() => MarketDepthChangedEventHandler(changedMarketDepth));
            _trader.MarketDepthsChanged += changedMarketDepths =>
                this.GuiAsync(() => MarketDepthsChangedEventHandler(changedMarketDepths));
            _trader.NewMyTrade += newMyTrade => this.GuiAsync(() => NewMyTradeEventHandler(newMyTrade));
            _trader.NewTrade += newTrade => this.GuiAsync(() => NewTradeEventHandler(newTrade));
            _trader.NewOrder += newOrder => this.GuiAsync(() => NewOrderEventHandler(newOrder));
            _trader.NewStopOrder += newStopOrder => this.GuiAsync(() => NewStopOrderEventHandler(newStopOrder));
            _trader.NewPortfolio += newPortfolio => this.GuiAsync(() => NewPortfolioEventHandler(newPortfolio));
            _trader.NewPosition += newPosition => this.GuiAsync(() => NewPositionEventHandler(newPosition));
            _trader.OrderRegisterFailed += orderRegisterFailed =>
                this.GuiAsync(() => OrderRegisterFailedEventHandler(orderRegisterFailed));
            _trader.OrderCancelFailed += orderCancelFailed =>
                this.GuiAsync(() => OrderCancelFailedEventHandler(orderCancelFailed));
            _trader.StopOrderRegisterFailed += stopOrderRegisterFailed =>
                this.GuiAsync(() => StopOrderRegisterFailedEventHandler(stopOrderRegisterFailed));
            _trader.StopOrderCancelFailed += stopOrderCancelFailed =>
                this.GuiAsync(() => StopOrderCancelFailedEventHandler(stopOrderCancelFailed));
            _trader.MassOrderCancelFailed += (transId, error) =>
                this.GuiAsync(() => MassOrderCancelFailedEventHandler(transId, error));
        }


        private void NewMyTradeEventHandler(MyTrade newMyTrade)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, newMyTrade);
        }

        private void MarketDataSubscriptionFailedEventHandler(Security security, MarketDataMessage msg, Exception error)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, security, msg,
                error);
        }

        private void DisconnectedEventHandler()
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void MassOrderCancelFailedEventHandler(long transId, Exception error)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, transId, error);
        }

        private void StopOrderCancelFailedEventHandler(OrderFail stopOrderCancelFailed)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name,
                stopOrderCancelFailed);
        }

        private void StopOrderRegisterFailedEventHandler(OrderFail stopOrderRegisterFailed)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name,
                stopOrderRegisterFailed);
        }

        private void OrderCancelFailedEventHandler(OrderFail orderCancelFailed)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, orderCancelFailed);
        }

        private void OrderRegisterFailedEventHandler(OrderFail orderRegisterFailed)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, orderRegisterFailed);
        }

        private void NewPositionEventHandler(Position newPosition)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, newPosition);
        }

        private void NewPortfolioEventHandler(Portfolio newPortfolio)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, newPortfolio);
        }

        private void NewStopOrderEventHandler(Order newStopOrder)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, newStopOrder);
        }

        private void NewOrderEventHandler(Order newOrder)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, newOrder);
        }

        private void NewTradeEventHandler(Trade newTrade)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, newTrade);
        }

        private void RestoredEventHandler()
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void ConnectionErrorEventHandler(Exception error)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, error);
        }

        private void ConnectedEventHandler()
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void TransactionErrorEventHandler(Exception error)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, error);
        }

        private void MarketDepthsChangedEventHandler(IEnumerable<MarketDepth> changedMarketDepths)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, changedMarketDepths);
        }

        private void MarketDepthChangedEventHandler(MarketDepth changedMarketDepth)
        {
            WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, changedMarketDepth);
        }

        private void NewSecurityEventHandler(Security security)
        {
            if (Contains(security))
            {
                WriteTrafficIfTrafficModeIsWrite(System.Reflection.MethodBase.GetCurrentMethod().Name, security);

                _trader.RegisterMarketDepth(security);
            }
        }

        private static bool Contains(Security security)
        {
            return security.Code.Contains("AFLT")
                   || security.Code.Contains("PLZL")
                   || security.Code.Contains("ALRS");
        }

        private void WriteTrafficIfTrafficModeIsWrite(string trafficEventHandlerName)
        {
            if (_trafficMode != TrafficMode.Write)
                return;

            _trafficFile = new StreamWriter(@"traficFile " + _dateTime + "txt", true);
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);
            jsonWriter.WriteEndObject();
            _trafficFile.Write(textWriter);
            _trafficFile.Close();
        }

        private void WriteTrafficIfTrafficModeIsWrite(string trafficEventHandlerName, object objectForWrite)
        {
            if (_trafficMode != TrafficMode.Write)
                return;

            _trafficFile = new StreamWriter(@"traficFile " + _dateTime + "txt", true);
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);
            jsonWriter.WritePropertyName("serializedObject");
            new JsonSerializer().Serialize(jsonWriter, objectForWrite);

            jsonWriter.WriteEndObject();
            _trafficFile.Write(textWriter);
            _trafficFile.Close();
        }

        private void WriteTrafficIfTrafficModeIsWrite(string trafficEventHandlerName, object objectForWrite1,
            object objectForWrite2)
        {
            if (_trafficMode != TrafficMode.Write)
                return;

            _trafficFile = new StreamWriter(@"traficFile " + _dateTime + "txt", true);
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
            _trafficFile.Write(textWriter);
            _trafficFile.Close();
        }

        private void WriteTrafficIfTrafficModeIsWrite(string trafficEventHandlerName, object objectForWrite1,
            object objectForWrite2,
            object objectForWrite3)
        {
            if (_trafficMode != TrafficMode.Write)
                return;
            _trafficFile = new StreamWriter(@"traficFile " + _dateTime + "txt", true);
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
            _trafficFile.Write(textWriter);
            _trafficFile.Close();
        }

        private enum TrafficMode
        {
            Write,
            Read
        }
    }
}