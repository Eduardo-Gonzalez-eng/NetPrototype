using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPrototype.Enums
{
    /// <summary>
    /// Represents the states of a network connection.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// The connection is not active or has not been initiated.
        /// </summary>
        Inactive,

        /// <summary>
        /// The connection is currently being established.
        /// </summary>
        Connecting,

        /// <summary>
        /// The connection is active and data can be transmitted.
        /// </summary>
        Active,

        /// <summary>
        /// The connection attempt failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The connection is being terminated.
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Listen for clients
        /// </summary>
        Listen,

    }
}
