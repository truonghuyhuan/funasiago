namespace FunAsiaGo.Dto.Responses;

public class TourProductionDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Slug { get; set; }
    public int TourDuration { get; set; } = 7;
    public bool IsRetired { get; set; } = false;
    public bool AllowInterchanges { get; set; } = false;
    public bool IsFixed { get; set; } = false;
    public int DDow { get; set; } = 0;
    public string? BrandCode { get; set; }
    public string? Country { get; set; }
    public string? Compulsory { get; set; } = "Cruise";
    public List<TourDayDto>? Days { get; set; } = [];
    public List<MediaSource>? MediaSources { get; set; } = [];
}

public class TourCountryDto
{
    public Guid Id { get; set; }
    public string? Country { get; set; }
}