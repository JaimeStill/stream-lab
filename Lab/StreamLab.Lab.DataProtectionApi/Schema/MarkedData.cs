using StreamLab.Common.Schema;

namespace StreamLab.Lab.DataProtectionApi.Schema;
public abstract class MarkedData : Entity
{
    public int MarkId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;

    public Mark? Mark { get; set; }
}