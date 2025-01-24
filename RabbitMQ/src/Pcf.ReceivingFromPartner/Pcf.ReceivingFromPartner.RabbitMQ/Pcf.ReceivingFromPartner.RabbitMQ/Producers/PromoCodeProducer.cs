using Newtonsoft.Json;
using Pcf.ReceivingFromPartner.RabbitMQ.Producers.Interface;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Pcf.ReceivingFromPartner.RabbitMQ.Producers;

public class PromoCodeProducer : IPromoCodeProducer, IAsyncDisposable
{
    private readonly IOptions<RabbitMQSettings> _rabbitMqSettings;

    private const string _exchange = "PromoCodeQueue";
    private const string _routingKey = "";

    private IChannel _channel;

    public PromoCodeProducer(IOptions<RabbitMQSettings> rabbitMqSettings)
    {
        _rabbitMqSettings = rabbitMqSettings;
    }

    public async Task SendMessageAsync<T>(T message, CancellationToken ct)
    {
        if (_channel == null || _channel.IsClosed)
        {
            await InitializeAsync(ct);
        }

        var json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        });

        var content = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            _exchange,
            _routingKey,
            content,
            ct);
    }

    private async Task InitializeAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqSettings.Value.Host,
            VirtualHost = _rabbitMqSettings.Value.VHost,
            UserName = _rabbitMqSettings.Value.Login,
            Password = _rabbitMqSettings.Value.Password
        };

        var connection = await factory.CreateConnectionAsync(ct);
        _channel = await connection.CreateChannelAsync(null, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }
    }
}