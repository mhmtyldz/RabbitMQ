using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher
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
                    channel.QueueDeclare(queue: "helloworld", durable: false, exclusive: false, arguments: null); //Kuyruk oluşturuyorum. ikinci parametrem durable, 
                    //durable false ise rabbitmq restart atarsa mesajlarımın hepsi uçar.
                    //(Memoryde duruyor şuan çünkü daha hızlı olması için) ama true dersek 
                    //disktte tutcak ve hiçb irşekilde mesaklarım restart durumunda kaybolmicak
                    //exclusive bu kuyruğa sadece 1 kanal mı yoksa başka kanallarda bağlanabilsin mi demek istiyoruz. False dersek diğerleri de bağlanır.
                    //autodelete bir kuyrukta 20 mesaj var son mesajda kuyruktan çıkarsa bu kuyruk silinsin mi silinmesin mi demek istiyoruz. 
                    //false silinsin true silinmesin.
                    string message = "Hello World";
                    //Mesajlarımı byte olarak göndermemiz lazım. Word pdf image text herşeyi byte çevirip gönderebilieceğimiz için stringi byte a çevircem
                    var bodyByte = Encoding.UTF8.GetBytes(message);

                    //Alt satırdaki kodda mesajı kuyruğuma gönderiyorum.
                    channel.BasicPublish("", routingKey: "helloworld", null, body: bodyByte);//exchange kullanmadığımız için boş geçiyorum default exchange geçsin.

                    Console.WriteLine("Mesajınız Gönderilmiştir.");
                }

                Console.WriteLine("Çıkış yapmak için tıklayınız..");
                Console.ReadLine();
            }
        }
    }
}
