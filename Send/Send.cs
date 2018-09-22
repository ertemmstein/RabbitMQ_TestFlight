using RabbitMQ.Client;
using System;
using System.Text;

namespace Send
{
    class Program
    {
        public static void Main(string[] args)
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
            {
                using (var channel = connection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true; //If the consumers fails the server keeps the queues


                    //Fill the Queue Name
                    channel.QueueDeclare(queue: "QueName",
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
                                channel.BasicPublish(exchange: "ExchangeName", //You need to fill
                                                                                 routingKey: "routingKeyName", //You have to create RoutingKey to send meessages to counsemers
                                                                                 basicProperties: null,
                                                                                 body: body);
                            }
                        }
                        else
                        {
                            channel.BasicPublish(exchange: "ExchangeName", //You need to fill
                                                 routingKey: "routingKeyName",
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
