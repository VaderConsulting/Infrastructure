/*
//
// The following code is part of the following article: 
// Title: Implementation of DHCP like client and server "Auto Discovery" behaviour with C# and Sockets by using UDP Broadcast packets.
// 
// Source-code for client console app example
//
// Author: Bruno Ratnieks
// Contact: bruno@sniffer.net
// Company: Sniffer Networks do Brasil Ltda. - SNF
//
// The following code may be used freely for any purpose.
//
//
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Bruno.Ratnieks.AutoDiscovery;

namespace AutoDiscovery_app_Client
{

    public class clientSide
    {
        public clientSide()
        {

            BackgroundWorker objWorkerServerDiscovery = new BackgroundWorker();
            objWorkerServerDiscovery.WorkerReportsProgress = true;
            objWorkerServerDiscovery.WorkerSupportsCancellation = true;

            AutoDiscoveryClient svcAutoDiscoveryClient = new AutoDiscoveryClient(ref objWorkerServerDiscovery);
            objWorkerServerDiscovery.DoWork += new DoWorkEventHandler(svcAutoDiscoveryClient.Start);
            objWorkerServerDiscovery.ProgressChanged += new ProgressChangedEventHandler(logWorkers_ProgressChanged);
            objWorkerServerDiscovery.RunWorkerAsync();
        }
        private void logWorkers_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Report thread messages to Console
            Console.WriteLine(e.UserState.ToString());
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            clientSide cs = new clientSide();
            while (true)
            {
                // keep running.. sleeping. 
                Console.ReadLine();
                return;
            }
        }
    }
}
    
