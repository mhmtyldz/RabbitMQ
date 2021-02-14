using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.Consumer
{
    class Program
    {
        //Bu yapılanlar ikinci versiyon içindir.
        static void Main(string[] args)
        {
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://nyvihzls:rLqUu-fCPotv9ojMMNnrr7mbOH9oKFLS@eagle.rmq.cloudamqp.com/nyvihzls");
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, arguments: null);

                    //Eşit dağılım işlemini anlatmıştık. Eşit dağılım işlemi için aşağıdaki methodu kullanıyorum
                    //PrefetchCount sen bana 1 tane mesaj ver ben sana bu mesajı doğru şekilde hallettikten sonra sen bana 1 tane daha ver
                    //Aynı anda bana 2 tane mesaj gönderme diyorum  eğer 3 dersem prefetchCounta 3 tane aynı anda mesaj alır
                    //global false deme sebebimiz tek bir seferde bir tane mesaj alsın istiyorum prefetchCount 2 dersem tek bir seferde iki mesaj gelir
                    //Global true dersem 5 tane instancem var prefetchCountta da 10 dedim eğerki global : true ise 5 instancem hepsi tplam 10 message alabilir.
                    //false dersemde 5 instancem tek seferde 10 tane alabilir demek.
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    Console.WriteLine("mesakları bekliyorum.......");

                    var consumer = new EventingBasicConsumer(channel);
                    //Autoact özelliği var eğerki true olursa mesaj kuyruktan silinir. Ancak biz false yapıyoruz
                    //Sebebi şu ben bilgi göndericem sen gönderme.  artık mesajı silebilrsin kuyruktan diye ben belirtmek için false yapıyorum. 
                    channel.BasicConsume(queue: "task_queue", autoAck: false, consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var bodyByte = ea.Body.Span;
                        var message = Encoding.UTF8.GetString(bodyByte);

                        Console.WriteLine("Mesaj alındı : " + message);
                        int time = int.Parse(GetMessage(args));
                        //Thread kullanmamın sebebi Bizim consumerlarımızın iyi kötü makine de çalıştığınu simule etmek adına
                        //Uygulamayı uyutuyorum. Yeni bir instance oluşturcam

                        Thread.Sleep(time);
                        Console.WriteLine("Mesaj işlendi...");

                        //Mesajın işlendiğini brokera göndermem gerekiyor 
                        //Alt satırdaki kod. Ben mesajı işledim sen artık bu mesajı kuyruktan silebilirsin anlamına geliyor

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
