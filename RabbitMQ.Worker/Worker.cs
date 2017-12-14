using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Worker
{
    class Worker
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
                    channel.QueueDeclare(queue: "Task_Queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    //每次只给consumer分配一个任务，当它Message Acknowledgment后再为它分配新任务.这样，就不会平均分配任务，导致某个worker累死，另一些闲死.
                    channel.BasicQos(prefetchSize:0,prefetchCount:1,global:false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" Worker Received {0}", message);
                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);

                        Console.WriteLine(" Worker Done");

                        channel.BasicAck(deliveryTag:ea.DeliveryTag,multiple:false);
                    };
                    //autoAck:false:当一条消息传给consumer A后，假如，consumer处理到一半突然挂掉了，它会造成消息丢失.
                    //如果,autoAck为true即需要消息确认.同上面的情况一样当consumer A挂到以后,消息不会丢失,此条消息还会传给consumer B.
                    channel.BasicConsume("Task_Queue", autoAck: true,consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
