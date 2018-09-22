using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Receive
{
    class Receiver
    {
        public static void Main()
        {
            var factory = new ConnectionFactory()
            {

                //Fill the connection info's
                HostName = "localhost",
                //Port = 8545,
                //VirtualHost = "",
                //UserName = "",
                //Password = ""

            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Fill the Queue Name
                channel.QueueDeclare(queue: "QueName",
                               durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                Console.WriteLine(" [*] Waiting for messages.");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);


                    //1000ms pause to test Fair dispatch queue between consumers 
                    Thread.Sleep(100);
                    Console.WriteLine(" [x] Done");
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                //Fill the Queue Name
                channel.BasicConsume(queue: "QueName",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
