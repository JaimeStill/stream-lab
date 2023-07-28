using System.ComponentModel.DataAnnotations.Schema;
using StreamLab.Common.Schema;

namespace StreamLab.Lab.DataProtectionApi.Schema;

[Table("Mark")]
public class Mark : Entity
{
    public string Value { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Vector { get; set; } = string.Empty;

    public ICollection<MarkedFile> Files { get; set; } = new List<MarkedFile>();
    public ICollection<MarkedObject> Objects { get; set; } = new List<MarkedObject>();
    public ICollection<MarkedString> Strings { get; set; } = new List<MarkedString>();
}