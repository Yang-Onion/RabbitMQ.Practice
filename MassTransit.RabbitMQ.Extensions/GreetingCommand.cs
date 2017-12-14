using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.RabbitMQ.Extensions
{
    public class GreetingCommand
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
    }
}
