using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Org.BouncyCastle.Crypto.Engines;


// State object for reading client data asynchronously  
public class StateObject
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

namespace ScannerDemo
{
    public class AsynchronousSocketListener
    {
        // Thread signal.  
        public ManualResetEvent allDone = new ManualResetEvent(false);
        public ManualResetEvent scannerDone = new ManualResetEvent(false);
        private bool mRunning = true;
        private bool isScannerBusy = false;
        private Form1 form;
        private volatile bool doWeHaveTheDocPath = false;
        private string theDocPath;
        private MyScannerService mContext;
        private bool theFormIsClosed = true;

        public AsynchronousSocketListener(MyScannerService context)
        {
            mContext = context;
            mContext.log("Started");
        }

        public void StartListening()
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            //IPHostEntry ipHostInfo =
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 10902);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            byte[] bytes = new byte[128];

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (mRunning)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    mContext.log("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                mContext.log(e.ToString());
            }

        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();
            mContext.log("AcceptCallback");

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
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

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.  

                    string tmp = "Read" + content.Length + "bytes from socket. \n Data : " + content;
                    mContext.log(tmp);

                    if (!isScannerBusy)
                    {
                        //scannerDone.WaitOne();
                        isScannerBusy = true;
                        Thread _thread = new Thread(setUpScanner);
                        _thread.SetApartmentState(ApartmentState.STA);
                        _thread.Name = "Scanning thread";
                        _thread.Start();

                        while (!doWeHaveTheDocPath)
                        {
                            Thread.Sleep(300);
                            mContext.log("waiting");
                        }

                        if (!theFormIsClosed)
                        {
                            form.Invoke(form.myDelegate);
                        }
                        _thread.Join();
                        theFormIsClosed = true;

                        Send(handler, theDocPath);
                        doWeHaveTheDocPath = false;
                        theDocPath = "";
                        isScannerBusy = false;
                        //scannerDone.Set();
                    }
                    else
                    {
                        // The scanner is busy
                        Send(handler, "--Unsuccessfull: the program is busy--");
                        doWeHaveTheDocPath = false;
                    }
                }
                else
                {
                    // Not all data received. Get more.  
                    try
                    {
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                    catch (SocketException se)
                    {

                    }
                }
            }
        }

        internal void setDocumentPath(string path)
        {
            theDocPath = path;
            doWeHaveTheDocPath = true;
        }

        private void setUpScanner()
        {
            form = new Form1();
            form.SetContext(this);
            theFormIsClosed = false;
            Application.Run(form);
            if (!doWeHaveTheDocPath)
            {
                theDocPath = "--Unsuccessfull: Scanning has been terminated--";
                doWeHaveTheDocPath = true;
                theFormIsClosed = true;
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            try
            {
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }
            catch (SocketException se)
            {
                doWeHaveTheDocPath = true;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                string tmp = "Sent" + bytesSent + "bytes to client.";
                mContext.log(tmp);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                mContext.log(e.ToString());
            }
        }

        public void setRunning(bool running)
        {
            mRunning = running;
        }
    }
}
