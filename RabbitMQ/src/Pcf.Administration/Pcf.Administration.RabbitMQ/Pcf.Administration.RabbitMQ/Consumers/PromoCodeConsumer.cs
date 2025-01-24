using MassTransit;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.RabbitMQ.Consumers;

public class PromoCodeConsumer : IConsumer<PromoCodeRabbitDto>
{
    private readonly IRepository<Employee> _employeeRepository;

    public PromoCodeConsumer(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task Consume(ConsumeContext<PromoCodeRabbitDto> context)
    {
        var employeeId = context.Message.Id;
        var employee = await _employeeRepository.GetByIdAsync(employeeId);

        if (employee == null)
        {
            Console.WriteLine($"Employee by Id = {employeeId} not found");
            return;
        }

        employee.AppliedPromocodesCount++;
        await _employeeRepository.UpdateAsync(employee);
    }
}
