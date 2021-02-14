using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Exchange.Topic.Consumer
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
                    channel.ExchangeDeclare(exchange: "topic-exchange-log", durable: true, type: ExchangeType.Topic);

                    var queueName = channel.QueueDeclare().QueueName;

                    //Info ile başlasın ortası herhangi bişey olabilir sonu Warning olsun diyorum
                    //string routingKey = "Info.*.Warning";
                    //Son noktadan sonraki kısım warning olsun diyorum
                    string routingKey = "#.Warning";

                    channel.QueueBind(queueName, exchange: "topic-exchange-log", routingKey: routingKey);


                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    Console.WriteLine("Custom log bekliyorum.......");

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var bodyByte = ea.Body.Span;
                        var log = Encoding.UTF8.GetString(bodyByte);

                        Console.WriteLine("Log alındı : " + log);

                        int time = int.Parse(GetMessage(args));

                        Thread.Sleep(time);
                        //txt dosyasına yazıyorum
                        File.AppendAllText("logs_critical_error.txt", log + "\n");
                        Console.WriteLine("Loglama işlemi sona erdi...");


                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    };
                    Console.WriteLine("Çıkış yapmak için tıklayınız..");
                    Console.ReadLine();
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0];
        }
    }
}
