using Newtonsoft.Json;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.RabbitMQ.Consumers;

public class PromoCodeConsumer : IPromoCodeConsumer, IAsyncDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly IRepository<Customer> _customersRepository;
    private readonly IRepository<PromoCode> _promoCodesRepository;
    private readonly IRepository<Preference> _preferencesRepository;

    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;

    private const string _exchange = "PromoCodeQueue";

    public PromoCodeConsumer(RabbitMQSettings settings)
    {
        _settings = settings;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        Console.WriteLine("StartAsync");

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
            _consumer.ReceivedAsync += HandlePromoCodeMessageAsync;
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

    private async Task HandlePromoCodeMessageAsync(object sender, BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
            var promoCodeDto = JsonConvert.DeserializeObject<PromoCodeRabbitDto>(message);
            await _channel.BasicAckAsync(deliveryTag: args.DeliveryTag, multiple: false);

            if (promoCodeDto != null)
            {
                Console.WriteLine($"Received PromoCode: {promoCodeDto.Code}, PartnerId: {promoCodeDto.PartnerId}");

                var preference = await _preferencesRepository.GetByIdAsync(promoCodeDto.PreferenceId);
                if (preference == null)
                    return;

                var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

                var promoCode = PromoCodeRabbitDtoMapper.MapFromModel(promoCodeDto, preference, customers);
                await _promoCodesRepository.AddAsync(promoCode);

                Console.WriteLine($"Handle promocode success!");
            }
            else
            {
                Console.WriteLine("Received PromoCode = null");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
            await _channel.BasicNackAsync(args.DeliveryTag, false, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
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
