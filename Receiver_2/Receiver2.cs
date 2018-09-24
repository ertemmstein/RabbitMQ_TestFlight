using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Receive
{
    class Receiver2
    {
        public static void Main()
        {
            Send.Utils.CreateQueConsumer();
        }
    }
}
