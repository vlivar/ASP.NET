using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeesController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    /// <summary>
    /// Получить данные всех сотрудников
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<EmployeeShortResponse>> GetEmployeesAsync(CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);

        var employeesModelList = employees.Select(x =>
            new EmployeeShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
            }).ToList();

        return employeesModelList;
    }

    /// <summary>
    /// Получить данные сотрудника по id
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetAsync(id, cancellationToken);

        if (employee == null)
            return NotFound();

        var employeeModel = new EmployeeResponse()
        {
            Id = employee.Id,
            Email = employee.Email,
            Role = new RoleItemResponse()
            {
                Name = employee.Role.Name,
                Description = employee.Role.Description
            },
            FullName = employee.FullName,
            AppliedPromocodesCount = employee.AppliedPromocodesCount
        };

        return employeeModel;
    }
}
