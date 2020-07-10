using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Alchemy;
using Alchemy.Classes;

//// State object for reading client data asynchronously  
//public class StateObject
//{
//    // Client  socket.  
//    public Socket workSocket = null;
//    // Size of receive buffer.  
//    public const int BufferSize = 1024;
//    // Receive buffer.  
//    public byte[] buffer = new byte[BufferSize];
//    // Received data string.  
//    public StringBuilder sb = new StringBuilder();
//}

namespace ScannerDemo
{
    public class AsynchronousSocketListener
    {
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
        public void OnConnect(UserContext context)
        {
            Console.WriteLine("Client Connection From : " + context.ToString());
        }
        public void OnReceive(UserContext context)
        {   
            if(context.DataFrame != null)
                mContext.log("Client Sent : " + context.DataFrame.ToString());
            if(context.DataFrame.ToString() == "scan")
            {
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
                    context.Send(theDocPath);
                    doWeHaveTheDocPath = false;
                    theDocPath = "";
                    isScannerBusy = false;
                    //scannerDone.Set();
                }
                else
                {
                    // The scanner is busy
                    context.Send("--Unsuccessfull: the program is busy--");
                    doWeHaveTheDocPath = false;
                }
            } else
            {
                context.Send("--Unsuccessfull: Something went wrong, please try again!--");
            }
        }
        public void StartListening()
        {
            var aServer = new WebSocketServer(10902, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };
            aServer.Start();

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                while (mRunning)
                {
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                mContext.log(e.ToString());
            }
        }

        private void OnDisconnect(UserContext context)
        {
            //throw new NotImplementedException();
            mContext.log("The user is disconnected");
        }

        private void OnSend(UserContext context)
        {
            //throw new NotImplementedException();
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

        public void setRunning(bool running)
        {
            mRunning = running;
        }
    }
}
