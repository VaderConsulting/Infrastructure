using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TinyDHCP
{
    class DHCP
    {
        #region Constants

        #endregion

        #region Delegates

        public delegate void AnnouncedEventHandler(Data Data, string MacId);
        public delegate void ReleasedEventHandler();
        public delegate void RequestEventHandler(Data Data, string MacId);
        public delegate void AssignedEventHandler(string IPAddress, string MacID);

        #endregion

        #region Events

        public event AnnouncedEventHandler Announced;
        public event RequestEventHandler Request;

        #endregion

        #region Enums

        

        public enum Option  //refer to rfc2132.txt for vendor specific info
        {
            SubnetMask = 1,
            TimeOffset = 2,
            Router = 3,
            TimeServer = 4,
            NameServer = 5,
            DomainNameServer = 6,
            LogServer = 7,
            CookieServer = 8,
            LPRServer = 9,
            ImpressServer = 10,
            ResourceLocServer = 11,
            HostName = 12,
            BootFileSize = 13,
            MeritDump = 14,
            DomainName = 15,
            SwapServer = 16,
            RootPath = 17,
            ExtensionsPath = 18,
            IpForwarding = 19,
            NonLocalSourceRouting = 20,
            PolicyFilter = 21,
            MaximumDatagramReAssemblySize = 22,
            DefaultIPTimeToLive = 23,
            PathMTUAgingTimeout = 24,
            PathMTUPlateauTable = 25,
            InterfaceMTU = 26,
            AllSubnetsAreLocal = 27,
            BroadcastAddress = 28,
            PerformMaskDiscovery = 29,
            MaskSupplier = 30,
            PerformRouterDiscovery = 31,
            RouterSolicitationAddress = 32,
            StaticRoute = 33,
            TrailerEncapsulation = 34,
            ARPCacheTimeout = 35,
            EthernetEncapsulation = 36,
            TCPDefaultTTL = 37,
            TCPKeepaliveInterval = 38,
            TCPKeepaliveGarbage = 39,
            NetworkInformationServiceDomain = 40,
            NetworkInformationServers = 41,
            NetworkTimeProtocolServers = 42,
            VendorSpecificInformation = 43,
            NetBIOSoverTCPIPNameServer = 44,
            NetBIOSoverTCPIPDatagramDistributionServer = 45,
            NetBIOSoverTCPIPNodeType = 46,
            NetBIOSoverTCPIPScope = 47,
            XWindowSystemFontServer = 48,
            XWindowSystemDisplayManager = 49,
            RequestedIPAddress = 50,
            IPAddressLeaseTime = 51,
            OptionOverload = 52,
            DHCPMessageType = 53,
            ServerIdentifier = 54,
            ParameterRequestList = 55,
            Message = 56,
            MaximumDHCPMessageSize = 57,
            RenewalTimeValue_T1 = 58,
            RebindingTimeValue_T2 = 59,
            Vendorclassidentifier = 60,
            ClientIdentifier = 61,
            NetworkInformationServicePlusDomain = 64,
            NetworkInformationServicePlusServers = 65,
            TFTPServerName = 66,
            BootfileName = 67,
            MobileIPHomeAgent = 68,
            SMTPServer = 69,
            POP3Server = 70,
            NNTPServer = 71,
            DefaultWWWServer = 72,
            DefaultFingerServer = 73,
            DefaultIRCServer = 74,
            StreetTalkServer = 75,
            STDAServer = 76,
            END_Option = 255
        }

        #endregion

        #region DLL Imports

        #endregion

        #region Fields

        private UDPService Service; // the udp snd/rcv class
        private string NetCard;

        #endregion

        #region Properties

        #endregion

        #region Constructors and Destructor

        public DHCP(string NetCard1)
        {
            NetCard = NetCard1;
        }

        ~DHCP()
        {

            Service = null;
        }

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        //pass the option type that you require
        //parse the option data
        //return the data in a byte of what we need
        private byte[] GetOptionData(Option Type, Data Data)
        {
            int DHCPId = 0;
            byte DataID = 0;
            byte DataLength = 0;
            byte[] Result;

            try
            {
                DHCPId = (int)Type;

                //loop through look for the bit that states that the identifier is there
                for (int i = 0; i < Data.Message.Options.Length; i++)
                {
                    //at the start we have the code + length
                    //i has the code, i+1 = length of data, i+1+n = data skip
                    DataID = Data.Message.Options[i];

                    if (DataID == DHCPId)
                    {
                        DataLength = Data.Message.Options[i + 1];
                        Result = new byte[DataLength];
                        Array.Copy(Data.Message.Options, i + 2, Result, 0, DataLength);

                        return Result;
                    }
                    else
                    {
                        DataLength = Data.Message.Options[i + 1]; //'length of code
                        i += 1 + DataLength;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Result = null;
            }

            return null;
        }

        private void CreateOptions(ref Data DHCPData, Message.Type OptionReplyMsg)
        {
            byte[] PReqList;
            byte[] Data;
            byte[] LeaseTime;
            byte[] MyIp;

            try
            {
                //we look for the parameter request list
                PReqList = GetOptionData(Option.ParameterRequestList, DHCPData);

                //erase the options array, and set the message type to ack
                DHCPData.Message.Options = null;
                CreateOptionElement(Option.DHCPMessageType, new byte[] { (byte)OptionReplyMsg }, ref DHCPData.Message.Options);

                //server identifier, my IP
                MyIp = IPAddress.Parse(DHCPData.MyIP).GetAddressBytes();
                CreateOptionElement(Option.ServerIdentifier, MyIp, ref DHCPData.Message.Options);

                //PReqList contains the option data in a byte that is requested by the unit
                foreach (byte i in PReqList)
                {
                    Data = null;
                    switch ((Option)i)
                    {
                        case Option.SubnetMask:
                            Data = IPAddress.Parse(DHCPData.SubMask).GetAddressBytes();
                            break;
                        case Option.Router:
                            Data = IPAddress.Parse(DHCPData.Gateway).GetAddressBytes();
                            break;
                        case Option.DomainNameServer:
                            Data = IPAddress.Parse(DHCPData.DomainIP).GetAddressBytes();
                            break;
                        case Option.DomainName:
                            Data = System.Text.Encoding.ASCII.GetBytes(DHCPData.DNSSuffix);
                            break;
                        case Option.ServerIdentifier:
                            Data = IPAddress.Parse(DHCPData.MyIP).GetAddressBytes();
                            break;
                        case Option.LogServer:
                            Data = System.Text.Encoding.ASCII.GetBytes(DHCPData.LogServerIP);
                            break;
                        case Option.NetBIOSoverTCPIPNameServer:
                            break;
                        //case Option.NameServer:
                        //    Data = System.Text.Encoding.ASCII.GetBytes(DHCPData.DNSServerIPAddressList[0]);
                        //    break;
                    }
                    if (Data != null)
                        CreateOptionElement((Option)i, Data, ref DHCPData.Message.Options);
                }

                //lease time
                LeaseTime = new byte[4];
                LeaseTime[3] = (byte)(DHCPData.LeaseTime);
                LeaseTime[2] = (byte)(DHCPData.LeaseTime >> 8);
                LeaseTime[1] = (byte)(DHCPData.LeaseTime >> 16);
                LeaseTime[0] = (byte)(DHCPData.LeaseTime >> 24);

                CreateOptionElement(Option.IPAddressLeaseTime, LeaseTime, ref DHCPData.Message.Options);
                CreateOptionElement(Option.RenewalTimeValue_T1, LeaseTime, ref DHCPData.Message.Options);
                CreateOptionElement(Option.RebindingTimeValue_T2, LeaseTime, ref DHCPData.Message.Options);

                //create the end option
                Array.Resize(ref DHCPData.Message.Options, DHCPData.Message.Options.Length + 1);
                Array.Copy(new byte[] { 255 }, 0, DHCPData.Message.Options, DHCPData.Message.Options.Length - 1, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                LeaseTime = null;
                PReqList = null;
                Data = null;

            }
        }

        //function to build the data structure to a byte array
        private byte[] CreateMessage(Message Message)
        {
            byte[] Result;

            Console.WriteLine("Creating message");

            try
            {
                Result = new byte[0];

                AddOptionElement(new byte[] { Message.OpCode }, ref Result);
                AddOptionElement(new byte[] { Message.AddressType }, ref Result);
                AddOptionElement(new byte[] { Message.AddressLength }, ref Result);
                AddOptionElement(new byte[] { Message.HardwareOptions }, ref Result);
                AddOptionElement(Message.TransactionID, ref Result);
                AddOptionElement(Message.ElapsedTime, ref Result);
                AddOptionElement(Message.Flags, ref Result);
                AddOptionElement(Message.ClientIPAddress, ref Result);
                AddOptionElement(Message.OtherIPAddress, ref Result);
                AddOptionElement(Message.ServerIPAddress, ref Result);
                AddOptionElement(Message.RelayIPAddress, ref Result);
                AddOptionElement(Message.MACAddress, ref Result);
                AddOptionElement(Message.ServerName, ref Result);
                AddOptionElement(Message.BootFIlename, ref Result);
                AddOptionElement(Message.MagicCookie, ref Result);
                AddOptionElement(Message.Options, ref Result);

                return Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                Result = null;
            }

        }

        private void AddOptionElement(byte[] FromValue, ref byte[] TargetArray)
        {
            try
            {
                if (TargetArray != null)
                {
                    Array.Resize(ref TargetArray, TargetArray.Length + FromValue.Length);
                }
                else
                {
                    Array.Resize(ref TargetArray, FromValue.Length);
                }

                Array.Copy(FromValue, 0, TargetArray, TargetArray.Length - FromValue.Length, FromValue.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Create an option message.  Shall always append at the end of the message
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="DataToAdd"></param>
        /// <param name="AddtoMe"></param>
        private void CreateOptionElement(Option Code, byte[] DataToAdd, ref byte[] AddtoMe)
        {
            byte[] Option;

            try
            {
                Option = new byte[DataToAdd.Length + 2];

                //add the code, and data length
                Option[0] = (byte)Code;
                Option[1] = (byte)DataToAdd.Length;

                //add the code to put in
                Array.Copy(DataToAdd, 0, Option, 2, DataToAdd.Length);

                //copy the data to the out array
                if (AddtoMe == null)
                {
                    Array.Resize(ref AddtoMe, (int)Option.Length);
                }
                else
                {
                    Array.Resize(ref AddtoMe, AddtoMe.Length + Option.Length);
                }

                Array.Copy(Option, 0, AddtoMe, AddtoMe.Length - Option.Length, Option.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        #endregion

        #region Public Methods

        //function to start the DHCP server
        //port 67 to receive, 68 to send
        public void Start()
        {
            try
            {   // start the DHCP server
                //assign the event handlers
                Service = new UDPService(67, 68, NetCard);
                Service.DataReceived += new UDPService.DataReceivedEventHandler(UDP_DataReceived);

                Console.WriteLine("Service started");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string ByteToString(byte[] Data, byte Length)
        {
            string Result;

            try
            {
                Result = string.Empty;
                if (Data != null)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        Result += Data[i].ToString("X2");
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
            finally
            {
                Result = null;
            }

        }

        //get the Message type
        //located in the options stream
        public Message.Type GetMessageType(Data Data)
        {
            byte[] Result;

            try
            {
                Result = GetOptionData(Option.DHCPMessageType, Data);

                if (Result != null)
                {
                    return (Message.Type)Result[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        public void UDP_DataReceived(byte[] IncomingData, IPEndPoint ReceivingEndPoint)
        {
            Data Data;
            Message.Type MsgTyp;
            string MacID;

            Console.WriteLine("UDP Data received");

            try
            {
                Data = new Data(IncomingData);

                //data is now in the structure
                //get the msg type
                MsgTyp = GetMessageType(Data);
                MacID = ByteToString(Data.Message.MACAddress, Data.Message.AddressLength);// (string)ddHcpS.dStruct.D_chaddr;

                switch (MsgTyp)
                {
                    case Message.Type.DISCOVER:
                        //myClass.Process(myLogger);
                        // a Mac has requested an IP
                        // discover Msg Has been sent
                        Console.WriteLine("...Discover");
                        Announced(Data, MacID);
                        break;
                    case Message.Type.REQUEST:
                        Console.WriteLine("...Request");
                        Request(Data, MacID);
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //mac announced itself, established IP etc....
        //send the offer to the mac
        public void SendDHCPMessage(Message.Type MessageType, Data DHCPData)
        {
            byte[] Subnet;
            byte[] HostID;
            byte[] DataToSend;


            //we shall leave everything as is structure wise
            //shall CHANGE the type to OFFER
            //shall set the client's IP-Address
            try
            {
                //change message type to reply
                DHCPData.Message.OpCode = 2; //reply

                //subnet
                Subnet = IPAddress.Parse(DHCPData.SubMask).GetAddressBytes();

                //create your ip address
                DHCPData.Message.OtherIPAddress = IPAddress.Parse(DHCPData.IPAddress).GetAddressBytes();

                //Host ID
                HostID = System.Text.Encoding.ASCII.GetBytes(DHCPData.DNSSuffix);

                CreateOptions(ref DHCPData, MessageType);

                //send the data to the unit
                DataToSend = CreateMessage(DHCPData.Message);

                Console.WriteLine("Sending Data...");
                Service.SendData(DataToSend);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Subnet = null;
                //LeaseTime= null;
                HostID = null;

                DataToSend = null;
            }
        }

        public void Dispose()
        {
            if (Service != null)
                Service.StopListener();
            Service = null;

        }

        #endregion

    }
}
