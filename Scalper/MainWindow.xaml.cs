using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Ecng.Common;
using Ecng.Reflection;
using Ecng.Xaml;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Functions;
using OEC;
using ProtoBuf;
using StockSharp.Algo;
using StockSharp.BusinessEntities;
using StockSharp.Messages;
using StockSharp.SmartCom;

namespace Scalper
{
    public partial class MainWindow 
    {
        public MainWindow()
        {
            
            InitializeComponent();
            initConnections();
        }

        private void ShowLogMessage(String message)
        {
            systemLog.Text = systemLog.Text + "\n" + message;

        }
        
        private enum TrafficMode { Write, Read }

        private Connector _trader;

        private StreamWriter _trafficFile;

        private string _dateTime;

        private static bool CfgTradingSecurity(Security security)
        {
            return 
                security.Code.Contains("AFLT")
                   // || security.Code.Contains("PLZL")
                   // || security.Code.Contains("EUR_RUB_TOM")
                   // || security.Code.Contains("ETLN")
                   // || security.Code.Contains("RASP")
                   // || security.Code.Contains("MAGN")
                   // || security.Code.Contains("TGKA")
                   // || security.Code.Contains("FEES")
                   // || security.Code.Contains("MGNT")
                   // || security.Code.Contains("MTLR")
                   // || security.Code.Contains("OGKB")
                   // || security.Code.Contains("TGKA")
                   // || security.Code.Contains("YNDX")
                   // || security.Code.Contains("ALRS")
                ;
        }

        private void initConnections()
        {
            _dateTime = DateTime.Now.ToString("yyyy MM dd HH mm ss");
            _trader = new SmartTrader()
            {
                Login = "QF7R61QD",
                Password = "Y39HGY",
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
                SelectTrafficSourceDialog dialog = new SelectTrafficSourceDialog();
                dialog.TrafficSourсeFileSelectedEvent += fileName => playTraffic(new StreamReader("traffic\\"+fileName));
            }
        }

        private void playTraffic(StreamReader trafficSourceFile)
        {
            while (true)
            {
                string line = trafficSourceFile.ReadLine();
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
                    case "RestoredEventHandler":
                        RestoredEventHandler();
                        break;
                    case "ConnectedEventHandler":
                        ConnectedEventHandler();
                        break;
                    case "DisconnectedEventHandler":
                        DisconnectedEventHandler();
                        break;
                    case "ConnectionErrorEventHandler":
                        
                        break;
                    case "TransactionErrorEventHandler":

                        break;
                    case "MarketDataSubscriptionFailedEventHandler":

                        break;
                    case "newSecurityEventHandler":
                    
                        break;
                    case "MarketDepthChangedEventHandler":
                        
                        break;
                    case "MarketDepthsChangedEventHandler":
                        
                        break;
                    case "NewMyTradeEventHandler":
                        
                        break;
                    case "NewTradeEventHandler":
                        
                        break;
                    case "NewOrderEventHandler":
                        
                        break;
                    case "NewStopOrderEventHandler":
                        
                        break;
                    case "NewPortfolioEventHandler":
                        
                        NewPortfolioEventHandler(readPortfolioFromFile(jObject));
                        
                        break;
                    case "NewPositionEventHandler":
                        break;
                    case "OrderRegisterFailedEventHandler":
                        break;
                    case "OrderCancelFailedEventHandler":
                        break;
                    case "StopOrderRegisterFailedEventHandler":
                        break;
                    case "StopOrderCancelFailedEventHandler":
                        break;
                    case "MassOrderCancelFailedEventHandler":
                        break;
                    default:
                        break;
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
            // _trader.MarketDepthsChanged += changedMarketDepths =>
            //     this.GuiAsync(() => MarketDepthsChangedEventHandler(changedMarketDepths));
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

        private void NewSecurityEventHandler(Security security)
        {
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
            
            

            trafficEntitiesCounter.Text = (Int32.Parse(trafficEntitiesCounter.Text)+1).ToString();
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("trafficEventHandlerName");
            jsonWriter.WriteValue(trafficEventHandlerName);

            foreach (object objectForWrite in objectsForWrite)
            {
                jsonWriter.WritePropertyName(objectForWrite.GetType().ToString());
                using (MemoryStream memory_stream = new MemoryStream())
                {
                    // Serialize the object into the memory stream.
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memory_stream, objectForWrite);
                    byte[] array = memory_stream.ToArray();
                    string filename = "traffic\\"+DateTime.Now.Ticks;
                    BinaryWriter file = new BinaryWriter(new FileStream(filename,FileMode.Create));
                    file.Write(array);
                    file.Close();
                    jsonWriter.WriteValue(filename);

                }
            }
            jsonWriter.WriteEndObject();
            _trafficFile.Write(textWriter);
            _trafficFile.Write("\n");
            _trafficFile.Flush();
        }

        private  Portfolio readPortfolioFromFile(JObject jObject)
        {
            string filename = getFilenameOfObject(jObject, "StockSharp.BusinessEntities.Portfolio");
            object readedObject = getObjectFromFile(filename);
            return (Portfolio) readedObject;
        }

        private object getObjectFromFile(string filename)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryReader readFile = new BinaryReader(new FileStream(filename, FileMode.Open));
                byte[] readBytes = readFile.ReadBytes((int) new System.IO.FileInfo(filename).Length + 1);

                var memoryWriter = new BinaryWriter(memoryStream);
                memoryWriter.Write(readBytes);
                memoryWriter.Flush();
                memoryStream.Position = 0;
                var binaryFormatter = new BinaryFormatter();
                object deserializedObject = binaryFormatter.Deserialize(memoryStream);
                return deserializedObject;
            }
        }

        private static string getFilenameOfObject(JObject jObject, string objectName)
        {
            return jObject.Property(objectName).ToString().
                Replace("\"","").
                Replace(":","").
                Replace(" ","").
                Replace(objectName,"");
        }

        private readonly TrafficMode _trafficMode = TrafficMode.Write;
    }
}
