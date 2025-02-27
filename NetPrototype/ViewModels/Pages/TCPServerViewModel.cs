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
        private string _messageToClientOfServerStatus;

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

        public TCPServerViewModel()
        {
            _tcpServerUtility = new TCPServerUtility();

            // initialize all suscriptions.
            _tcpServerUtility
                .OnConnectionStatus(ConnectionHandler)
                .OnTransaction(TransactionHandler);

            // Default status.
            SelectedBufferProcessorMethod = BufferProccesorMethod.TextMessage;
            IsAutomaticResponse = true;
            MessageFromServerToClient = "Hola desde el servidor!";
        }

        private void ConnectionHandler(ConnectionState connSts)
        {
            switch (connSts)
            {
                case ConnectionState.Connecting:
                    Debug.WriteLine("Initializing server");
                    MessageToClientOfServerStatus = "Iniciando servidor...";

                    break;
                case ConnectionState.Listen:
                    Debug.WriteLine("The server is listening.");
                    MessageToClientOfServerStatus = $"Escuchando en el puerto {ServerPort}...";

                    break;
                case ConnectionState.Inactive:
                    Debug.WriteLine("The server is inactive.");
                    MessageToClientOfServerStatus = "Servidor inactivo.";
                    break;
                default:
                    Debug.WriteLine("State not implemented.");
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
    }
}
