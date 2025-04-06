using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VacationManagementApi.Models;
using VacationManagementApi.Policies;
using VacationManagementApi.Services;

namespace VacationManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController(CompanyService companyService, IStringLocalizer<EmployeeController> localizer) : ControllerBase
{

    private readonly CompanyService _companyService = companyService;
    private readonly IStringLocalizer<EmployeeController> _localizer = localizer;

    [HttpGet("{id}/vacation-days/{year:int}")]
    public async Task<ActionResult<CompanyVacationPolicy>> GetPublicVacationDays(int id, int year)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);

        if (company == null)
            return NotFound(_localizer["CompanyNotFound", id].Value);

        var policy = VacationPolicyFactory.For(company);

        return Ok(policy.GetPublicVacationDays(year));
    }

}
