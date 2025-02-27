using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetPrototype.Enums;
using NetPrototype.Hardware.NetworkManagement.TCP;
using NetPrototype.Hardware.Processors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace NetPrototype.ViewModels.Pages
{
    /// <summary>
    /// TCP Client event page
    /// </summary>
    public partial class TCPClientViewModel : ObservableObject
    {
        /// <summary>
        /// Represents the client tcp utility.
        /// </summary>
        private readonly TCPClientUtility _tcpClientUtility;

        public TCPClientViewModel()
        {
            _tcpClientUtility = new TCPClientUtility();

            _tcpClientUtility.OnConnectionStatus(ConnectionHandler);
        }

        [RelayCommand]
        private void ToggleStartClient(object parameter)
        {
            if(_tcpClientUtility != null && _tcpClientUtility.ClientStatus == ConnectionState.Inactive)
            {
                if (parameter is object[] values)
                {
                    string ipAdressConn = values[0]?.ToString() ?? string.Empty;
                    string portConn = values[1]?.ToString() ?? string.Empty;

                    Debug.WriteLine($"IP Adress define: {ipAdressConn}");
                    Debug.WriteLine($"Port defined: {portConn}");

                    if (NetworkUtils.IsValidIPAddress(ipAdressConn))
                    {
                        _ = _tcpClientUtility.ConnectWithServerAsync(IPAddress.Parse(ipAdressConn), int.Parse(portConn));
                    }
                }
            }
            else
            {
                _tcpClientUtility?.StopClient();
            }
        }

        [RelayCommand]
        private void SendBufferToServer(object parameter)
        {
            if (_tcpClientUtility != null && _tcpClientUtility.ClientStatus == ConnectionState.Active)
            {
                // Get strem
                NetworkStream stream = _tcpClientUtility
                    .GetClient()
                    .GetStream();

                if(parameter is string msg)
                {
                    // Convertir el mensaje a bytes
                    byte[] data = Encoding.UTF8.GetBytes(msg);

                    _ = _tcpClientUtility.SendBufferToServer(BufferProccesorMethod.TextMessage, data);
                }
            }
        }

        private void ConnectionHandler(ConnectionState connSts)
        {
            switch (connSts)
            {
                case ConnectionState.Connecting:
                    Debug.WriteLine("Initializing connect with server.");
                   

                    break;
                case ConnectionState.Active:
                    Debug.WriteLine("The client has been connected with the server.");

                    break;
                case ConnectionState.Inactive:
                    Debug.WriteLine("The client has been disconected.");

                    break;
                default:
                    Debug.WriteLine("State not implemented.");
                    break;
            }
        }
    }
}
