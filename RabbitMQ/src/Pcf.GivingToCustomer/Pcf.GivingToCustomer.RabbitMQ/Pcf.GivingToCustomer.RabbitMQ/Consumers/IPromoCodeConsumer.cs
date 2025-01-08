using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.RabbitMQ.Consumers;

public interface IPromoCodeConsumer
{
    Task StartAsync(CancellationToken ct);
    Task StopAsync(CancellationToken ct);
}