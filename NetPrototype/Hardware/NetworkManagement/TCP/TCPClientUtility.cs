using NetPrototype.Enums;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetPrototype.Hardware.NetworkManagement.TCP
{
    public class TCPClientUtility : INetworkStatusHandler<TCPClientUtility>
    {
        /// <summary>
        /// The semaphore to enable a only operation.
        /// </summary>
        //private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

        /// <summary>
        /// Represents the connection status.
        /// </summary>
        private event Action<ConnectionState> ConnectionStatus;

        /// <summary>
        /// Represents the operations of connection status.
        /// </summary>
        private event Action<DateTime, TcpClient, Memory<byte>?> Transaction;

        /// <summary>
        /// The client status.
        /// </summary>
        public ConnectionState ClientStatus { get; private set; }

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> token.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// The <see cref="TcpClient"/> connection reference.
        /// </summary>
        private TcpClient _client;

        public TCPClientUtility()
        {

        }

        /// <summary>
        /// Obtain the current client instance.
        /// </summary>
        /// <returns>The client.</returns>
        public TcpClient GetClient()
        {
            return _client;
        }

        /// <summary>
        /// Connect with server asynchronously.
        /// </summary>
        /// <param name="ipv4Address">The IPv4 of de server.</param>
        /// <param name="port">The port of the server</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If the client is connected.</exception>
        public async Task ConnectWithServerAsync(IPAddress ipv4Address, int port)
        {
            if(_client != null && _client.Connected)
            {
                throw new InvalidOperationException("The client is connected.");
            }

            NotifyConnectionStatus(ConnectionState.Connecting);

            await Task.Delay(100);

            try
            {
                // Crear una instancia de TcpClient y conectarse al servidor
                _client = new TcpClient();
                _cancellationTokenSource = new CancellationTokenSource();

                await _client.ConnectAsync(ipv4Address, port); // Conectar de forma asíncrona
                _ = ReadBufferFromServerAsync();
               
                NotifyConnectionStatus(ConnectionState.Active);
            }
            catch (SocketException ex)
            {
                Trace.WriteLine($"Socket Error: {ex.Message}");
                NotifyConnectionStatus(ConnectionState.Failed); // Estado explícito en caso de fallo
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error: {ex.Message}");
                NotifyConnectionStatus(ConnectionState.Failed);
            }
        }

        public void StopClient()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _client?.Close();
                _client?.Dispose(); // Asegura la liberación de recursos correctamente
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error al cerrar cliente: {ex.Message}");
            }
            finally
            {
                NotifyConnectionStatus(ConnectionState.Inactive);
            }
        }


        public async Task SendBufferToServer(BufferProccesorMethod method, ReadOnlyMemory<byte> txBuffer)
        {
            if (_client == null || !_client.Connected)
            {
                throw new InvalidOperationException("The must be connected with some server.");
            }

            switch (method)
            {
                case BufferProccesorMethod.TextMessage:
                    Debug.WriteLine($"Method message.");
                    await _client
                    .GetStream()
                        .WriteAsync(txBuffer, _cancellationTokenSource.Token)
                        .ConfigureAwait(false);
                    break;
                default:
                    Debug.WriteLine($"Method no implemented.");
                    break;
            }
        }
        /// <summary>
        /// Return 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task ReadBufferFromServerAsync()
        {
            if (_client == null || ClientStatus == ConnectionState.Active)
            {
                throw new InvalidOperationException("The must be connected with some server.");
            }

            while(!_cancellationTokenSource.IsCancellationRequested)
            {
                Memory<byte> RxBuffer = new(new byte[1024]);

                Debug.WriteLine("The buffer message is update {0}", RxBuffer);

                await _client.GetStream().ReadAsync(RxBuffer, _cancellationTokenSource.Token);

                NotifyTransaction(DateTime.Now, _client, RxBuffer);
            }
        }

        #region EventManagerSection
        public void NotifyConnectionStatus(ConnectionState connectionStatus)
        {
            ClientStatus = connectionStatus;
            ConnectionStatus?.Invoke(connectionStatus);
        }

        public void NotifyTransaction(DateTime time, TcpClient client, Memory<byte>? buffer)
        {
            Transaction?.Invoke(time, client, buffer);
        }

        public TCPClientUtility OffConnectionStatus(Action<ConnectionState> action)
        {
            ConnectionStatus -= action;
            return this;
        }

        public TCPClientUtility OffTransaction(Action<DateTime, TcpClient, Memory<byte>?> action)
        {
            Transaction -= action;
            return this;
        }

        public TCPClientUtility OnConnectionStatus(Action<ConnectionState> state)
        {
            ConnectionStatus += state;
            return this;
        }

        public TCPClientUtility OnTransaction(Action<DateTime, TcpClient, Memory<byte>?> tryInitializeConnection)
        {
            Transaction += tryInitializeConnection;
            return this;
        }
        #endregion
    }
}
