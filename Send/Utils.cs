using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Send
{
    public  class Utils
    {
        private static string queName;
        private static string routingKeyName;
        private static string exchangeName;


        public Utils()
        {
            //Fill these mandatory Fields
            queName = "ertemtestQueue";
            routingKeyName = "taskQue";
            exchangeName = "ertemTestExchange";
        }
        public ConnectionFactory CreateConFactory()
        {
            var factory = new ConnectionFactory()
            {
                //Fill the connection info's
                HostName = "apps.erc-grup.com.tr",
                Port = 8545,
                VirtualHost = "ertemyHost",
                UserName = "ertemy",
                Password = "ertemy"

            };

            return factory;
        }

        public void  CreateQueConsumer(bool pauseThread = false)
        {
           

            var factory = CreateConFactory();
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    
                    channel.QueueDeclare(queue: queName,
                                   durable: true,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);


                   

                    //Oos for Consumers
                    //Fair Dispatch
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    Console.WriteLine(" [*] Waiting for messages.");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                        Console.WriteLine(" [x] Done");
                        if (pauseThread) // Acts as slower consumer
                            Thread.Sleep(1000);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                   
                    channel.BasicConsume(queue: queName,
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }

        }

        public void CreateQuePublisher()
        {
             var factory = CreateConFactory();
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true; //If the consumers fails the server keeps the queues


                    //Fill the Queue Name
                    channel.QueueDeclare(queue: queName,
                                        durable: true, //If the consumers fails the server keeps the messages
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                    bool stopFlag = true;
                    while (stopFlag)
                    {

                        Console.WriteLine("Type the message: ");
                        //var message = Console.ReadLine();
                        var message = GetMessage();
                        var body = Encoding.UTF8.GetBytes(message);

                        //to send messages as loop => "loop";[loopNumb(int)];[Message]
                        if (message.StartsWith("loop;"))
                        {
                            var splittedMsg = message.Split(';');
                            var loopEndInt = Convert.ToUInt32(splittedMsg[1]);
                            message = splittedMsg[2];
                            for (int i = 0; i < loopEndInt; i++)
                            {
                                body = Encoding.UTF8.GetBytes(message + " :" + (i + 1).ToString());
                                channel.BasicPublish(exchange: exchangeName, //You need to fill
                                                                                 routingKey: routingKeyName, //You have to create RoutingKey to send meessages to counsemers
                                                                                 basicProperties: null,
                                                                                 body: body);
                            }
                        }
                        else
                        {
                            channel.BasicPublish(exchange: exchangeName, //You need to fill
                                                 routingKey: routingKeyName,
                                                 basicProperties: null,
                                                 body: body);
                        }
                    }

                }


            }
        }

        private static string GetMessage()
        {
            var message = Console.ReadLine();
            return message;
            //return ((args.Length > 0) ? string.Join(" ", args) : message);

        }
    }
}
