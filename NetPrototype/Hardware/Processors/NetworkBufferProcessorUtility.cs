using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetPrototype.Hardware.Processors
{
    public static class NetworkBufferProcessorUtility
    {
        /// <summary>
        /// Process the buffer and converts it into a text message string.
        /// </summary>
        /// <param name="buffer">The buffer to process.</param>
        /// <returns>A string message post processor.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string BufferToStringMessageProcessor(Memory<byte>? buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer), "buffer can´t be null");

            Span<byte> data = buffer.Value.Span;

            return Encoding.UTF8.GetString(data);
        }
    }
}
