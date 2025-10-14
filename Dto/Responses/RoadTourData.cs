namespace FunAsiaGo.Dto.Responses;

public class RoadTourData
{
    public double Distance { get; set; } = 0;
    public string? Category { get; set; } = "city";
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public int Population { get; set; } = 7;
    public double Moving { get; set; } = 0;
}