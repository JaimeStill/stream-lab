using System.Text;
using StreamLab.Common;
using StreamLab.Lab.DataProtectionApi.Data;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Services;
public class MarkedStringService : MarkedDataService<MarkedString>
{
    public MarkedStringService(MarkService markSvc, MarkContext db)
    : base(markSvc, db)
    { }

    public async Task<ApiResult<MarkedString>> Create(string data, string label, string marking)
    {
        Mark mark = await markSvc.GetByValue(marking);

        byte[] binary = Encoding.UTF8.GetBytes(data);

        MarkedString markedString = new()
        {
            MarkId = mark.Id,
            Payload = await Encrypt(binary, mark),
            Label = label
        };

        await set.AddAsync(markedString);
        await SaveChanges();

        return new(markedString, $"{markedString.Label} successfully created");
    }

    public async Task<ApiResult<string>> Decrypt(MarkedString data)
    {
        try
        {
            Mark mark = await markSvc.GetById(data.MarkId);
            byte[] binary = await Decrypt(data.Payload, mark);
            string result = Encoding.UTF8.GetString(binary);

            return new(result, $"{data.Label} successfully decrypted");
        }
        catch (Exception ex)
        {
            return new("Decrypt", ex);
        }
    }
}