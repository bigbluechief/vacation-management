using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VacationManagementApi.Enums;

namespace VacationManagementApi.Models;

public class VacationRequest
{
    [Key]
    public int Id { get; set; }
    [ForeignKey(nameof(Employee))]
    required public int EmployeeId { get; set; }
    required public DateOnly StartDate { get; set; }
    required public DateOnly EndDate { get; set; }
    required public VacationRequestStatus Status { get; set; }
    required public string Note { get; set; }
    [ForeignKey(nameof(Administrator))]
    required public int ApproverAdminId { get; set; }

    public Employee Employee { get; set; } = default!;
    public Administrator Administrator { get; set; } = default!;
}
