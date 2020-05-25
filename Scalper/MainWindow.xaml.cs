using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using Ecng.Common;
using Ecng.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockSharp.Algo;
using StockSharp.BusinessEntities;
using StockSharp.Messages;
using StockSharp.SmartCom;

namespace Scalper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            initConnections();
        }

        public void ShowLogMessage(String message)
        {
            systemLog.Text = systemLog.Text + "\n" + message;

        }
        
        private enum TrafficMode { Write, Read }
        private readonly TrafficMode _trafficMode = TrafficMode.Read;

        private Connector _trader;

        private StreamWriter _trafficFile;
        private StreamReader _trafficSourceFile;

        private string _dateTime;

        private static bool CfgTradingSecurity(Security security)
        {
            return security.Code.Contains("AFLT")
                   || security.Code.Contains("PLZL")
                   || security.Code.Contains("EUR_RUB_TOM")
                   || security.Code.Contains("ALRS");
        }

        private void initConnections()
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
                Directory.CreateDirectory("traffic");
                _trafficFile = new StreamWriter(@"traffic\trafficFile " + _dateTime + ".txt", true);
                SubscribeEvents();
                _trader.Connect();
            } 

            if (_trafficMode == TrafficMode.Read)
            {
                IEnumerable trafficFileList = Directory.EnumerateFiles("traffic");
                SelectTrafficSourceDialog dialog = new SelectTrafficSourceDialog("trafficFileList");


                _trafficSourceFile = new StreamReader("trafficFile 2020 05 25 00 57 28.txt");
                
                while (true)
                {
                    string line = _trafficSourceFile.ReadLine();
                    if (line == null)
                        break;

                    JObject jObject = JObject.Parse(line);
                    string trafficEventHandlerName = jObject.Property("trafficEventHandlerName").ToString()
                        .Replace("trafficEventHandlerName", "")
                        .Replace("\"", "")
                        .Replace(" ", "")
                        .Replace(":", "");
                    switch (trafficEventHandlerName)
                    {
                        case "trafficEventHandlerName: RestoredEventHandler":
                            RestoredEventHandler();
                            break;
                        case "trafficEventHandlerName: ConnectedEventHandler":
                            ConnectedEventHandler();
                            break;
                        case "trafficEventHandlerName: DisconnectedEventHandler":
                            DisconnectedEventHandler();
                            break;
                        case "trafficEventHandlerName: ConnectionErrorEventHandler":

                            ConnectionErrorEventHandler(
                                JsonConvert.DeserializeObject<Exception>(
                                    jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: TransactionErrorEventHandler":
                            TransactionErrorEventHandler(
                                JsonConvert.DeserializeObject<Exception>(
                                    jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: MarketDataSubscriptionFailedEventHandler":
                            MarketDataSubscriptionFailedEventHandler(
                                JsonConvert.DeserializeObject<Security>(
                                    jObject.Property("serializedObject1").ToString()),
                                JsonConvert.DeserializeObject<MarketDataMessage>(jObject.Property("serializedObject2")
                                    .ToString()),
                                JsonConvert.DeserializeObject<Exception>(jObject.Property("serializedObject3")
                                    .ToString()));
                            break;
                        case "newSecurityEventHandler":
                            string value = jObject.Property("serializedObject").ToString()
                                .Replace("\"serializedObject\": ", "");
                            NewSecurityEventHandler(
                                JsonConvert.DeserializeObject<Security>(value));
                            break;
                        case "trafficEventHandlerName: MarketDepthChangedEventHandler":
                            MarketDepthChangedEventHandler(
                                JsonConvert.DeserializeObject<MarketDepth>(jObject.Property("serializedObject")
                                    .ToString()));
                            break;
                        case "trafficEventHandlerName: MarketDepthsChangedEventHandler":
                            MarketDepthsChangedEventHandler(
                                JsonConvert.DeserializeObject<Enumerable<MarketDepth>>(jObject
                                    .Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: NewMyTradeEventHandler":
                            NewMyTradeEventHandler(
                                JsonConvert.DeserializeObject<MyTrade>(jObject.Property("serializedObject")
                                    .ToString()));
                            break;
                        case "trafficEventHandlerName: NewTradeEventHandler":
                            NewTradeEventHandler(
                                JsonConvert.DeserializeObject<Trade>(jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: NewOrderEventHandler":
                            NewOrderEventHandler(
                                JsonConvert.DeserializeObject<Order>(jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: NewStopOrderEventHandler":
                            NewStopOrderEventHandler(
                                JsonConvert.DeserializeObject<Order>(jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: NewPortfolioEventHandler":
                            NewPortfolioEventHandler(
                                JsonConvert.DeserializeObject<Portfolio>(
                                    jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: NewPositionEventHandler":
                            NewPositionEventHandler(
                                JsonConvert.DeserializeObject<Position>(jObject.Property("serializedObject")
                                    .ToString()));
                            break;
                        case "trafficEventHandlerName: OrderRegisterFailedEventHandler":
                            OrderRegisterFailedEventHandler(
                                JsonConvert.DeserializeObject<OrderFail>(
                                    jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: OrderCancelFailedEventHandler":
                            OrderCancelFailedEventHandler(
                                JsonConvert.DeserializeObject<OrderFail>(
                                    jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: StopOrderRegisterFailedEventHandler":
                            StopOrderRegisterFailedEventHandler(
                                JsonConvert.DeserializeObject<OrderFail>(
                                    jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: StopOrderCancelFailedEventHandler":
                            StopOrderCancelFailedEventHandler(
                                JsonConvert.DeserializeObject<OrderFail>(
                                    jObject.Property("serializedObject").ToString()));
                            break;
                        case "trafficEventHandlerName: MassOrderCancelFailedEventHandler":
                            MassOrderCancelFailedEventHandler(
                                JsonConvert.DeserializeObject<long>(jObject.Property("serializedObject1").ToString()),
                                JsonConvert.DeserializeObject<Exception>(jObject.Property("serializedObject2")
                                    .ToString()));
                            break;
                        default:
                            break;
                    }
                }
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
            WriteTraffic(MethodBase.GetCurrentMethod().Name, newMyTrade);
        }

        private void MarketDataSubscriptionFailedEventHandler(Security security, MarketDataMessage msg, Exception error)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, security, msg,
                error);
        }

        private void DisconnectedEventHandler()
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name);
        }

        private void MassOrderCancelFailedEventHandler(long transId, Exception error)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, transId, error);
        }

        private void StopOrderCancelFailedEventHandler(OrderFail stopOrderCancelFailed)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name,
                stopOrderCancelFailed);
        }

        private void StopOrderRegisterFailedEventHandler(OrderFail stopOrderRegisterFailed)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name,
                stopOrderRegisterFailed);
        }

        private void OrderCancelFailedEventHandler(OrderFail orderCancelFailed)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, orderCancelFailed);
        }

        private void OrderRegisterFailedEventHandler(OrderFail orderRegisterFailed)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, orderRegisterFailed);
        }

        private void NewPositionEventHandler(Position newPosition)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, newPosition);
        }

        private void NewPortfolioEventHandler(Portfolio newPortfolio)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, newPortfolio);
        }

        private void NewStopOrderEventHandler(Order newStopOrder)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, newStopOrder);
        }

        private void NewOrderEventHandler(Order newOrder)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, newOrder);
        }

        private void NewTradeEventHandler(Trade newTrade)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, newTrade);
        }

        private void RestoredEventHandler()
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name);
        }

        private void ConnectionErrorEventHandler(Exception error)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, error);
        }

        private void ConnectedEventHandler()
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name);
        }

        private void TransactionErrorEventHandler(Exception error)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, error);
        }

        private void MarketDepthsChangedEventHandler(IEnumerable<MarketDepth> changedMarketDepths)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, changedMarketDepths);
        }

        private void MarketDepthChangedEventHandler(MarketDepth changedMarketDepth)
        {
            WriteTraffic(MethodBase.GetCurrentMethod().Name, changedMarketDepth);
        }
        
        HashSet<string> boards = new HashSet<string>();
        private void NewSecurityEventHandler(Security security)
        {
            string board = security.Board.ToString();
            if (!boards.Contains(board))
            {
                boards.Add(board);
                ShowLogMessage(board);
            }
            
            if (CfgTradingSecurity(security))
            {
                WriteTraffic(MethodBase.GetCurrentMethod().Name, security);

                _trader.RegisterMarketDepth(security);
            }
        }

        private void WriteTraffic(string trafficEventHandlerName, params object[] objectsForWrite)
        {
            if (_trafficMode != TrafficMode.Write)
                return;

            trafficEntitiesCounter.Text = (Int32.Parse(trafficEntitiesCounter.Text.ToString())+1).ToString();
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);
            jsonWriter.WritePropertyName("serializedObjects");
            jsonWriter.WriteStartArray();
            foreach (object objectForWrite in objectsForWrite)
                new JsonSerializer().Serialize(jsonWriter, objectForWrite);
            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
            _trafficFile.Write(textWriter);
            _trafficFile.Write("\n");
            _trafficFile.Flush();
        }
    }
}
