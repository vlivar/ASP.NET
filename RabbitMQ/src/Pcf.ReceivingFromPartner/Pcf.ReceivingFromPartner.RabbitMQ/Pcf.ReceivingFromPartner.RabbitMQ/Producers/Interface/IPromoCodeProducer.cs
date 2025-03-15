using System.Threading;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.RabbitMQ.Producers.Interface;

public interface IPromoCodeProducer
{
    Task SendMessageAsync<T>(T message, CancellationToken ct);
}