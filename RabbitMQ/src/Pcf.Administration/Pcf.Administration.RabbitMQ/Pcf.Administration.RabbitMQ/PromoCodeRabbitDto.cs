using System;

namespace Pcf.Administration.RabbitMQ;

public class PromoCodeRabbitDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string ServiceInfo { get; set; }
    public string BeginDate { get; set; }
    public string EndDate { get; set; }
    public Guid PartnerId { get; set; }
    public Guid? PartnerManagerId { get; set; }
    public Guid PreferenceId { get; set; }
}
