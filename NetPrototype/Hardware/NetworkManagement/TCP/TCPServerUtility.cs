using NetPrototype.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetPrototype.Hardware.NetworkManagement.TCP
{
    public class TCPServerUtility : INetworkStatusHandler<TCPServerUtility>
    {
        /// <summary>
        /// The semaphore to enable a only operation.
        /// </summary>
        private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

        /// <summary>
        /// Represents the connection status.
        /// </summary>
        private event Action<ConnectionState> ConnectionStatus;

        /// <summary>
        /// Represents the operations of connection status.
        /// </summary>
        private event Action<DateTime, TcpClient, Memory<byte>?> Transaction;

        /// <summary>
        /// The <see cref="TcpListener"/> connection reference.
        /// </summary>
        private TcpListener _server;

        /// <summary>
        /// The server status.
        /// </summary>
        public ConnectionState StatusServer {  get; private set; }

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> token.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Parameters of server
        /// </summary>
        /// <param name="ipv4Address"></param>
        /// <param name="port"></param>
        public TCPServerUtility()
        {
            
        }

        /// <summary>
        /// Initialize server asynchronously.
        /// </summary>
        public async Task StartServerAsync(IPAddress ipv4Address, int port)
        {
            if(StatusServer == ConnectionState.Connecting)
            {
                Trace.WriteLine("The server is connecting. Please Wait");
                return;
            }

            if (ipv4Address == null)
            {
                throw new ArgumentNullException(nameof(ipv4Address), "IP address cannot be null.");
            }

            NotifyConnectionStatus(ConnectionState.Connecting);

            // ConfigureAwait(false) for the prevent deadlocks
            await Task.Delay(100).ConfigureAwait(false);

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _server = new TcpListener(ipv4Address ?? IPAddress.Any, port);
                _server.Start();

                // Notify server initialize
                NotifyConnectionStatus(ConnectionState.Listen);

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        // wait for any connection.   
                        TcpClient client = await _server.AcceptTcpClientAsync(_cancellationTokenSource.Token).ConfigureAwait(false);

                        // Handle de client connected.
                        _ = HandleClientAsync(client);
                    }
                    catch(OperationCanceledException ex)
                    {
                        Debug.WriteLine("The operation has been canceled by the user.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _server?.Stop();
                NotifyConnectionStatus(ConnectionState.Inactive);
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void StopServer()
        {
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Handle clients that connect with server.
        /// </summary>
        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    

                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    Memory<byte> RxBuffer = new Memory<byte>(buffer);

                    NotifyTransaction(DateTime.Now, client, null);

                    // Read asynchronously
                    while ((bytesRead = await stream.ReadAsync(RxBuffer, _cancellationTokenSource.Token).ConfigureAwait(false)) > 0)
                    {
                        await _syncSemaphore.WaitAsync(_cancellationTokenSource.Token);
                        try
                        {
                            NotifyTransaction(DateTime.Now, client, RxBuffer.Slice(0, bytesRead));
                        }
                        finally
                        {
                            _syncSemaphore.Release();
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error {0}", e.Message);
            }
            finally
            {
                client.Close();
            }
        }

        /// <summary>
        /// Handle response for the client.
        /// </summary>
        /// <param name="method">Represent the method of the precessor.</param>
        /// <param name="client">Represent a client to whom the data will be sent. This must be connected.</param>
        /// <param name="txBuffer">Represent the buffer to will be send.</param>
        /// <returns></returns>
        public async Task SendToClientAsync(BufferProccesorMethod method, TcpClient client, ReadOnlyMemory<byte> txBuffer)
        {
            if(!client.Client.Connected)
            {
                throw new InvalidOperationException("The client cannot be offline.");
            }

            await _syncSemaphore.WaitAsync(_cancellationTokenSource.Token); 

            try
            {
                switch (method)
                {
                    case BufferProccesorMethod.TextMessage:
                        Debug.WriteLine($"Method message.");
                        await client
                            .GetStream()
                            .WriteAsync(txBuffer, _cancellationTokenSource.Token)
                            .ConfigureAwait(false);
                        break;
                    default:
                        Debug.WriteLine($"Method no implemented.");
                        break;
                }
            }
            finally
            {
                _syncSemaphore.Release(); 
            }
        }

        #region EventManagerSection
        public void NotifyConnectionStatus(ConnectionState connectionStatus)
        {
            StatusServer = connectionStatus;
            ConnectionStatus?.Invoke(connectionStatus);
        }

        public void NotifyTransaction(DateTime time, TcpClient client, Memory<byte>? buffer)
        {
            Transaction?.Invoke(time, client, buffer);
        }

        public TCPServerUtility OffConnectionStatus(Action<ConnectionState> action)
        {
            ConnectionStatus -= action;
            return this;
        }

        public TCPServerUtility OffTransaction(Action<DateTime, TcpClient, Memory<byte>?> action)
        {
            Transaction -= action;
            return this;
        }

        public TCPServerUtility OnConnectionStatus(Action<ConnectionState> state)
        {
            ConnectionStatus += state;
            return this;
        }

        public TCPServerUtility OnTransaction(Action<DateTime, TcpClient, Memory<byte>?> tryInitializeConnection)
        {
            Transaction += tryInitializeConnection;
            return this;
        }
        #endregion
    }
}
