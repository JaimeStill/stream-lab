using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using StreamLab.Common;
using StreamLab.Lab.DataProtectionApi.Data;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Services;
public abstract class MarkedDataService<T> where T : MarkedData
{
    protected readonly MarkService markSvc;
    protected DbSet<T> set;
    protected Func<Task<int>> SaveChanges;

    public MarkedDataService(MarkService markSvc, MarkContext db)
    {
        this.markSvc = markSvc;
        set = db.Set<T>();
        SaveChanges = async () => await db.SaveChangesAsync();
    }

    protected Aes GetAes(byte[] key, byte[] iv)
    {
        Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        return aes;
    }

    public async Task<List<T>> Get(int markId) =>
        await set
            .Where(x => x.MarkId == markId)
            .OrderBy(x => x.Label)
            .ToListAsync();

    public async Task<T> GetById(int id) =>
        await set.FindAsync(id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} does not have a record with ID {id}");

    public async Task<ApiResult<int>> Remove(T data)
    {
        try
        {
            set.Remove(data);
            int result = await SaveChanges();

            return result > 0
                ? new(data.Id, $"{typeof(T).Name} successfully removed")
                : new("Remove", new Exception("The operation was not successful"));
        }
        catch (Exception ex)
        {
            return new("Remove", ex);
        }
    }

    protected async Task<string> Encrypt(byte[] data, Mark mark)
    {
        byte[] key = await markSvc.GetToken(mark.Value);
        using Aes aes = GetAes(key, Convert.FromBase64String(mark.Vector));

        using MemoryStream output = new();
        using CryptoStream crypt = new(
            output,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write
        );

        await crypt.WriteAsync(data);
        await crypt.FlushFinalBlockAsync();

        return Convert.ToBase64String(output.ToArray());
    }

    protected async Task<byte[]> Decrypt(string data, Mark mark)
    {
        byte[] key = await markSvc.GetToken(mark.Value);

        using Aes aes = GetAes(key, Convert.FromBase64String(mark.Vector));

        using MemoryStream output = new();
        using CryptoStream crypt = new(
            output,
            aes.CreateDecryptor(),
            CryptoStreamMode.Write
        );

        byte[] binary = Convert.FromBase64String(data);

        await crypt.WriteAsync(binary, 0, binary.Length);
        await crypt.FlushFinalBlockAsync();

        return output.ToArray();
    }
}