using System.ComponentModel.DataAnnotations.Schema;

namespace StreamLab.Lab.DataProtectionApi.Schema;

[Table("MarkedObject")]
public class MarkedObject : MarkedData
{
    public string DataType { get; set; } = string.Empty;
}