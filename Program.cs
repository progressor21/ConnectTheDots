using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using SuperWebSocket;
using System.Text;
using log4net;
using Newtonsoft.Json;

namespace ConnectTheDots
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebSocketServer));

        private static WebSocketServer wsServer;
        //public MyHTTPServer wServer;

        static void Main(string[] args)
        {
            //MyHTTPServer wServer = new MyHTTPServer();
            //int retServer = wServer.Server();

        int _port = 8081;
            IPHostEntry _host = Dns.GetHostEntry("localhost");
            IPAddress _ipaddress = _host.AddressList[0];
            //IPAddress _ipaddress = IPAddress.Parse("127.0.0.1");

            wsServer = new WebSocketServer();

            try
            {
                wsServer.Setup(_port);
                wsServer.NewSessionConnected += NewSessionConnected;
                wsServer.SessionClosed += SessionClosed;
                wsServer.NewMessageReceived += NewMessageReceived;
                wsServer.Start();

                Console.WriteLine("Web Server is running (port:" + _port.ToString() + "). Waiting for a client connection to start a new session...");
                Console.WriteLine("Press any key to stop the game server and exit.");
                Console.ReadKey(true);

            }
            catch (Exception e)
            {
                //Logging the exeption 
                //log.Info("LogDate=" + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString("MM/dd/yyyy hh:mm:ssss") + " | ERROR: " + e.Message + " \nInner Exception: " + e.InnerException.Message);
                Console.WriteLine("ERROR: " + e.Message + " \nInner Exception: " + e.InnerException.Message);
                Console.ReadKey(true);
            }
            finally
            {
                Console.WriteLine("Session is closing/Server is stopping...");
                wsServer.Stop();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
            }
        }

        private static void NewMessageReceived(WebSocketSession session, string requesMessage)
        {
            Console.WriteLine("New Message received: " + requesMessage);

            try
            {
                MsgRequestPayload request = new MsgRequestPayload();
                request = JsonConvert.DeserializeObject<MsgRequestPayload>(requesMessage);

                MsgResponsePayload response = new MsgResponsePayload();
                response = GameLogic.HandleRequestMessage(request);

                string responseMessage = JsonConvert.SerializeObject(response);
                session.Send(responseMessage);

                Console.WriteLine("New Message sent: " + responseMessage);
            }
            catch (Exception e)
            {
                //Logging the exception
                Console.WriteLine("ERROR: " + e.Message);
                
            }

        }

        private static void NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("New Session connected: " + session.SessionID + " Method: " + session.Method);
        }

        private static void SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("Session closed: " + value.ToString());
        }

        //public override void handleGETRequest(HttpProcessor p)
        //{ 
        //}
        //public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        //{ 
        //}
    }
}