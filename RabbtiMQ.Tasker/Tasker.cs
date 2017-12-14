using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbtiMQ.Tasker
{
    public class Tasker
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName="localhost"
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    
                    //创建一个名为Task_Queue的队列,durable:true 重启电脑或RabbitMQ后【队列】不会丢失
                    channel.QueueDeclare("Task_Queue", durable:true,exclusive:false,autoDelete: false,arguments: null);

                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);
                    
                    //消息持久化,它会将消息持久化到硬盘，重启电脑或RabbitMQ后【消息】不会丢失
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish("", "Task_Queue", false,properties,body);

                    Console.WriteLine(" Tasker Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ",args) : "Default Message ";
        }

    }
}
