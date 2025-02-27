using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPrototype.Enums
{
    /// <summary>
    /// The enum <see cref="BufferProccesorMethod"/> Represents the buffer processor's method of displaying input data.
    /// </summary>
    public enum BufferProccesorMethod
    {
        /// <summary>
        /// The text UTF8 method.
        /// </summary>
        TextMessage,

        /// <summary>
        /// The modbus TCP protocol method.
        /// </summary>
        ModbusTCP,

        /// <summary>
        /// Disable method
        /// </summary>
        Disable

    }
}
