using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Rabbitmq clientına bağlanmak için bir tane factory oluşturuyorum.
            //factory.Uri = new Uri(""); // Burası bağlanacağım url ben locale bağlandım
            //Bağlantı açıyorum alttaki kodda
            using (var connection = factory.CreateConnection())
            {
                //kanal oluşturmam gerekiyor ve kanal oluşturma kodları
                using (var channel = connection.CreateModel())
                {
                    //Publisher da kuyruğu nasıl oluşturduysak aynı şekilde burada da kuyruğu o şekilde oluşturmalıyız.
                    //Queue methodunda belirttiğimiz methodların hepsi aynı olmak zorunda yoksa eşleşemedeği için
                    //İşlemimiz gerçekleşmeyecektir
                    channel.QueueDeclare(queue: "helloworld", durable: false, exclusive: false, arguments: null);

                    //Kuyruktaki mesajları alması için bir event oluşturuyorum. Ve dinleyeceği kanalı belirtiyorum.
                    var consumer = new EventingBasicConsumer(channel); // Kanalı bu şekilde dinletiyorum.
                    channel.BasicConsume(queue: "helloworld", true, consumer); //autoack true verirsek otomatik olarak doğru da yanlışta olsa kuyruktan sil. Eğer true dersek rabbitmq ya ben belirtcem sil diye 
                    //Mesajı ne zaman yakalayacağım.Kuyruktaki mesajları alıyorum.
                    consumer.Received += (model, ea) =>
                    {
                        var bodyByte = ea.Body.Span; //Kuyruktaki göndermiş olduğum mesajı alıyorum. String göndermiştim. Stringe çevirmeliyim. Eğerki model gönderseydim serilize deseriliaze yapmalıydım
                        var message = Encoding.UTF8.GetString(bodyByte);
                        Console.WriteLine("Mesaj alındı : " + message);
                    };
                }

                Console.WriteLine("Çıkış yapmak için tıklayınız..");
                Console.ReadLine();
            }
        }
    }
}
