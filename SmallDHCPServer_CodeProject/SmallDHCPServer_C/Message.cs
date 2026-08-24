using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyDHCP
{
    public class Message
    {
        public enum Type
        {
            DISCOVER = 1,
            OFFER = 2,
            REQUEST = 3,
            DECLINE = 4,
            ACK = 5,
            NAK = 6,
            RELEASE = 7,
            INFORM = 8
        }

        public byte OpCode;            // Op code:   1 = bootRequest, 2 = BootReply
        public byte AddressType;       // Hardware Address Type: 1 = 10MB ethernet
        public byte AddressLength;     // hardware address length: length of MACID
        public byte HardwareOptions;   // Hw options
        public byte[] TransactionID;   // transaction id (5)
        public byte[] ElapsedTime;     // elapsed time from trying to boot (3)
        public byte[] Flags;           // flags (3)
        public byte[] ClientIPAddress; // client IP (5)
        public byte[] OtherIPAddress;  // your client IP (5)
        public byte[] ServerIPAddress; // Server IP  (5)
        public byte[] RelayIPAddress;  // relay agent IP (5)
        public byte[] MACAddress;      // Client HW address (16)
        public byte[] ServerName;      // Optional server host name (64)
        public byte[] BootFIlename;    // Boot file name (128)
        public byte[] MagicCookie;     // Magic cookie (4)
        public byte[] Options;         // options (rest)

    }
}
