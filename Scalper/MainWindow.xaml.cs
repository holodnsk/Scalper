using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using ddfplus;
using Ecng.Common;
using Ecng.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scalper.Strategies;
using StockSharp.Algo;
using StockSharp.BusinessEntities;
using StockSharp.Messages;
using StockSharp.SmartCom;
using JsonSerializer = RestSharp.Serializers.JsonSerializer;

namespace Scalper
{
    public partial class MainWindow 
    {
        public MainWindow()
        {
            
            InitializeComponent();
            InitConnections();
        }

        // private void ShowLogMessage(String message)
        // {
        //     systemLog.Text = systemLog.Text + "\n" + message;
        //
        // }
        
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

        private void InitConnections()
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
                dialog.TrafficSourсeFileSelectedEvent += fileName => PlayTraffic(new StreamReader("traffic\\"+fileName));
            }
        }

        private void SubscribeEvents()
        {
            _trader.ReConnectionSettings.WorkingTime = ExchangeBoard.Forts.WorkingTime;
            // _trader.NewMessage +=  message => this.GuiAsync(NewMessageEventHandler(message));
            _trader.OutMessageChannel.NewOutMessage +=  message => this.GuiAsync(NewMessageEventHandler(message));
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
            _trader.NewOrderLogItem += newOrderLogItem => this.GuiAsync(()=> NewOrderLogItemHandler(newOrderLogItem));
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

        private Action NewMessageEventHandler(Message message)
        {
            WriteTraffic(message);
            return null;
        }

        private void NewOrderLogItemHandler(OrderLogItem newOrderLogItem)
        {
            //WriteTraffic(MethodBase.GetCurrentMethod().Name, newOrderLogItem);
        }

        private void NewMyTradeEventHandler(MyTrade newMyTrade)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, newMyTrade);
        }

        private void MarketDataSubscriptionFailedEventHandler(Security security, MarketDataMessage msg, Exception error)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, security, msg,
                // error);
        }

        private void DisconnectedEventHandler()
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name);
        }

        private void MassOrderCancelFailedEventHandler(long transId, Exception error)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, transId, error);
        }

        private void StopOrderCancelFailedEventHandler(OrderFail stopOrderCancelFailed)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name,
                // stopOrderCancelFailed);
        }

        private void StopOrderRegisterFailedEventHandler(OrderFail stopOrderRegisterFailed)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name,
                // stopOrderRegisterFailed);
        }

        private void OrderCancelFailedEventHandler(OrderFail orderCancelFailed)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, orderCancelFailed);
        }

        private void OrderRegisterFailedEventHandler(OrderFail orderRegisterFailed)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, orderRegisterFailed);
        }

        private void NewPositionEventHandler(Position newPosition)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, newPosition);
        }

        private void NewPortfolioEventHandler(Portfolio newPortfolio)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, newPortfolio);
        }

        private void NewStopOrderEventHandler(Order newStopOrder)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, newStopOrder);
        }

        private void NewOrderEventHandler(Order newOrder)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, newOrder);
        }

        private void NewTradeEventHandler(Trade newTrade)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, newTrade);
        }

        private void RestoredEventHandler()
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name);
        }

        private void ConnectionErrorEventHandler(Exception error)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, error);
        }

        private void ConnectedEventHandler()
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name);
        }

        private void TransactionErrorEventHandler(Exception error)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, error);
        }

        private void MarketDepthsChangedEventHandler(IEnumerable<MarketDepth> changedMarketDepths)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, changedMarketDepths);
        }

        private void MarketDepthChangedEventHandler(MarketDepth changedMarketDepth)
        {
            // WriteTraffic(MethodBase.GetCurrentMethod().Name, changedMarketDepth);
            if (changedMarketDepth.Security.Name=="AFLT")
            {
                AFLT.NewMarketDepth(changedMarketDepth);
            }
        }

        private void NewSecurityEventHandler(Security security)
        {
            if (CfgTradingSecurity(security))
            {
                // WriteTraffic(MethodBase.GetCurrentMethod().Name, security);
                _trader.RegisterMarketDepth(security);
                _trader.RegisterTrades(security);
                _trader.RegisterOrderLog(security);
            }
        }
        private void PlayTraffic(StreamReader trafficSourceFile)
        {
            while (true)
            {
                string line = trafficSourceFile.ReadLine();
                if (line == null)
                    break;

                JObject jObject = JObject.Parse(line);

                var messageTypes = typeof(ConnectMessage).Assembly.GetTypes().Where((t) => t.FullName == jObject.GetValue("ObjectType").ToString());
                foreach (Type messageType in messageTypes)
                {
                    string objectToDeserialize = jObject.GetValue("message").ToString();
                    
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        BinaryReader readFile = new BinaryReader(new FileStream(objectToDeserialize, FileMode.Open));
                        byte[] readBytes = readFile.ReadBytes((int) new FileInfo(objectToDeserialize).Length + 1);

                        var memoryWriter = new BinaryWriter(memoryStream);
                        memoryWriter.Write(readBytes);
                        memoryWriter.Flush();
                        memoryStream.Position = 0;
                        var binaryFormatter = new BinaryFormatter();
                        object deserializedObject = binaryFormatter.Deserialize(memoryStream);
                        
                    }
                    
                    Message deserializeObject = (Message) JsonConvert.DeserializeObject(objectToDeserialize,messageType);
                    _trader.SendInMessage(deserializeObject);                    
                }
                
            }
        }

        private object[] GetParametersForMethodFromFile(MethodInfo methodInfo, JObject jObject)
        {
            ParameterInfo[] parametersFromMethodInfo = methodInfo.GetParameters();
            object[] resultParameters = new object[parametersFromMethodInfo.Length];
            int countParameter = 0;
            foreach (var parameter in parametersFromMethodInfo)
            {
                string objectType = parameter.ToString().Split(" ")[0];
                object objectFromFile = readObjectFromFile(jObject, objectType);
                resultParameters[countParameter] = objectFromFile;
                countParameter++;
            }

            return resultParameters;
        }

        private object readObjectFromFile(JObject jObject, string objectType)
        {
            string filename = jObject.Property(objectType).ToString().
                Replace("\"","").
                Replace(":","").
                Replace(" ","").
                Replace(objectType,"");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryReader readFile = new BinaryReader(new FileStream(filename, FileMode.Open));
                byte[] readBytes = readFile.ReadBytes((int) new FileInfo(filename).Length + 1);

                var memoryWriter = new BinaryWriter(memoryStream);
                memoryWriter.Write(readBytes);
                memoryWriter.Flush();
                memoryStream.Position = 0;
                var binaryFormatter = new BinaryFormatter();
                object deserializedObject = binaryFormatter.Deserialize(memoryStream);
                return deserializedObject;
            }
        }

        private void WriteTraffic(Message message)
        {
            if (_trafficMode != TrafficMode.Write)
                return;

            string serializeObject = new JsonSerializer().Serialize(message.Clone());
            
            if (message.Type==MessageTypes.Time)
            {
                return;
            }
            if (serializeObject.Contains("SecurityCode"))
            {
                if (!serializeObject.Contains("AFLT"))
                {
                    return;
                }
            }
            
            //trafficEntitiesCounter.Text = (Int32.Parse(trafficEntitiesCounter.Text)+1).ToString();
            TextWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("ObjectType");
            jsonWriter.WriteValue(message.GetType().ToString());
            jsonWriter.WritePropertyName("message");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, serializeObject);
                byte[] array = memoryStream.ToArray();
                string filename = "traffic\\"+DateTime.Now.Ticks;
                BinaryWriter file = new BinaryWriter(new FileStream(filename,FileMode.Create));
                file.Write(array);
                file.Close();
                jsonWriter.WriteValue(filename);
            }

            jsonWriter.WriteEndObject();
            _trafficFile.Write(textWriter);
            _trafficFile.Write("\n");
            _trafficFile.Flush();
        }

        private readonly TrafficMode _trafficMode = TrafficMode.Read;
    }
}
