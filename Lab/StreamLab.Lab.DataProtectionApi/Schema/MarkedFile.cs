using System.ComponentModel.DataAnnotations.Schema;

namespace StreamLab.Lab.DataProtectionApi.Schema;

[Table("MarkedFile")]
public class MarkedFile : MarkedData
{
    public string Filename { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
}