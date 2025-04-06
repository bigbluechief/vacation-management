using System.ComponentModel.DataAnnotations;

namespace VacationManagementApi.Models;

public class Administrator
{
    [Key]
    public int Id { get; set; }
    required public string Name { get; set; }
}
