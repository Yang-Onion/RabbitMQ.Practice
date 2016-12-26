﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQ.Topics.Publisher
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
                    channel.ExchangeDeclare(exchange:"topic_logs",type: "topic");
                    var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
                    var message = (args.Length > 1)? string.Join(" ", args.Skip(1).ToArray()): "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "topic_logs",
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
                }
            }
        }
    }
}
