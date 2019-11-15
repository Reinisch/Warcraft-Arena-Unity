using System;
using System.Collections.Generic;
using System.Text;
using ElleFramework.Utils;
using ElleFramework.Utils.Utility;
using Newtonsoft.Json;

namespace ElleRealTime.Core.Configuration
{
    public class Configuration
    {
        public static void ReadConfiguration()
        {
            if (System.IO.File.Exists("config.json"))
            {
                string configFileContent = System.IO.File.ReadAllText("config.json");
                try
                {
                    ApplicationUtils.Configuration = JsonConvert.DeserializeObject<Config>(configFileContent);
                    Program.Logger.Info("Configuration file read.");
                }
                catch (Exception ex)
                {
                    Program.Logger.Error(ex.InnerException?.Message ?? ex.Message, true);
                }
            }
            else
            {
                string message = "Configuration files does not exists. Please create it and restart the application.";
                Program.Logger.Error(message, true);
            }
        }
    }
}
