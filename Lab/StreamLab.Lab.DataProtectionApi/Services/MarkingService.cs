using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using StreamLab.Common;
using StreamLab.Common.Services;
using StreamLab.Lab.DataProtectionApi.Data;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Services;
public class MarkingService : EntityService<Marking, MarkingContext>
{
    private readonly IDataProtectionProvider provider;

    public MarkingService(MarkingContext db, IDataProtectionProvider provider)
    : base(db)
    {
        this.provider = provider;
    }

    private IDataProtector Protector(string marking) =>
        provider.CreateProtector(marking);

    protected override Func<Marking, Task<Marking>>? OnAdd => (Marking marking) =>
    {
        using RSA rsa = RSA.Create();
        marking.Token = Protector(marking.Value).Protect(rsa.ToXmlString(true));
        return Task.FromResult(marking);
    };

    async Task<Return> Execute<Return>(string marking, Func<Return> exec)
    {
        if (await set.AnyAsync(x => x.Value.ToLower() == marking.ToLower()))
            return exec();
        else
            throw new KeyNotFoundException($"No Marking {marking} was found");
    }

    public async Task<Marking?> GetByValue(string marking) =>
        await set
            .FirstOrDefaultAsync(x =>
                x.Value.ToLower() == marking.ToLower()
            );

    public async Task<string> GetToken(string marking)
    {
        Marking m = await GetByValue(marking)
            ?? throw new KeyNotFoundException($"No Marking {marking} was found");

        return await Unprotect(m.Value, m.Token);
    }

    public async Task<string> Protect(string marking, string payload) =>
        await Execute(marking, () =>
            Protector(marking).Protect(payload)
        );

    public async Task<string> Unprotect(string marking, string payload) =>
        await Execute(marking, () =>
            Protector(marking).Unprotect(payload)
        );

    public override async Task<ValidationResult> Validate(Marking marking)
    {
        ValidationResult result = new();

        if (string.IsNullOrWhiteSpace(marking.Value))
            result.AddMessage("Marking must have a value");

        if (await set.AnyAsync(x =>
            x.Id != marking.Id
            && x.Value.ToLower() == marking.Value.ToLower()
        ))
            result.AddMessage($"{marking.Value} is already registered");

        return result;
    }
}