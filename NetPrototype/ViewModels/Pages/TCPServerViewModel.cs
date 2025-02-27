using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetPrototype.Enums;
using NetPrototype.Hardware.NetworkManagement.TCP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

using static NetPrototype.Hardware.Processors.NetworkBufferProcessorUtility;

namespace NetPrototype.ViewModels.Pages
{
    /// <summary>
    /// Viewmodel for TCPClientPage.xaml
    /// </summary>
    public partial class TCPServerViewModel : ObservableObject
    {
        /// <summary>
        /// Represents the server tcp utility.
        /// </summary>
        private readonly TCPServerUtility _tcpServerUtility;

        /// <summary>
        /// Represents the server port. 
        /// </summary>
        [ObservableProperty]
        private int _serverPort;

        /// <summary>
        /// Represents the server port. 
        /// </summary>
        [ObservableProperty]
        private string _messageServerStatus;

        /// <summary>
        /// Represents the TextBox of the clients messages.
        /// </summary>
        [ObservableProperty]
        private string _messageFromClient;

        /// <summary>
        /// Represents the method collections for show to user. Asociated directly with <see cref="_selectedBufferProcessorMethod"/>
        /// </summary>
        public ObservableCollection<string> MethodProcess { get; } = [ "Mensaje de texto" ];

        /// <summary>
        /// Represents the method selected by user. Asociated directly with <see cref="MethodProcess"/>.
        /// </summary>
        [ObservableProperty]
        private BufferProccesorMethod? _selectedBufferProcessorMethod;

        /// <summary>
        /// Represents the automatic response to client connected.
        /// </summary>
        [ObservableProperty]
        private bool _isAutomaticResponse;

        /// <summary>
        /// Represents the TextBox message to all clients.
        /// </summary>
        [ObservableProperty]
        private string _messageFromServerToClient;

        /// <summary>
        /// Represents the TextBlock of Connect button;
        /// </summary>
        [ObservableProperty]
        private string _textButtonServer;

        /// <summary>
        /// Represents the dinamic color for inner textblock.
        /// </summary>
        [ObservableProperty]
        private bool _inOperation;

        public bool IsSuscriptionInitalized { get; set; } = false;

        public TCPServerViewModel()
        {
            _tcpServerUtility = new TCPServerUtility();

            // initialize all suscriptions.
            InitalizeSuscriptions();

            // Default status.
            SelectedBufferProcessorMethod = BufferProccesorMethod.TextMessage;
            IsAutomaticResponse = true;
            MessageFromServerToClient = "Hola desde el servidor!";
            TextButtonServer = "Iniciar";
            InOperation = false;
            ServerPort = 3000;
        }

        public void InitalizeSuscriptions()
        {
            if(IsSuscriptionInitalized) return;

            // initialize all suscriptions.
            _tcpServerUtility
                .OnConnectionStatus(ConnectionHandler)
                .OnTransaction(TransactionHandler);

            // Initialized.
            IsSuscriptionInitalized = true;
        }

        private void ConnectionHandler(ConnectionState connSts)
        {
            switch (connSts)
            {
                case ConnectionState.Connecting:
                    Debug.WriteLine("Initializing server");
                    MessageServerStatus = "Iniciando servidor...";
                    InOperation = true;

                    break;
                case ConnectionState.Listen:
                    Debug.WriteLine("The server is listening.");
                    MessageServerStatus = $"Escuchando en el puerto {ServerPort}...";
                    TextButtonServer = "Detener";
                    InOperation = false;
                    break;
                case ConnectionState.Inactive:
                    Debug.WriteLine("Shutdown server.");
                    MessageServerStatus = "Servidor inactivo.";
                    TextButtonServer = "Iniciar";
                    InOperation = false;
                    break;
                default:
                    Debug.WriteLine("State not implemented.");
                    InOperation = false;
                    break;
            }

        }
        private void TransactionHandler(DateTime time, TcpClient client, Memory<byte>? buffer)
        {
            // Get the endpoint of client
            IPEndPoint? endPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Debug.WriteLine($"{time} : {endPoint?.Address}/{endPoint?.Port}");


            if(buffer != null)
            {
                switch (SelectedBufferProcessorMethod ?? BufferProccesorMethod.Disable)
                {
                    case BufferProccesorMethod.TextMessage:
                        Debug.WriteLine($"Message method.");
                        MessageFromClient = $"({time:HH:mm} {endPoint?.Address}) {BufferToStringMessageProcessor(buffer)}";

                        // Enviar una respuesta al cliente
                        byte[] responseData = Encoding.UTF8.GetBytes(MessageFromServerToClient);

                        // If the user select automatically response for the client.
                        if (IsAutomaticResponse) _ = _tcpServerUtility
                                .SendToClientAsync( SelectedBufferProcessorMethod ?? BufferProccesorMethod.Disable, client,  responseData );

                        break;
                    default:
                        Debug.WriteLine($"Method not defined.");
                        break;
                }

            }
        }

        public void Cleanup()
        {
            _tcpServerUtility?.StopServer();

            _tcpServerUtility?.OffConnectionStatus(ConnectionHandler);
            _tcpServerUtility?.OffTransaction(TransactionHandler);

            IsSuscriptionInitalized = false;
        }

        [RelayCommand]
        private void ToggleServer(object parameter)
        {
            if (_tcpServerUtility.StatusServer == ConnectionState.Inactive)
            {
                _ = _tcpServerUtility.StartServerAsync(IPAddress.Any, ServerPort);
            }
            else
            {
                _tcpServerUtility.StopServer();
            }
        }

        [RelayCommand]
        private void SendBufferToServer(object parameter)
        {
            // Send to server
            // _ = _tcpServerUtility.SendToClientAsync(SelectedBufferProcessorMethod ?? BufferProccesorMethod.Disable, // no available, // no available);
        }
    }
}
