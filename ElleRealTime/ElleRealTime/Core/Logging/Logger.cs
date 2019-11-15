using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NLog;

namespace ElleRealTime.Core.Logging
{
    public class Logger
    {
        private static readonly NLog.Logger LoggerObj = NLog.LogManager.GetCurrentClassLogger();
        public Logger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "nlog.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        public void Info(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            LoggerObj.Info(message);
            Console.ForegroundColor = oldColor;
        }

        public void Success(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            LoggerObj.Info(message);
            Console.ForegroundColor = oldColor;
        }

        public void Error(string message, bool close = false)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            LoggerObj.Error(message);

            if( close )
                Console.WriteLine("Closing in 5 seconds...");

            Console.ForegroundColor = oldColor;

            if (close)
            {
                Thread.Sleep(5 * 1000);
                Environment.Exit(0);
            }
            
        }
    }
}
