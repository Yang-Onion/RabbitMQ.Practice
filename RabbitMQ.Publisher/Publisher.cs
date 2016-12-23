using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQ.Publisher
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
                    channel.ExchangeDeclare(exchange: "log", type: "fanout");
                    

                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange:"log",routingKey:"",mandatory:false,basicProperties:null,body:body);
                    Console.WriteLine($"Publisher send message to log-exchange. '{message}'");
                }
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "Default Message ";
        }
    }
}
