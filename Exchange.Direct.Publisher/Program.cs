using RabbitMQ.Client;
using System;
using System.Text;

namespace Exchange.Direct.Publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Info = 3,
        Warning = 4
    }
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Kanal oluşturduktan sonra exchangeime bir isim veriyorum.
                    channel.ExchangeDeclare(exchange: "direct-exchange-log", durable: true, type: ExchangeType.Direct);

                    Array logNameArrays = Enum.GetValues(typeof(LogNames));

                    for (int i = 1; i < 11; i++)
                    {
                        Random rnd = new Random();

                        LogNames log = (LogNames)logNameArrays.GetValue(rnd.Next(logNameArrays.Length));

                        var bodyByte = Encoding.UTF8.GetBytes($"log = {log.ToString()}");


                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.BasicPublish(exchange: "direct-exchange-log", routingKey: log.ToString(), basicProperties: properties, body: bodyByte);

                        Console.WriteLine($"Log mesajı gönderilmiştir : {log.ToString()}");
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
