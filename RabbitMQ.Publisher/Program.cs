using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher
{
    class Program
    {
        //Bu yapılanlar ikinci versiyon içindir.
        static void Main(string[] args) //arguman kullandığımız için projeye dizinine powershelde açıp dotnet run mesaj diyorum ki çalışsın
        {
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://nyvihzls:rLqUu-fCPotv9ojMMNnrr7mbOH9oKFLS@eagle.rmq.cloudamqp.com/nyvihzls");
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Öncelikle kuyruğun ismini değiştiriyorum
                    //Mesaj sağlama almak için durable i true ya alıyorum
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, arguments: null);
                    //string message = "Hello World";
                    //Amacım 1 değil 10 tane kuyruğa mesaj göndermek o yüzden for döngüsü içerisine yazıyorum
                    string message = GetMessage(args);
                    for (int i = 1; i < 11; i++)
                    {
                        var bodyByte = Encoding.UTF8.GetBytes($"{message}-{i}");


                        //Kuyruğu sağlama aldım durable ile şimdi de mesajı sağlama almam lazım
                        //Mutlaka ve mutlaka ikisini de sağlama almam gerekiyor.
                        //RoutingKeyi de channeldaki kuyruğun ismi ile aynı yapıyorum
                        //Yukarıda tanımladığım basic propertiesi basic publishe set ediyorum
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        //Artık instance miz çökse bile mesaj kuyrukta durcak basicProperties ve yukarıda kuyruğu tanımlarken kullandığımız
                        //durable true sebebi ile
                        channel.BasicPublish("", routingKey: "task_queue", basicProperties: properties, body: bodyByte);

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
