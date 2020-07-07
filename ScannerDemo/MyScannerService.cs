
using System.Diagnostics;
using System.Threading;

namespace ScannerDemo
{
    public class MyScannerService
    {
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _thread;


        internal MyScannerService()
        {
            log("STARTEDSTARTEDSTARTEDSTARTEDSTARTEDSTARTED");
            _thread = new Thread(WorkerThreadFunc);
            _thread.Name = "My Worker Thread";
            _thread.IsBackground = true;
        }

        public Thread getThread()
        {
            return _thread;
        }

        internal void startListening()
        {
            _thread.Start();
        }

        private void WorkerThreadFunc()
        {
            AsynchronousSocketListener socketListener = new AsynchronousSocketListener(this);
            socketListener.StartListening();
        }

        internal void OnStop()
        {
            _shutdownEvent.Set();
            if (!_thread.Join(3000))
            { // give the thread 3 seconds to stop
                _thread.Abort();
            }
        }

        internal void log(string v)
        {
            Trace.WriteLine(v);
        }
    }
}
