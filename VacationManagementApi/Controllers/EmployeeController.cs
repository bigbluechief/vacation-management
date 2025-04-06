using Microsoft.AspNetCore.Mvc;
using VacationManagementApi.Enums;
using VacationManagementApi.Services;
using VacationManagementApi.Dtos;
using VacationManagementApi.Models;
using Microsoft.Extensions.Localization;

namespace VacationManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController(EmployeeService vacationService, IStringLocalizer<EmployeeController> localizer) : ControllerBase
{

    private readonly EmployeeService _vacationService = vacationService;
    private readonly IStringLocalizer<EmployeeController> _localizer = localizer;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
    {
        var employees = await _vacationService.GetAllEmployeesAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        var employee = await _vacationService.GetEmployeeByIdAsync(id);

        if (employee == null)
            return NotFound(_localizer["EmployeeNotFound", id].Value);

        return Ok(employee);
    }

    [HttpPost("vacation-balance")]
    public async Task<ActionResult<List<VacationBalance>>> GetVacationBalanceByYearForEmployees(GetVacationBalancyForEmployeesDto dto)
    {
        var (error, balances) =
            await _vacationService.GetVacationBalancesForEmployeesByYearAsync(dto.EmployeeIds, dto.Year);

        if (error != VacationBalanceError.None)
        {
            return error switch
            {
                VacationBalanceError.InvalidEmployeeIds => BadRequest(_localizer["InvalidEmployeeIds"].Value),
                VacationBalanceError.EmployeesNotFound => NotFound(_localizer["EmployeesNotFound"].Value),
                VacationBalanceError.BalancesNotFound => NotFound(_localizer["NoVacationBalanceFoundForYear", dto.Year].Value),
                _ => StatusCode(500, _localizer["UnexpectedErrorOccured"].Value)
            };
        }

        return Ok(balances);
    }

    [HttpGet("{id}/vacation-balance/{year}")]
    public async Task<ActionResult<VacationBalance>> GetVacationBalanceByYearForEmployee(int id, int year)
    {
        var (error, balance) = await _vacationService.GetVacationBalancesForEmployeeByYearAsync(id, year);

        if (error != VacationBalanceError.None)
        {
            return error switch
            {
                VacationBalanceError.EmployeeNotFound => NotFound(_localizer["EmployeeNotFound", id].Value),
                _ => StatusCode(500, _localizer["UnexpectedErrorOccured"].Value)
            };
        }

        return Ok(balance);
    }
}
