using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Exchange.Fanout.Consumer
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
                    //publisher tarafta yaptığı exchanger işlemini burada da gerçekleştiriyorum.
                    //channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "logs", durable: true, type: ExchangeType.Fanout);

                    //herbir instance ı ayağa kaldırdığımda kuyruk isimlerinin farklı olmasını istiyorum
                    //aşağıya kuyruk ismi oluşturuyorum

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queueName, exchange: "logs", routingKey: string.Empty);



                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    Console.WriteLine("logları bekliyorum.......");

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var bodyByte = ea.Body.Span;
                        var log = Encoding.UTF8.GetString(bodyByte);

                        Console.WriteLine("Log alındı : " + log);
                        int time = int.Parse(GetMessage(args));

                        Thread.Sleep(time);
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
