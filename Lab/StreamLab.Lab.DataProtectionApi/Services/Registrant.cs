using StreamLab.Common.Services;

namespace StreamLab.Lab.DataProtectionApi.Services;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddScoped<MarkingService>();
    }
}