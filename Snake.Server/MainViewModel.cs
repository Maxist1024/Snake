using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Snake.Server
{
    public class MainViewModel : ViewModelBase
    {
        SocketPermission permission;
        Socket sListener;
        IPEndPoint ipEndPoint;
        Socket handler;

        private string _valueTextBox;
        private bool _start_ButtonEnable = true;
        private bool _startListen_ButtonEnable = false;
        private bool _send_ButtonEnable;
        private bool _close_ButtonEnable = false;


        public ICommand StartServerCommand { get; }
        public ICommand ListenServerCommand { get; }
        public ICommand CloseServerCommand { get; }

        public string ValueTextBox
        {
            get { return _valueTextBox; }
            set
            {
                if (value != this._valueTextBox)
                {
                    _valueTextBox = value; OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            StartServerCommand = new DelegateCommand(OnStartServerExecute, CanStartServer);
            ListenServerCommand = new DelegateCommand(OnListenServerExecute, CanListenServer);
            CloseServerCommand = new DelegateCommand(OnCloseServerExecute, CanCloseServer);
            ValueTextBox = "Not connected yet !";
        }

        public void SetButtonsEnable()
        {
            _start_ButtonEnable = true;
            _startListen_ButtonEnable = false;
            _send_ButtonEnable = false;
            _close_ButtonEnable = false;
            ((DelegateCommand)StartServerCommand).RaiseCanExecuteChanged();
        }

        private void OnCloseServerExecute()
        {
            try
            {
                if (sListener.Connected)
                {
                    sListener.Shutdown(SocketShutdown.Receive);
                    sListener.Close();
                }
                ValueTextBox = "Connection has been closed. To connect open again, must run server once again.";
                _close_ButtonEnable = false;
                ((DelegateCommand)CloseServerCommand).RaiseCanExecuteChanged();
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }

        private bool CanCloseServer()
        {
            return _close_ButtonEnable;
        }

        private void OnListenServerExecute()
        {
            try
            {
                // Places a Socket in a listening state and specifies the maximum 
                // Length of the pending connections queue 
                sListener.Listen(10);

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                sListener.BeginAccept(aCallback, sListener);

                ValueTextBox = "Server is now listening on " + ipEndPoint.Address + " port: " + ipEndPoint.Port;

                _startListen_ButtonEnable = false;
                ((DelegateCommand)ListenServerCommand).RaiseCanExecuteChanged();
                //Send_ButtonEnable = true;
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }

        private bool CanListenServer()
        {
            return _startListen_ButtonEnable;
        }

        private void OnStartServerExecute()
        {
            try
            {
                // Creates one SocketPermission object for access restrictions
                permission = new SocketPermission(
                NetworkAccess.Accept,     // Allowed to accept connections 
                TransportType.Tcp,        // Defines transport types 
                "",                       // The IP addresses of local host 
                SocketPermission.AllPorts // Specifies all ports 
                );

                // Listening Socket object 
                sListener = null;

                // Ensures the code to have permission to access a Socket 
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance 
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost 
                IPAddress ipAddr = ipHost.AddressList[0];

                // Creates a network endpoint 
                ipEndPoint = new IPEndPoint(ipAddr, 4510);

                // Create one Socket object to listen the incoming connection 
                sListener = new Socket(
                    ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );

                // Associates a Socket with a local endpoint 
                sListener.Bind(ipEndPoint);

                ValueTextBox = "Server started.";

                _start_ButtonEnable = false;
                _startListen_ButtonEnable = true;
                _close_ButtonEnable = true;
                ((DelegateCommand)StartServerCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)ListenServerCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)CloseServerCommand).RaiseCanExecuteChanged();
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }

        private bool CanStartServer()
        {
            return _start_ButtonEnable;
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = null;

            // A new Socket to handle remote host communication 
            Socket handler = null;
            try
            {
                // Receiving byte array 
                byte[] buffer = new byte[1024];
                // Get Listening Socket object 
                listener = (Socket)ar.AsyncState;
                // Create a new socket 
                handler = listener.EndAccept(ar);

                // Using the Nagle algorithm 
                handler.NoDelay = false;

                // Creates one object array for passing data 
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                // Begins to asynchronously receive data 
                handler.BeginReceive(
                    buffer,        // An array of type Byt for received data 
                    0,             // The zero-based position in the buffer  
                    buffer.Length, // The number of bytes to receive 
                    SocketFlags.None,// Specifies send and receive behaviors 
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate 
                    obj            // Specifies infomation for receive operation 
                    );

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Fetch a user-defined object that contains information 
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                // Received byte array 
                byte[] buffer = (byte[])obj[0];

                // A Socket to handle remote host communication. 
                handler = (Socket)obj[1];

                // Received message 
                string content = string.Empty;


                // The number of bytes received. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.Unicode.GetString(buffer, 0,
                        bytesRead);

                    // If message contains "<Client Quit>", finish receiving
                    if (content.IndexOf("<Client Quit>") > -1)
                    {
                        // Convert byte array to string
                        string str = content.Substring(0, content.LastIndexOf("<Client Quit>"));

                        //this is used because the UI couldn't be accessed from an external Thread
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                        {
                            //      ValueTextBox = "Read " + str.Length * 2 + " bytes from client.\n Data: " + str;
                        }
                        );
                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] buffernew = new byte[1024];
                        obj[0] = buffernew;
                        obj[1] = handler;
                        handler.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        // ValueTextBox = content;
                    }
                    );
                }
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }

    }
}
