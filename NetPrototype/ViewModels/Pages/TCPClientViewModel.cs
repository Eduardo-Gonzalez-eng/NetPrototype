using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetPrototype.Enums;
using NetPrototype.Hardware.NetworkManagement.TCP;
using NetPrototype.Hardware.Processors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

using static NetPrototype.Hardware.Processors.NetworkBufferProcessorUtility;

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

        /// <summary>
        /// Represents the server port. 
        /// </summary>
        [ObservableProperty]
        private string _messageClientStatus;


        /// <summary>
        /// Represents the TextBox of the clients messages.
        /// </summary>
        [ObservableProperty]
        private string _messageFromServer;

        /// <summary>
        /// Represents the method collections for show to user. Asociated directly with <see cref="_selectedBufferProcessorMethod"/>
        /// </summary>
        public ObservableCollection<string> MethodProcess { get; } = ["Mensaje de texto"];

        /// <summary>
        /// Represents the method selected by user. Asociated directly with <see cref="MethodProcess"/>.
        /// </summary>
        [ObservableProperty]
        private BufferProccesorMethod? _selectedBufferProcessorMethod;

        /// <summary>
        /// Represents the TextBlock of Connect button;
        /// </summary>
        [ObservableProperty]
        private string _textButtonClient;

        /// <summary>
        /// Represents the dinamic color for inner textblock.
        /// </summary>
        [ObservableProperty]
        private bool _inOperation;

        /// <summary>
        /// Represents the dinamic color for send button.
        /// </summary>
        [ObservableProperty]
        private bool _connectedToServer;

        public TCPClientViewModel()
        {
            _tcpClientUtility = new TCPClientUtility();

            // Initialize all suscriptions.
            _tcpClientUtility
                .OnConnectionStatus(ConnectionHandler)
                .OnTransaction(TransactionHandler);

            // Default status.
            SelectedBufferProcessorMethod = BufferProccesorMethod.TextMessage;
            InOperation = false;
            ConnectedToServer = false;
            TextButtonClient = "Conectar";
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
                TextButtonClient = "Conectar";
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
                    switch (SelectedBufferProcessorMethod ?? BufferProccesorMethod.Disable)
                    {
                        case BufferProccesorMethod.TextMessage:
                            Debug.WriteLine($"Message method.");
                            // Convertir el mensaje a bytes
                            byte[] data = Encoding.UTF8.GetBytes(msg);

                            _ = _tcpClientUtility.SendBufferToServer(BufferProccesorMethod.TextMessage, data);

                            break;
                        default:
                            Debug.WriteLine($"Method not defined.");
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Debes conectar un servidor.");
            }
        }

        private void ConnectionHandler(ConnectionState connSts)
        {
            switch (connSts)
            {
                case ConnectionState.Connecting:
                    Debug.WriteLine("Initializing connect with server.");
                    MessageClientStatus = "Conectando con servidor...";
                    InOperation = true;

                    break;
                case ConnectionState.Active:
                    Debug.WriteLine("The client has been connected with the server.");

                    MessageClientStatus = "Conectado al servidor.";

                    TextButtonClient = "Desconectar";
                    InOperation = false;
                    ConnectedToServer = true;

                    break;
                case ConnectionState.Inactive:
                    Debug.WriteLine("The client has been disconected.");

                    MessageClientStatus = "Cliente desconectado";

                    InOperation = false;
                    ConnectedToServer = false;
                    break;
                default:
                    Debug.WriteLine("State not implemented.");
                    InOperation = false;
                    ConnectedToServer = false;
                    break;
            }
        }

        /// <summary>
        /// TCP Transaction handler.
        /// </summary>
        /// <param name="time">Time of transaction.</param>
        /// <param name="client">Client</param>
        /// <param name="buffer"></param>
        private void TransactionHandler(DateTime time, TcpClient client, Memory<byte>? buffer)
        {
            // Get the endpoint of client
            IPEndPoint? endPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Debug.WriteLine($"{time} : {endPoint?.Address.MapToIPv4()}/{endPoint?.Port}");


            if (buffer != null)
            {
                switch (SelectedBufferProcessorMethod ?? BufferProccesorMethod.Disable)
                {
                    case BufferProccesorMethod.TextMessage:
                        Debug.WriteLine($"Message method.");
                        MessageFromServer = $"({time:HH:mm} {endPoint?.Address.MapToIPv4()}) {BufferToStringMessageProcessor(buffer)}";
                        break;
                    default:
                        Debug.WriteLine($"Method not defined.");
                        break;
                }

            }
        }
    }
}
