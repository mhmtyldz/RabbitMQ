using RabbitMQ.Client;
using System;
using System.Text;

namespace Exchange.Topic.Publisher
{
    //Critical.Error.Info
    //Info.Warning.Critical
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
                    channel.ExchangeDeclare(exchange: "topic-exchange-log", durable: true, type: ExchangeType.Topic);

                    Array logNameArrays = Enum.GetValues(typeof(LogNames));

                    for (int i = 1; i < 11; i++)
                    {
                        Random rnd = new Random();

                        LogNames log1 = (LogNames)logNameArrays.GetValue(rnd.Next(logNameArrays.Length));
                        LogNames log2 = (LogNames)logNameArrays.GetValue(rnd.Next(logNameArrays.Length));
                        LogNames log3 = (LogNames)logNameArrays.GetValue(rnd.Next(logNameArrays.Length));

                        string routingKey = $"{log1}.{log2}.{log3}";

                        var bodyByte = Encoding.UTF8
                            .GetBytes($"log = {log1.ToString()} - {log2.ToString()} - {log3.ToString()}");

                        //Direct exchange de routingkeyi direk alıyodum
                        //Topic exchange de noktalı bir şekilde alcam
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.BasicPublish(exchange: "topic-exchange-log", routingKey: routingKey, basicProperties: properties, body: bodyByte);

                        Console.WriteLine($"Log mesajı gönderilmiştir : mesaj -> {routingKey}");
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
