using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Header.Consumer
{
    class Program
    {
        public enum LogNames
        {
            Critical,
            Error
        }
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "header-exchange-log", durable: true, type: ExchangeType.Headers);

                    channel.QueueDeclare("kuyruk1", false, false, false);

                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");
                    headers.Add("x-match", "all");

                    channel.QueueBind("kuyruk1", exchange: "header-exchange-log", string.Empty, arguments: headers);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("kuyruk1", autoAck: false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var bodyByte = ea.Body.Span;
                        var message = Encoding.UTF8.GetString(bodyByte);

                        Console.WriteLine("gelen mesaj : " + message);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    };
                    Console.WriteLine("Çıkış yapmak için tıklayınız..");
                    Console.ReadLine();
                }
            }
        }

    }
}
