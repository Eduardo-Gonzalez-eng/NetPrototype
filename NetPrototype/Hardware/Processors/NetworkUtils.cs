using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetPrototype.Hardware.Processors
{
    public static class NetworkUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress">The ip address to evaluate.</param>
        /// <returns>If valid.</returns>
        public static bool IsValidIPAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return false;
            }

            return IPAddress.TryParse(ipAddress, out _);
        }
    }
}
