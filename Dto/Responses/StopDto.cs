namespace FunAsiaGo.Dto.Responses;

public class StopDto
{
    public string? CityId { get; set; }
    public string? TourId { get; set; }
    public string? Name { get; set; }
    public string? Time { get; set; }
    public int? Position { get; set; } = 1;
    public int? Dow { get; set; } = 0;
    public bool IsCityTour { get; set; }
    public bool IsJoin { get; set; }
    public bool IsLeave { get; set; }
    public string? PickupLocation { get; set; }
    public int? DayNo { get; set; }
    public string? Mp { get; set; }
    public string? Ml { get; set; }
    public string? Ma { get; set; }
    public string? Md { get; set; }
    public string? Mi { get; set; }
}