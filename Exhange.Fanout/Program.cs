using RabbitMQ.Client;
using System;
using System.Text;

namespace Exhange.Fanout
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://nyvihzls:rLqUu-fCPotv9ojMMNnrr7mbOH9oKFLS@eagle.rmq.cloudamqp.com/nyvihzls");
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Artık ben direk queue ye göndermeyeceğim için (exchangeye göndercem)alttaki queueyi yorum satırına alıyorum
                    //channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, arguments: null);

                    channel.ExchangeDeclare(exchange: "logs", durable: true, type: ExchangeType.Fanout);

                    string message = GetMessage(args);
                    for (int i = 1; i < 11; i++)
                    {
                        var bodyByte = Encoding.UTF8.GetBytes($"{message}-{i}");


                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        //Yorum satırında bulunan basicpublish methodunda ilk örnekte excahnge belirtmiyordum artık belirtiyorum
                        //routingkeyi de siliyorum.
                        //channel.BasicPublish("", routingKey: "task_queue", basicProperties: properties, body: bodyByte);
                        channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: properties, body: bodyByte);

                        Console.WriteLine("Mesajınız Gönderilmiştir.");
                    }

                }

                Console.WriteLine("Çıkış yapmak için tıklayınız..");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0];
        }
    }
}
