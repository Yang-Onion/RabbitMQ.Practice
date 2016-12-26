using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQ.Routing.Publisher
{
    public class Publisher
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.2.122",
                UserName = "Yang",
                Password = "cms2016...",
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange:"direct_exchange",type:"direct");
                    var routingKey = args.Length > 0 ? args[0] : "info";
                    Console.WriteLine($"routingKey:{routingKey}");

                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange:"direct_exchange",
                                         routingKey: routingKey,
                                         basicProperties:null,
                                         body:body);
                    Console.WriteLine($" The RabbitMQ.Routing.Publisher Send {message}");
                }
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }

        private static string GetMessage(string[] args)
        {
            return (args.Length > 1)? string.Join(" ", args.Skip(1).ToArray()): "Hello World!";
        }
    }
}
