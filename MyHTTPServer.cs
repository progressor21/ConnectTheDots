using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ConnectTheDots
{
    public class MyHTTPServer
    {
        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public int Server()
        {
            int _port = 8080;
            IPHostEntry _host = Dns.GetHostEntry("localhost");
            IPAddress _ipaddress = _host.AddressList[0];
            //IPAddress _ipaddress = IPAddress.Parse("127.0.0.1");

            StartServer(_ipaddress, _port);
            return 0;

            //HttpServer httpServer = new MyHttpServer(_ipaddress, _port);
            //Thread thread = new Thread(new ThreadStart(httpServer.listen));
            //thread.Start();
            //Console.WriteLine("Server is running (Port:" + _port + ")... Press any key to exit.");
            //Console.ReadKey(true);
        }

        public void StartServer(IPAddress iPAddress, int port)
        {
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses  

            //IPHostEntry host = Dns.GetHostEntry("localhost");
            //IPAddress ipAddress = host.AddressList[0];

            IPEndPoint localEndPoint = new IPEndPoint(iPAddress, port);

            try
            {
                // Create a Socket that will use Tcp protocol      
                Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method  
                listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.  
                // We will listen 10 requests at a time  
                listener.Listen(2);

                //Console.WriteLine("Waiting for a connection...");
                //Socket handler = listener.Accept();

                // Incoming data from the client.    
                string data = null;
                byte[] bytes = null;

                bytes = new byte[1024];

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("GET") > -1 || data.IndexOf("POST") > -1)
                        {
                            break;
                        }
                    }

                    Process(data);

                    // Show the data on the console.  
                    Console.WriteLine("Text received: {0}", data);



                    // Echo the data back to the client.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                //int bytesRec = handler.Receive(bytes);
                //data += Encoding.ASCII.GetString(bytes, 0, bytesRec);


                //while (true)
                //{
                //    bytes = new byte[1024];
                //    int bytesRec = handler.Receive(bytes);
                //    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                //    if (data.IndexOf("GET") > -1)
                //    {
                //        break;
                //    }
                //}

                //Console.WriteLine("Text received : {0}", data);
                //byte[] msg = Encoding.ASCII.GetBytes(data);
                //handler.Send(msg);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }

        public void Process(string data)
        {
            //inputStream = new BufferedStream(socket.GetStream());
            //outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));

            try
            {
                parseRequest(data);
                readHeaders(data);
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                //writeFailure();
            }
            //outputStream.Flush();

            //inputStream = null; outputStream = null;
            //socket.Close();
        }

        public void parseRequest(string data)
        {
            //String request = streamReadLine(inputStream);
            String request = data;
            
            string[] tokens = request.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string[] headerTokens = null;

            if (tokens.Length > 1)
                headerTokens = tokens[0].Split(' ');


            if (headerTokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = headerTokens[0].ToUpper();
            http_url = headerTokens[1];
            http_protocol_versionstring = headerTokens[2];

            Console.WriteLine("starting: " + request);
        }

        public void readHeaders(string data)
        {
            Console.WriteLine("readHeaders()");
            String line;
            //while ((line = streamReadLine(inputStream)) != null)
            while ((line = data) != null)
                {
                    if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++;
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            //srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {

            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);

                    //int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    int numread = 369;
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            //srv.handlePOSTRequest(this, new StreamReader(ms));
        }
    }

    //public class MyHttpServer : HttpServer
    //    {
    //        public MyHttpServer(IPAddress iPAddress, int port)
    //            : base(iPAddress, port)
    //        {
    //        }
    //        public override void handleGETRequest(HttpProcessor p)
    //        {
    //            Console.WriteLine("request: {0}", p.http_url);
    //            p.writeSuccess();
    //            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
    //            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
    //            p.outputStream.WriteLine("url : {0}", p.http_url);

    //            p.outputStream.WriteLine("<form method=post action=/form>");
    //            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
    //            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
    //            p.outputStream.WriteLine("</form>");
    //        }

    //        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
    //        {
    //            Console.WriteLine("POST request: {0}", p.http_url);
    //            string data = inputData.ReadToEnd();

    //            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
    //            p.outputStream.WriteLine("<a href=/test>return</a><p>");
    //            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
    //        }
    //    }
    //    public abstract class HttpServer
    //    {
    //        protected int port;
    //        protected IPAddress ipAddress;
    //        TcpListener listener;
    //        bool is_active = true;

    //        public HttpServer(IPAddress ipAddress, int port)
    //        {
    //            this.ipAddress = ipAddress;
    //            this.port = port;
    //        }

    //        public void listen()
    //        {
    //            listener = new TcpListener(ipAddress, port);
    //            listener.Start();
    //            while (is_active)
    //            {
    //                TcpClient s = listener.AcceptTcpClient();
    //                HttpProcessor processor = new HttpProcessor(s, this);
    //                Thread thread = new Thread(new ThreadStart(processor.process));
    //                thread.Start();
    //                Thread.Sleep(1);
    //            }
    //        }

    //        public abstract void handleGETRequest(HttpProcessor p);
    //        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    //    }
    //}

}
