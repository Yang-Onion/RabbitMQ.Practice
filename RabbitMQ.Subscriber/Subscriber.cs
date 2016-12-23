using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Subscriber
{
    public class Subscriber
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
                    channel.ExchangeDeclare(exchange:"log",type:"fanout");

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queueName,"log","");

                    Console.WriteLine($" queueName:{queueName} ");

                    Console.WriteLine(" Subscriber Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($" Subscriber receive message: {message}" );
                    };

                    channel.BasicConsume(queueName, true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
