using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.RabbitMQ.Consumers;

public class PromoCodeConsumer : IPromoCodeConsumer, IAsyncDisposable
{
    private readonly RabbitMQSettings _settings;

    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;

    private const string _exchange = "PromoCodeQueue";

    public PromoCodeConsumer(RabbitMQSettings settings)
    {
        _settings = settings;

        Task.Run(async () =>
        {
            await StartAsync(new CancellationToken());
        });
    }

    public async Task StartAsync(CancellationToken ct)
    {
        if (_channel == null || _channel.IsClosed)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Host,
                VirtualHost = _settings.VHost,
                UserName = _settings.Login,
                Password = _settings.Password
            };

            var connection = await factory.CreateConnectionAsync(ct);
            _channel = await connection.CreateChannelAsync(null, ct);

            await _channel.QueueDeclareAsync(queue: _exchange,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += HandleMessageAsync;
        }

        await _channel.BasicConsumeAsync(queue: _exchange,
                                      autoAck: false,
                                      consumer: _consumer);
    }

    public async Task StopAsync(CancellationToken ct)
    {
        await _channel?.CloseAsync(ct);
        await _connection?.CloseAsync(ct);
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
            await _channel.BasicAckAsync(deliveryTag: args.DeliveryTag, multiple: false);
        }
        catch
        {
            await _channel.BasicNackAsync(args.DeliveryTag, false, true);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}
