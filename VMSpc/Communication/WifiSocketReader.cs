using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using VMSpc.Loggers;
using System.Windows;

namespace VMSpc.Communication
{
    class WifiSocketReader : DataReader
    {
        Socket socket;
        IPEndPoint ipEndPoint;
        Thread requestThread;
        private bool requestThreadShouldStop;
        public WifiSocketReader(string ipAddress, int port)
            : base()
        {
            var ip = IPAddress.Parse(ipAddress);
            ipEndPoint = new IPEndPoint(ip, port);
            socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        ~WifiSocketReader()
        {
            CloseDataReader();
        }

        public override void InitDataReader()
        {
            ConnectSocket();
            base.InitDataReader();
        }

        protected override void KeepJibAwake()
        {
            try
            {
                SendMessage("V\r\n");
            }
            catch (Exception e)
            {
                if (socket == null || !socket.Connected)
                {
                    keepJibAwakeTimer?.Dispose();
                }
            }
        }

        public override void CloseDataReader()
        {
            TerminateRequestThread();
            DisconnectSocket();
        }

        private void ActivateRequestThread()
        {
            TerminateRequestThread();
            requestThreadShouldStop = false;
            requestThread = new Thread(new ThreadStart(RequestData));
            requestThread.Start();
        }

        private void TerminateRequestThread()
        {
            requestThreadShouldStop = true;
            requestThread?.Join();
            if (requestThread != null)
            {
                if (requestThread.IsAlive)
                {
                    requestThread.Abort();
                }
            }
            requestThread = null;
        }

        private void ConnectSocket()
        {
            socket.Connect(ipEndPoint);
            if (socket.Connected)
            {
                ActivateRequestThread();
            }
        }

        private void DisconnectSocket()
        {
            socket?.Shutdown(SocketShutdown.Both);
            socket?.Close();
        }

        /// <summary>
        /// RequestData() operates in a separate thread. requestThreadShouldStop is controlled by the main thread. Setting it
        /// to true will force this thread to terminate. Otherwise, it runs indefinitely.
        /// packets.
        /// </summary>
        private void RequestData()
        {
            while (!requestThreadShouldStop)
            {
                var data = RequestData(4096);
                if (data != null)
                {
                    HandleWifiData(data);
                }
            }
            requestThreadShouldStop = false;
        }

        private string RequestData(int bufferSize)
        {
            byte[] bytes = new byte[bufferSize];
            string data = null;
            int numBytes;
            if (socket != null && socket.Connected)
            {
                try
                {
                    numBytes = socket.Receive(bytes);
                    if (numBytes > 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, numBytes);
                        return data;
                    }
                }
                catch (Exception e)
                {
                    //check if the socket was disconnected in the middle of the socket.Receive() call
                    if (socket == null || !socket.Connected)
                    {
                        CloseDataReader();
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            return null;
        }

        private void HandleWifiData(string data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                //break the buffer into an array of messages and process them individually.
                //each valid, individual message in the buffer ends in a newline character
                try
                {
                    string[] buffer = data.Split('\n');
                    if (buffer != null)
                    {
                        foreach (string message in buffer)
                        {
                            messagesReceived++;
                            OnDataReceived(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.GenerateErrorRecord(ex);
                }
            });
        }


        public override void SendMessage(string message)
        {
            byte[] bytesSent = Encoding.ASCII.GetBytes(message);
            socket.Send(bytesSent, bytesSent.Length, SocketFlags.None);
        }

        public override void SendMessage(OutgoingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
