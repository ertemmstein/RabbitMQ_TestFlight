using RabbitMQ.Client;
using System;
using System.Text;

namespace Send
{
    class Program
    {
        public static void Main(string[] args)
        {
            var utils = new Utils();
            utils.CreateQuePublisher();
        }

       
       
    }
}
