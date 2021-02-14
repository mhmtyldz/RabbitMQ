using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Header.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "header-exchange-log", durable: true, type: ExchangeType.Headers);

                    //Bir tane properties oluşturuyorum
                    var properties = channel.CreateBasicProperties();

                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");

                    properties.Headers = headers;
                    Console.WriteLine("Mesaj gönderildi");
                    channel.BasicPublish(exchange: "header-exchange-log", routingKey: string.Empty, basicProperties: properties, body:
                        Encoding.UTF8.GetBytes("header mesajım"));


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
