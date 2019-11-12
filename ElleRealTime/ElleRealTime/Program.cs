using System;
using Grpc.Core;
using MagicOnion.Server;

namespace ElleRealTime
{
    class Program
    {
        static void Main(string[] args)
        {
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            var service = MagicOnionEngine.BuildServerServiceDefinition(isReturnExceptionStackTraceInErrorDetail: true);

            var port = new ServerPort("localhost", 12345, ServerCredentials.Insecure);

            var server = new global::Grpc.Core.Server
            {
                Services = {service}
            };

            server.Ports.Add(port);

            server.Start();

            Console.ReadLine();
        }
    }
}
