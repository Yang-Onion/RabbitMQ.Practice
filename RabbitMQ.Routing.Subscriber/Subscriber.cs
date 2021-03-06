﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Routing.Subscriber
{
    public class Subscriber
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_exchange",
                                            type: "direct");

                    var queueName = channel.QueueDeclare().QueueName;

                    if (args.Length<1)
                    {
                        Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",Environment.GetCommandLineArgs()[0]);
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                        Environment.ExitCode = 1;
                        return;
                    }
                    foreach (var routingKey in args)
                    {
                        Console.WriteLine($" routingKey :{routingKey}");
                        channel.QueueBind(queue: queueName,
                                          exchange: "direct_exchange",
                                          routingKey: routingKey);
                    }

                    Console.WriteLine(" Subscriber Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;

                        Console.WriteLine($" RabbitMQ.Routing.Subscriber routingKeY:{routingKey}, Receive Messge: {message}");
                    };

                    Console.WriteLine($"queueName:{queueName}");
                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
