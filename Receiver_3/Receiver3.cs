using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Receive
{
    class Receiver3
    {
        public static void Main()
        {
            var utils = new Send.Utils();
            utils.CreateQueConsumer();
        }
    }
}
