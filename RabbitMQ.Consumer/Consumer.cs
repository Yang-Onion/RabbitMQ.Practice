using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer
{
   public class Consumer
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
                    channel.QueueDeclare("hello", false, false, false, null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" Consumer Received {0}", message);
                    };
                    channel.BasicConsume(queue: "hello",
                                 noAck: true,
                                 consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
