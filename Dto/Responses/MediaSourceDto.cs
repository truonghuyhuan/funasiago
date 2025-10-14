namespace FunAsiaGo.Dto.Responses;

public class MediaSource
{
    public string? Path { get; set; }
    public int? Type { get; set; } = 0;
    public int? Scope { get; set; } = 0;
    public int? No { get; set; } = 1;
    public bool Hide { get; set; } = false;
}