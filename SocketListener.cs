using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace client
{
    public class StateObject // State object for reading client data asynchronously
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class AsynchronousSocketListener
    {
        public static ISubject<BaseStationChange> bsChange;
        public static ISubject<SubscribersChange> subChange;
        public static ISubject<GeolocationChange> geoChange;
        public static ISubject<TAChange> taChange;
        public static ISubject<CellIDChange> cellidChange;
        public static ISubject<LogChange> logChange;

        public static volatile bool listening = true;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            bsChange = new Subject<BaseStationChange>();
            subChange = new Subject<SubscribersChange>();
            geoChange = new Subject<GeolocationChange>();
            taChange = new Subject<TAChange>();
            cellidChange = new Subject<CellIDChange>();
            logChange = new Subject<LogChange>();
            subChange.Where(c => c.AddRemove == "+").Subscribe(MainWindow.AddNewSub);
            subChange.Where(c => c.AddRemove == "-").Subscribe(MainWindow.RemoveSub);
            bsChange.Where(c => c.AddRemove == "+").Subscribe(MainWindow.AddNewBS);
            bsChange.Where(c => c.AddRemove == "-").Subscribe(MainWindow.RemoveBS);
            bsChange.Where(c => c.AddRemove == "+-").Subscribe(MainWindow.AlterBS);
            bsChange.Where(c => c.AddRemove == "-all").Subscribe(MainWindow.RemoveAllBS);
            geoChange.Subscribe(MainWindow.AddNewGeo);
            taChange.Subscribe(MainWindow.AddNewTA);
            cellidChange.Subscribe(MainWindow.AddNewCellID);
            logChange.Subscribe(MainWindow.AddNewLog);

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8081);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (listening)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                logChange.OnNext(new LogChange() { NewLog = new LogUnit(e.Message) });
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));
                try
                {
                    content = state.sb.ToString();
                    Subscriber sub;
                    BaseStation bs;
                    switch (content[0])
                    {
                        case '0':
                            bs = BaseStation.Deserialize(content.Substring(1));
                            Client.client = new TcpClient(bs.ip, Int32.Parse(bs.port));
                            Client.stream = Client.client.GetStream();
                            break;
                        case '1':
                            switch (content[1])
                            {
                                case '1':
                                    bs = BaseStation.Deserialize(content.Substring(2));
                                    bsChange.OnNext(new BaseStationChange() { AddRemove = "+", NewBS = bs });
                                    break;
                                case '2':
                                    bs = BaseStation.Deserialize(content.Substring(2));
                                    bsChange.OnNext(new BaseStationChange() { AddRemove = "-", NewBS = bs });
                                    break;
                                case '3':
                                    bs = BaseStation.Deserialize(content.Substring(2));
                                    bsChange.OnNext(new BaseStationChange() { AddRemove = "+-", NewBS = bs });
                                    break;
                            }
                            break;
                        case '2':
                            switch (content[1])
                            {
                                case '1':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    sub.ParseClassmark();
                                    subChange.OnNext(new SubscribersChange() { AddRemove = "+", NewSub = sub });
                                    break;
                                case '2':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    subChange.OnNext(new SubscribersChange() { AddRemove = "-", NewSub = sub });
                                    break;
                            }
                            break;
                        case '3':
                            switch (content[1])
                            {
                                case '1':
                                    TA ta = TA.Deserialize(content.Substring(2));
                                    taChange.OnNext(new TAChange() { NewTA = ta });
                                    break;
                                case '2':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    geoChange.OnNext(new GeolocationChange() { NewGeo = SplitAssist.GetGeolocation(sub) });
                                    break;
                                case '3':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    string res;
                                    using (var client = new WebClient())
                                    {
                                        res = client.DownloadString(@"http://192.168.70.132/cgi-bin/rrlpserver.cgi?query=decode&apdu=" + sub.assistData);
                                        //res = client.DownloadString(@"http://192.168.70.132/cgi-bin/rrlpserver.cgi?query=apdu&apdu=" + sub.assistData);
                                    }
                                    logChange.OnNext(new LogChange()
                                    {
                                        NewLog = new LogUnit(String.Format(
                                        "Получены измерения местоположения для абонента: IMSI = {0}, IMEI-SV = {1} - {2}", sub.imsi, sub.imeiSV, res))
                                    });
                                    break;
                                case '4':
                                    CellID cellid = CellID.Deserialize(content.Substring(2));
                                    cellidChange.OnNext(new CellIDChange() { NewCellID = cellid });
                                    break;
                            }
                            break;
                        case '4':
                            bs = BaseStation.Deserialize(content.Substring(1));
                            bsChange.OnNext(new BaseStationChange() { AddRemove = "-all", NewBS = bs });
                            break;
                        case '9':
                            switch (content[1])
                            {
                                case '0':
                                    break;
                                case '1':
                                    break;
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    logChange.OnNext(new LogChange() { NewLog = new LogUnit(e.Message) });
                    Send(handler, "91");
                }
                Send(handler, "90");
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                logChange.OnNext(new LogChange() { NewLog = new LogUnit(e.Message) });
            }
        }

        public static void StopListening()
        {
            listening = false;
            allDone.Set();
        }
    }

}