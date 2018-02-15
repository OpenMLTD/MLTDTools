using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace OpenMLTD.ThankYou {
    internal static class IPAddressHelper {

        public static IReadOnlyList<KeyValuePair<IPAddress, string>> GetAvailableIPAddresses() {
            var niList = from interf in NetworkInterface.GetAllNetworkInterfaces()
                where interf.OperationalStatus == OperationalStatus.Up
                let kv = new KeyValuePair<IPAddress, string>(GetIpFromUnicastAddresses(interf), interf.Name)
                where kv.Key != null
                select kv;
            return niList.ToArray();
        }

        private static IPAddress GetIpFromUnicastAddresses([NotNull] NetworkInterface i) {
            return (from ip in i.GetIPProperties().UnicastAddresses
                where ip.Address.AddressFamily == AddressFamily.InterNetwork
                select ip.Address).SingleOrDefault();
        }

    }
}