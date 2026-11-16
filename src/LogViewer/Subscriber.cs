using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogViewer
{
    public class Subscriber
    {
        private readonly IConfiguration _config;

        // Pasamos la configuración por el constructor (leída del appsettings.json)
        public Subscriber(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync()
        {
            // 1. Crear la conexión
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:HostName"],
                Port = int.Parse(_config["RabbitMQ:Port"]!),
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"]
            };

            var connection = await factory.CreateConnectionAsync();
            
            // 2. Crear un canal
            var _channel = await connection.CreateChannelAsync();

            var _exchangeName = _config["RabbitMQ:Exchange"];

            // 3. Declarar exchange
            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);

            // 4. Declarar cola efímera/exclusiva y recuperar su nombre
            var tempQueue = await _channel.QueueDeclareAsync();
            var queueName = tempQueue.QueueName;

            // 5. Binding cola-exchange (vincularlos)
            await _channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: string.Empty);

            Console.WriteLine($"[*] Esperando los logs en la cola efímera: {queueName}...");

            // 6. Crear consumidor y configurar callback
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    // Deserializamos usando dynamic como sugiere la práctica
                    dynamic? log = JsonSerializer.Deserialize<dynamic>(message);
                    Console.WriteLine($"[x] NUEVO LOG: {message}"); // Aquí puedes formatearlo más bonito si quieres
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al deserializar: {ex.Message}");
                }

                await Task.CompletedTask;
            };

            // 7. Iniciar consumo
            await _channel.BasicConsumeAsync(
                queue: queueName, 
                autoAck: true, 
                consumer: consumer
            );

            Console.WriteLine(" Presiona [Enter] para salir del LogViewer.");
            Console.ReadLine();

            // Cierre seguro
            await _channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}