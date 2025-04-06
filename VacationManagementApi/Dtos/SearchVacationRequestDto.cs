using VacationManagementApi.Enums;

namespace VacationManagementApi.Dtos;

public class SearchVacationRequestDto
{
    public int Year { get; set; }
    public int? Month { get; set; }
    public HashSet<string>? Departments { get; set; }
    public HashSet<string>? Roles { get; set; }
    public HashSet<VacationRequestStatus>? Statuses { get; set; }
    public bool ExcludeInactiveEmployees { get; set; } = true;
}
