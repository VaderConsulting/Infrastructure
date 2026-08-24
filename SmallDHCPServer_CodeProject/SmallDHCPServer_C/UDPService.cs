using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;

namespace TinyDHCP
{
    class UDPService

    {
        #region Constants

        #endregion

        #region Delegates

        public delegate void DataReceivedEventHandler(byte[] Data, IPEndPoint IpEndPoint);
        public delegate void ErrorEventHandler(string Message);

        #endregion

        #region Events

        public event DataReceivedEventHandler DataReceived;

        #endregion

        #region Enums and tructs

        public struct DHCPService
        {
            public IPEndPoint Endpoint; //define an end point
            public UdpClient Client;    //define a client
        }

        #endregion

        #region DLL Imports

        #endregion

        #region Fields

        private Int32 PortToListenTo = 0;
        private Int32 PortToSendTo = 0;
        private string ReceiveCardIP;
        public bool Listening;

        // call backs for send/receive!
        public DHCPService Service;

        #endregion

        #region Properties

        #endregion

        #region Constructors and Destructor

        public UDPService()
        {
            Listening = false;
        }

        //overrides pass the port to listen to/sendto and startup
        public UDPService(Int32 PortToListenTo, Int32 PortToSendTo, string CardIP)
        {
            try
            {
                Listening = false;
                this.PortToListenTo = PortToListenTo;
                this.PortToSendTo = PortToSendTo;
                this.ReceiveCardIP = CardIP;

                Console.WriteLine("Service listening");

                StartListener();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        //dispose of all resources
        ~UDPService()
        {
            try
            {
                StopListener();

                if (Service.Client != null)
                {
                    Service.Client.Close();
                }

                Service.Client = null;
                Service.Endpoint = null;
                ReceiveCardIP = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        //function to start the listener call back everytime something is recieved
        private void InitialiseListenerCallBack()
        {
            try
            {
                // start the receive call back method
                Service.Client.BeginReceive(new AsyncCallback(OnDataReceived), Service);
            }
            catch (Exception ex)
            {
                if (Listening)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void StartListener()
        {
            IPAddress ThisIPAddress;
            IPEndPoint ThisEndPoint;

            try
            {
                Listening = false;
                
                //resolve the net card ip address
                ThisIPAddress = IPAddress.Parse(ReceiveCardIP);
                
                //get the ipEndPoint
                ThisEndPoint = new IPEndPoint(ThisIPAddress, PortToListenTo);
                
                // if the udpclient interface is active destroy
                if (Service.Client != null)
                {
                    Service.Client.Close();
                }

                Service.Client = null;
                Service.Endpoint = null;
                
                //re initialise the udp client
                Service = new DHCPService();
                Service.Endpoint = ThisEndPoint;
                Service.Client = new UdpClient(ThisEndPoint);

                Listening = true; // set to start listening
                
                // wait for data
                InitialiseListenerCallBack();
            }
            catch (Exception ex)
            {
                if (Listening)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            finally
            {
                if (Service.Client == null)
                {
                    Thread.Sleep(1000);
                    StartListener();
                }
                else
                {
                    ThisIPAddress = null;
                    ThisEndPoint = null;
                }
            }
        }

        #endregion

        #region Public Methods

        //function to send data as a byte stream to a remote socket
        // modified to work as a callback rather than a block
        public void SendData(byte[] Data)
        {
            Console.WriteLine("Sending data");

            try
            {
                Service.Client.BeginSend(Data, Data.Length, "255.255.255.255", PortToSendTo, new AsyncCallback(OnDataSent), Service);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // This is the call back function, which will be invoked when a client is connected
        public void OnDataSent(IAsyncResult ConnectingClient)
        {
            try
            {
                //get the data
                UdpClient Client = (UdpClient)((DHCPService)(ConnectingClient.AsyncState)).Client;
                Client.EndSend(ConnectingClient); // stop the send call back
            }
            catch (Exception ex)
            {
                if (Listening)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Data sent");
        }

        // This is the call back function, which will be invoked when a client is connected
        public void OnDataReceived(IAsyncResult ConnectedClient)
        {
            byte[] ReceivedBytes;
            UdpClient Client;
            IPEndPoint Endpoint;

            Console.WriteLine("Data received");

            try
            {
                Client = ((DHCPService)(ConnectedClient.AsyncState)).Client;
                Endpoint = ((DHCPService)(ConnectedClient.AsyncState)).Endpoint;

                ReceivedBytes = Client.EndReceive(ConnectedClient, ref Endpoint);

                //raise the event with the data recieved
                DataReceived(ReceivedBytes, Endpoint);
            }
            catch (Exception ex)
            {
                if (Listening)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            finally
            {
                Client = null;
                Endpoint = null;
                ReceivedBytes = null;

                // recall the call back
                InitialiseListenerCallBack();
            }

        }

        //stop the listener thread
        public void StopListener()
        {
            try
            {
                Listening = false;

                if (Service.Client != null)
                {
                    Service.Client.Close();
                }

                Service.Client = null;
                Service.Endpoint = null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion


    }
}
