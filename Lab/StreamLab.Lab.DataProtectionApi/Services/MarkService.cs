using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using StreamLab.Common;
using StreamLab.Common.Services;
using StreamLab.Lab.DataProtectionApi.Data;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Services;
public class MarkService : EntityService<Mark, MarkContext>
{
    private readonly IDataProtectionProvider provider;

    public MarkService(MarkContext db, IDataProtectionProvider provider)
    : base(db)
    {
        this.provider = provider;
    }

    private IDataProtector Protector(string marking) =>
        provider.CreateProtector(marking);

    protected override Func<Mark, Task<Mark>>? OnAdd => (Mark mark) =>
    {
        using Aes aes = Aes.Create();
        mark.Token = Convert.ToBase64String(Protector(mark.Value).Protect(aes.Key));
        mark.Vector = Convert.ToBase64String(aes.IV);
        return Task.FromResult(mark);
    };

    async Task<Return> Execute<Return>(string marking, Func<Return> exec)
    {
        if (await set.AnyAsync(x => x.Value.ToLower() == marking.ToLower()))
            return exec();
        else
            throw new KeyNotFoundException($"No Mark {marking} was found");
    }

    async Task<byte[]> Unprotect(string marking, byte[] payload) =>
        await Execute(marking, () =>
            Protector(marking).Unprotect(payload)
        );

    public async Task<Mark> GetByValue(string marking) =>
        await set
            .FirstAsync(x =>
                x.Value.ToLower() == marking.ToLower()
            )
        ?? throw new KeyNotFoundException($"No Mark {marking} was found");

    public async Task<byte[]> GetToken(string marking)
    {
        Mark m = await GetByValue(marking);
        return await Unprotect(m.Value, Convert.FromBase64String(m.Token));
    }

    public override async Task<ValidationResult> Validate(Mark mark)
    {
        ValidationResult result = new();

        if (string.IsNullOrWhiteSpace(mark.Value))
            result.AddMessage("Mark must have a value");

        if (await set.AnyAsync(x =>
            x.Id != mark.Id
            && x.Value.ToLower() == mark.Value.ToLower()
        ))
            result.AddMessage($"{mark.Value} is already registered");

        return result;
    }
}