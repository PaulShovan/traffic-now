﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.NotificationServer
{
    internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new NotificationServer()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception)
            {
                
            }
        }
    }
}