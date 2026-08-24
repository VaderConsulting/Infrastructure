#region Copyright Information
/*
 * (C)  2005-2007, Marcello Cauchi Savona
 *
 * For terms and usage, please see the LICENSE file
 * provided alongwith or contact marcello_c@hotmail.com
 * 
 * http://www.cheekyneedle.com
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace TinyDHCP
{


    public partial class frmMain : Form
    {
        #region Constants

        #endregion

        #region Delegates

        #endregion

        #region Events

        #endregion

        #region Enums

        #endregion

        #region DLL Imports

        #endregion

        #region Fields

        private DHCP Server;
        //private Button tt = new Button();

        //private delegate void Adff();

        delegate void SimpleDelegate();
        public string startIp, MacMask, AdapterIP;
        public bool NetCardAlive = false;
        private Thread dHCPThread = null;

        #endregion

        #region Properties

        #endregion

        #region Constructors and Destructor

        public frmMain()
        {
            InitializeComponent();
            //ini the text strings
            lblDHCPMACAddress.Text = "Enter the Mac Mask: \n(leave empty to allow all)";
            //fill up the adapter list box with info
            //get the default settings from registry
            txtStartIPAddress.Text = "10.1.1.200"; // (string)Registry.LocalMachine.GetValue("IpAdd", "10.192.10.22");
            txtMACMask.Text = ""; // (string)Registry.LocalMachine.GetValue("MacMask", "00"); //add more data to this to block 
            btnSetServerState.Text = "Start";
        }

        #endregion

        #region Event Handlers

        private void btnSetServerState_Click(object sender, EventArgs e)
        {
            switch (btnSetServerState.Text)
            {
                case "Start":

                    //save the values to registry
                    //Registry.LocalMachine.SetValue("IPAdd", txtStartIPAddress.Text);
                    //Registry.LocalMachine.SetValue("MacMask", txtMACMask.Text);


                    AdapterIP = "0.0.0.0"; //allow data from all network cards
                    txtStartIPAddress.Enabled = false;
                    txtMACMask.Enabled = false;
                    btnSetServerState.Text = "Stop";
                    Server = new DHCP(AdapterIP);
                    Server.Announced += new DHCP.AnnouncedEventHandler(cDhcp_Announced);
                    Server.Request += new DHCP.RequestEventHandler(cDhcp_Request);
                    startIp = txtStartIPAddress.Text;
                    MacMask = txtMACMask.Text;

                    dHCPThread = new Thread(Server.Start);
                    dHCPThread.Start();
                    break;
                default:
                    txtStartIPAddress.Enabled = true;
                    txtMACMask.Enabled = true;
                    btnSetServerState.Text = "Start";

                    if (Server != null)
                    {
                        Server.Dispose();
                    }

                    break;
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            //btnSetServerState.PerformClick();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Server != null)
            {
                Server.Dispose();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Private Methods

        private static uint IPAddressToLongBackwards(string IPAddr)
        {
            System.Net.IPAddress Address = System.Net.IPAddress.Parse(IPAddr);
            byte[] Octet = Address.GetAddressBytes();

            uint Result = (uint)Octet[0] << 24;
            Result += (uint)Octet[1] << 16;
            Result += (uint)Octet[2] << 8;
            Result += (uint)Octet[3];

            return Result;
        }

        private string GetIPAddress()
        {
            IPAddress Result;
            byte[] Octet;
            UInt32 Index;
            try
            {
                Index = IPAddressToLongBackwards(startIp);
                Index -= 1;
                do
                {
                    Index += 1;
                    Octet = new byte[4];
                    Octet[3] = (byte)(Index);
                    Octet[2] = (byte)(Index >> 8);
                    Octet[1] = (byte)(Index >> 16);
                    Octet[0] = (byte)(Index >> 24);
                    Result = new IPAddress(Octet);
                    // yy = IPAddress.HostToNetworkOrder(ii);
                }
                while (CheckAlive(Result.ToString()));
                //reaching here means that the ip is free

                return Result.ToString();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        public static bool CheckAlive(string IpAdd)
        {
            Ping pingSender = new Ping();
            IPAddress address;
            PingReply reply;

            try
            {
                address = IPAddress.Parse(IpAdd);//IPAddress.Loopback;
                reply = pingSender.Send(address, 100);
                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine("Address: {0}", reply.Address.ToString());
                    Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                    Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                    Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                    Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                    return true;
                }
                else
                {
                    Console.WriteLine(reply.Status);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                if (pingSender != null)
                {
                    pingSender.Dispose();
                }

                pingSender = null;
                address = null;
                reply = null;
            }
        }

        public void cDhcp_Announced(Data Data, string MacId)
        {
            string str = string.Empty;

            if (MacId.StartsWith(MacMask) == true)
            {
                //options should be filled with valid data
                Data.IPAddress = GetIPAddress();
                Data.SubMask = "255.255.0.0";
                Data.LeaseTime = 2000;
                Data.DNSSuffix = "Judo";
                Data.MyIP = AdapterIP;
                Data.Gateway = "0.0.0.0";
                Data.LogServerIP = "0.0.0.0";
                Data.DomainIP = "0.0.0.0";

                str = "IP requested for Mac: " + MacId;

            }
            else
            {
                str = "Mac: " + MacId + " is not part of the mask!";
            }

            Server.SendDHCPMessage(Message.Type.OFFER, Data);

            SimpleDelegate lst = delegate
            {
                listBox1.Items.Add(str);
            };

            Invoke(lst);
            //Application.DoEvents()
        }

        public void cDhcp_Request(Data Data, string MacId)
        {
            string str = string.Empty;

            if (MacId.StartsWith(MacMask))
            {
                //announced so then send the offer
                Data.IPAddress = GetIPAddress();
                Data.SubMask = "255.255.255.0";
                Data.LeaseTime = 2000;
                Data.DNSSuffix = "Judo";
                Data.MyIP = AdapterIP;
                Data.Gateway = "10.1.1.1";
                Data.LogServerIP = "0.0.0.0";
                Data.DomainIP = "0.0.0.0";
                Data.DNSServerIPAddressList = new string[2] {"10.1.1.1", "10.1.1.2"};
                
                Server.SendDHCPMessage(Message.Type.ACK, Data);

                str = "IP " + Data.IPAddress + " Offered to Mac: " + MacId;
            }
            else
            {
                str = "Mac: " + MacId + " is not part of the mask!";
            }

            SimpleDelegate lst = delegate
            {
                listBox1.Items.Add(str);
            };

            Invoke(lst);
        }

        #endregion

    }
}