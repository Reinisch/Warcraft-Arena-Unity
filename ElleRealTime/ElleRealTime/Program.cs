using System;
using System.Data.Common;
using System.Data.SqlClient;
using ElleFramework.Utils;
using ElleRealTime.Core.BO;
using ElleRealTime.Core.BO.World;
using ElleRealTime.Core.Configuration;
using ElleRealTime.Core.Logging;
using ElleRealTime.Shared.BO;
using ElleRealTime.Shared.DBEntities.PlayersInfo;
using Grpc.Core;
using MagicOnion.Server;
using MySql.Data.MySqlClient;

namespace ElleRealTime
{
    class Program
    {
        public static Logger Logger { get; set; }
        static void Main(string[] args)
        {
            Logger = new Logger();
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            var service = MagicOnionEngine.BuildServerServiceDefinition(isReturnExceptionStackTraceInErrorDetail: true);

            /*LOAD CONFIGURATION*/
            Configuration.ReadConfiguration();
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            Shared.BO.Utils.ElleRealTimeDB = new DB(ApplicationUtils.Configuration.Database.Type, ApplicationUtils.Configuration);



            var port = new ServerPort("localhost", 12345, ServerCredentials.Insecure);

            var server = new global::Grpc.Core.Server
            {
                Services = {service}
            };

            server.Ports.Add(port);

            server.Start();

            string line = "";
            do
            {
                line = Console.ReadLine();
                if (line != "quit")
                {
                    if (line.StartsWith(".createaccount"))
                    {
                        try
                        {
                            string[] parameters = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (parameters.Length == 3)
                            {
                                int id = Login.CreateAccount(parameters[1], parameters[2]);
                                if (id > 0)
                                {
                                    Logger.Success($"Successfully created the account \"{parameters[1]}\" with ID: {id}!");
                                }
                                else
                                {
                                    Logger.Error("An error occurred while creating the account!");
                                }
                            }
                            else
                            {
                                Logger.Error("Syntax error: .createaccount {username} {password}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.InnerException?.Message ?? ex.Message, true);
                        }
                    }

                    else if (line.StartsWith(".modifypassword"))
                    {
                        try
                        {
                            string[] parameters = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (parameters.Length == 3)
                            {
                                Login.ModifyPassword(parameters[1], parameters[2]);
                                Logger.Success($"Successfully modified the account \"{parameters[1]}\" with a new password!");
                            }
                            else
                            {
                                Logger.Error("Syntax error: .modifypassword {username} {newpassword}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.InnerException?.Message ?? ex.Message, true);
                        }
                    }
                    else if (line.StartsWith(".getplayerinfo"))
                    {
                        try
                        {
                            string[] parameters = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (parameters.Length == 2)
                            {
                                int accountId;
                                if (int.TryParse(parameters[1], out accountId))
                                {
                                    var bo = new Players();
                                    var playerInfo = bo.GetPlayerInfo(new PlayersInfoFilter { AccountID = accountId });
                                    if (playerInfo != null && playerInfo.Length > 0)
                                    {
                                        Logger.Success($"Position X: {playerInfo[0].PosX}");
                                        Logger.Success($"Position Y: {playerInfo[0].PosY}");
                                        Logger.Success($"Position Z: {playerInfo[0].PosZ}");
                                        Logger.Success($"Rotation X: {playerInfo[0].RotX}");
                                        Logger.Success($"Rotation Y: {playerInfo[0].RotY}");
                                        Logger.Success($"Rotation Z: {playerInfo[0].RotZ}");
                                    }
                                    else
                                    {
                                        Logger.Error($"No account with id={accountId} have data stored.");
                                    }
                                }
                                else
                                {
                                    Logger.Error($"Invalid account id: {parameters[1]}");
                                }
                            }
                            else
                            {
                                Logger.Error("Syntax error: .getplayerinfo {AccountID}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.InnerException?.Message ?? ex.Message, true);
                        }
                    }
                }

            } while (line != "quit");

        }
    }
}
