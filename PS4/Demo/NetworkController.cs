using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    class NetworkController
    {
        public delegate void ConnectionHandler(SocketState s);

        /// <summary>
        /// This class holds all the necessary state to handle a client connection
        /// Note that all of its fields are public because we are using it like a "struct"
        /// It is a simple collection of fields
        /// </summary>
        public class SocketState
        {
            public Socket theSocket;
            public int ID;

            // This is the buffer where we will receive message data from the client
            public byte[] messageBuffer = new byte[1024];

            // This is a larger (growable) buffer, in case a single receive does not contain the full message.
            public StringBuilder sb = new StringBuilder();

            public ConnectionHandler handleConnection;

            public SocketState(Socket s, int id)
            {
                theSocket = s;
                ID = id;
            }
        }

        public class TCPState
        {
            public TcpListener listener;
            public ConnectionHandler handler;
            private int clients = 0;

            public TCPState(TcpListener listener, ConnectionHandler handler)
            {
                this.listener = listener;
                this.handler = handler;
            }

            public int getNextID()
            {
                lock (this)
                {
                    return clients++;
                }

            }

        }

        public class Networking
        {
            public const int DEFAULT_PORT = 11000;




            public static SocketState ConnectToServer(ConnectionHandler handler, string hostName)
            {
                System.Diagnostics.Debug.WriteLine("connecting  to " + hostName);

                // Connect to a remote device.
                try
                {
                    // Establish the remote endpoint for the socket.
                    IPHostEntry ipHostInfo;
                    IPAddress ipAddress = IPAddress.None;

                    // Determine if the server address is a URL or an IP
                    try
                    {
                        ipHostInfo = Dns.GetHostEntry(hostName);
                        bool foundIPV4 = false;
                        foreach (IPAddress addr in ipHostInfo.AddressList)
                            if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                            {
                                foundIPV4 = true;
                                ipAddress = addr;
                                break;
                            }
                        // Didn't find any IPV4 addresses
                        if (!foundIPV4)
                        {
                            System.Diagnostics.Debug.WriteLine("Invalid addres: " + hostName);
                            return null;
                        }
                    }
                    catch (Exception e1)
                    {
                        // see if host name is actually an ipaddress, i.e., 155.99.123.456
                        System.Diagnostics.Debug.WriteLine("using IP");
                        ipAddress = IPAddress.Parse(hostName);
                    }

                    // Create a TCP/IP socket.
                    Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                    SocketState server = new SocketState(socket, -1);

                    server.handleConnection = handler;

                    server.theSocket.BeginConnect(ipAddress, Networking.DEFAULT_PORT, ConnectedCallback, server);

                    return server;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                    return null;
                }
            }

            private static void ConnectedCallback(IAsyncResult ar)
            {
                SocketState ss = (SocketState)ar.AsyncState;

                try
                {
                    // Complete the connection.
                    ss.theSocket.EndConnect(ar);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                    return;
                }

                ss.handleConnection(ss);

                ss.theSocket.BeginReceive(ss.messageBuffer, 0, ss.messageBuffer.Length, SocketFlags.None, ReceiveCallback, ss);

            }

            private static void ReceiveCallback(IAsyncResult ar)
            {
                SocketState ss = (SocketState)ar.AsyncState;
                try
                {
                    int bytesRead = 0;
                    bytesRead = ss.theSocket.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        string theMessage = Encoding.UTF8.GetString(ss.messageBuffer, 0, bytesRead);
                        // Append the received data to the growable buffer.
                        // It may be an incomplete message, so we need to start building it up piece by piece
                        ss.sb.Append(theMessage);

                        ss.handleConnection(ss);
                    }

                    //Continue the recieve loop once data is processed
                    ss.theSocket.BeginReceive(ss.messageBuffer, 0, ss.messageBuffer.Length, SocketFlags.None, ReceiveCallback, ss);
                }
                catch (SocketException)
                {
                    ss.theSocket.Close();
                    return;
                }
            }

            public static void GetData(SocketState state)
            {
                // Continue the "event loop"
                // Start listening for more parts of a message, or more new messages
                state.theSocket.BeginReceive(state.messageBuffer, 0, state.messageBuffer.Length, SocketFlags.None, ReceiveCallback, state);
            }

            public static void Send(Socket socket, string data)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(data);

                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
            }

            private static void SendCallback(IAsyncResult ar)
            {
                Socket ss = (Socket)ar.AsyncState;
                // Nothing much to do here, just conclude the send operation so the socket is happy.
                ss.EndSend(ar);
            }


            public static void ServerAwaitingClientLoop(ConnectionHandler callMe)
            {
                TcpListener listener = new TcpListener(IPAddress.Any, Networking.DEFAULT_PORT);

                listener.Start();

                listener.BeginAcceptSocket(AcceptNewClient, new TCPState(listener, callMe));
            }

            private static void AcceptNewClient(IAsyncResult ar)
            {
                Console.WriteLine("(Client Loop) Accepting new client");

                TCPState state = (TCPState)(ar.AsyncState);
                TcpListener listener = state.listener;

                // Get the socket
                Socket s = listener.EndAcceptSocket(ar);

                // Save the socket in a SocketState, 
                // so we can pass it to the receive callback, so we know which client we are dealing with.
                int nextID = state.getNextID();

                SocketState newClient = new SocketState(s, nextID);
                newClient.handleConnection = state.handler;

                Console.WriteLine("(Client Loop) Accepted clientID: " + nextID);

                // Start listening for a message
                // When a message arrives, handle it on a new thread with ReceiveCallback
                //                                  the buffer          buffer offset        max bytes to receive                         method to call when data arrives    "state" object representing the socket
                newClient.theSocket.BeginReceive(newClient.messageBuffer, 0, newClient.messageBuffer.Length, SocketFlags.None, ReceiveCallback, newClient);

                // Continue the "event loop" that was started on line 53
                // Start listening for the next client, on a new thread
                listener.BeginAcceptSocket(AcceptNewClient, state);
            }

        }

    }
}
