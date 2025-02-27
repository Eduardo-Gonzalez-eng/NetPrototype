using NetPrototype.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetPrototype.Hardware.NetworkManagement
{
    public interface INetworkStatusHandler<T>
    {
        T OnConnectionStatus(Action<ConnectionState> state);
        T OffConnectionStatus(Action<ConnectionState> action);
        void NotifyConnectionStatus(ConnectionState connectionStatus);

        T OnTransaction(Action<DateTime, TcpClient, Memory<byte>?> state);
        T OffTransaction(Action<DateTime, TcpClient, Memory<byte>?> action);
        void NotifyTransaction(DateTime time, TcpClient client, Memory<byte>? buffer);
    }
}
