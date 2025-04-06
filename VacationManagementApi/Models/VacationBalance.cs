using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManagementApi.Models;

public class VacationBalance
{
    [Key]
    public int Id { get; set; }
    [ForeignKey(nameof(Employee))]
    required public int EmployeeId { get; set; }
    required public int Year { get; set; }
    required public int DaysUsed { get; set; }
    required public int DaysRemaining { get; set; }

    public Employee? Employee { get; set; }
}