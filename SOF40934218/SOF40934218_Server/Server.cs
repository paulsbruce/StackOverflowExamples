using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//in response to http://stackoverflow.com/questions/40934218/second-call-to-httpclients-postasjsonasync-stucks

namespace SOF40934218_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            using (var server = new HTTPServer(18080))
            {
                server.StartListener();

                while (server.IsRunning)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }

    public class HTTPServer : IDisposable
    {
        public int Port { get; private set; }
        public HTTPServer (int port)
        {
            this.Port = port;
        }
        ~HTTPServer()
        {
            this.Dispose();
        }


        HttpListener _listener = new HttpListener();

        public void StartListener()
        {
            _listener.Prefixes.Add("http://127.0.0.1:"+this.Port+"/");
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(OnRequestReceive), _listener);
        }

        public bool IsRunning { get { return _listener.IsListening; } }

        private void OnRequestReceive(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;

            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerResponse response = context.Response;
            HttpListenerRequest request = context.Request;

            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                Console.WriteLine(reader.ReadToEnd());
            }

            string responseString = "{'a': \"b\"}";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);

            listener.BeginGetContext(new AsyncCallback(OnRequestReceive), listener);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _listener.Stop();
                    _listener = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HTTPServer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
