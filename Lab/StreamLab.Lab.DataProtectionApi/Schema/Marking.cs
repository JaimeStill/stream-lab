using System.ComponentModel.DataAnnotations.Schema;
using StreamLab.Common.Schema;

namespace StreamLab.Lab.DataProtectionApi.Schema;

[Table("Marking")]
public class Marking : Entity
{
    public string Value { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}