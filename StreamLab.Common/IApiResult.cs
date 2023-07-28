namespace StreamLab.Common;
public interface IApiResult
{
    string Message { get; set; }
    bool Error { get; set; }
    bool HasData { get; }
}