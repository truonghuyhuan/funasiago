namespace FunAsiaGo.Dto.Responses;

public class TourDayDto
{
    public int Dow { get; set; } = 1;
    public int Position { get; set; } = 1;
    public List<StopDto>? Stops { get; set; } = [];
    public List<AttractionDto>? Attractions { get; set; } = [];
    public List<InterChange>? InterChanges { get; set; } = [];
    public double Distance { get; set; } = 0;
    public string? Description { get; set; }
    public bool? NeedCruise { get; set; } = false;
    public List<MediaSource>? MediaSource { get; set; } = [];
}