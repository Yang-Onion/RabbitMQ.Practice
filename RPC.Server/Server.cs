using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPC.Server
{
    public class Server
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
                    //定义一个queue来接收client断的请求
                    channel.QueueDeclare(
                        queue: "rpc_queue",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    channel.BasicQos(0,1,false);

                    //定义一个consumer来处理(消费)上面那个queue中的请求
                    var consumer =new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: "rpc_queue", noAck: false, consumer: consumer);
                    Console.WriteLine($"RPC Server waitting for request...");

                    consumer.Received += (model, ea) =>
                    {
                        string response = string.Empty;
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
                            Console.WriteLine(" [.] " + ex.Message);
                            response = "";
                        }
                        finally
                        {
                            //consumer处理了client的请求后将请求结果
                            var responseBody = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(exchange:"",routingKey:replyProps.ReplyTo,basicProperties:replyProps,body:responseBody);
                            channel.BasicAck(deliveryTag:ea.DeliveryTag,multiple:true);

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
                return num;
            return Fib(num - 1) + Fib(num - 2);

        }
    }
}
