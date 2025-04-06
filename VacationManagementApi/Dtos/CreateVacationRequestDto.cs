namespace VacationManagementApi.Dtos;

public class CreateVacationRequestDto
{
    public int EmployeeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Note { get; set; } = default!;
    public int ApproverAdminId { get; set; }
}
