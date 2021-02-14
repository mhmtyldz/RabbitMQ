using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Exchange.Direct.Consumer_Info_Warning
{
    class Program
    {
        public enum LogNames
        {
            Info,
            Warning
        }
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct-exchange-log", durable: true, type: ExchangeType.Direct);

                    var queueName = channel.QueueDeclare().QueueName;

                    //Normalde 1 kuyruk bind ediyordum ancak şimdi hem critical hemde erroru kuyruklarını bind edeceğim için
                    //iki tane kuyruk bind ediyorum o yüzden foreach ile dönüyorum

                    foreach (var log in Enum.GetNames(typeof(LogNames)))
                    {
                        channel.QueueBind(queueName, exchange: "direct-exchange-log", routingKey: log);
                    }


                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    Console.WriteLine("Critical ve error logları bekliyorum.......");

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
                        Console.WriteLine("Bu log : ", log + "\n");
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
