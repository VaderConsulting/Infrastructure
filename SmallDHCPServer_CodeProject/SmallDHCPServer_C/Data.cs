using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyDHCP
{
    public class Data
    {
        #region Constants

        public const int OPTION_OFFSET = 240;

        #endregion

        #region Delegates

        #endregion

        #region Events

        #endregion

        #region Enums and Structs

        //public struct DHCPData
        //{
        //    public string IPAddr;
        //    public string SubMask;
        //    public uint LeaseTime;
        //    public string ServerName;
        //    public string MyIP;
        //    public string RouterIP;
        //    public string DomainIP;
        //    public string LogServerIP;
        //}

        #endregion

        #region DLL Imports

        #endregion

        #region Fields

        public Message Message = new Message();

        // These are the scope options
        public string IPAddress;
        public string SubMask;
        public uint LeaseTime;
        public string DNSSuffix; // ** was ServerName
        public string MyIP;
        public string Gateway;  // ** was RouterIP
        public string DomainIP;
        public string LogServerIP;
        public string[] DNSServerIPAddressList;

        #endregion

        #region Properties

        #endregion

        #region Constructors and Destructor

        public Data(byte[] DataToRead)
        {
            System.IO.BinaryReader Reader;
            System.IO.MemoryStream Stream = new System.IO.MemoryStream(DataToRead, 0, DataToRead.Length);

            try
            {
                Reader = new System.IO.BinaryReader(Stream);

                //read data
                Message.OpCode = Reader.ReadByte();
                Message.AddressType = Reader.ReadByte();
                Message.AddressLength = Reader.ReadByte();
                Message.HardwareOptions = Reader.ReadByte();
                Message.TransactionID = Reader.ReadBytes(4);
                Message.ElapsedTime = Reader.ReadBytes(2);
                Message.Flags = Reader.ReadBytes(2);
                Message.ClientIPAddress = Reader.ReadBytes(4);
                Message.OtherIPAddress = Reader.ReadBytes(4);
                Message.ServerIPAddress = Reader.ReadBytes(4);
                Message.RelayIPAddress = Reader.ReadBytes(4);
                Message.MACAddress = Reader.ReadBytes(16);
                Message.ServerName = Reader.ReadBytes(64);
                Message.BootFIlename = Reader.ReadBytes(128);
                Message.MagicCookie = Reader.ReadBytes(4);
                Message.Options = Reader.ReadBytes(DataToRead.Length - OPTION_OFFSET);
            }
            catch
            {
                // AppendError(ex.Message, ClassName, "cDHCPStruct(byte[] Data)");
            }
            finally
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                }

                Stream = null;
                Reader = null;
            }
        }

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #endregion

    }
}
