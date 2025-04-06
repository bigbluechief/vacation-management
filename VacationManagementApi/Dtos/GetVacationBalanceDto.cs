namespace VacationManagementApi.Dtos;

public class GetVacationBalancyForEmployeesDto
{
    public List<int> EmployeeIds { get; set; } = new();
    public int Year { get; set; }
}
