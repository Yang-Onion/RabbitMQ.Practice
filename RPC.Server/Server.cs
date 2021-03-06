﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPC.Server
{
    public class Server
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName="localhost"
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //定义一个queue来接收client断的请求
                    channel.QueueDeclare("rpc_queue", false, false, false, null);
                    channel.BasicQos(0, 1, false);

                    //定义一个consumer来处理(消费)上面那个queue中的请求
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("rpc_queue", false, consumer);
                    Console.WriteLine($"RPC Server waitting for request...");

                    consumer.Received += (model, ea) =>
                    {
                        var response = string.Empty;
                        var requestBody = ea.Body;
                        var pops = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = pops.CorrelationId;
                        try
                        {
                            int num;
                            int.TryParse(Encoding.UTF8.GetString(requestBody), out num);
                            response = Fib(num).ToString();
                            Console.WriteLine(" RPC Server  Fib({0})", response);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(" client Exception: " + ex.Message);
                            response = "";
                        }
                        finally
                        {
                            //consumer处理了client的请求后将请求结果
                            var responseBody = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(exchange:"",
                                                 routingKey: replyProps.ReplyTo,
                                                 basicProperties:replyProps,
                                                 body: responseBody);
                            channel.BasicAck(deliveryTag:ea.DeliveryTag,multiple: false);
                        }
                    };
                }
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static int Fib(int num)
        {
            if (num == 0 || num == 1)
            {
                return num;
            }

            return Fib(num - 1) + Fib(num - 2);
        }
    }
}