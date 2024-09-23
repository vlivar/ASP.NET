using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Requests;
using PromoCodeFactory.WebHost.Models.Responses;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IRepository<Employee> _employeeRepository;

    public EmployeesController(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    /// <summary>
    /// Получить данные всех сотрудников
    /// </summary>
    /// <returns>Краткую информацию о сотрудниках</returns>
    [HttpGet]
    public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();

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
    /// Получить данные сотрудника по Id
    /// </summary>
    /// <returns>Полную информацию о сотруднике</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return NotFound($"Employee not found id={id}");

        var employeeModel = new EmployeeResponse()
        {
            Id = employee.Id,
            Email = employee.Email,
            Roles = employee.Roles.Select(x => new RoleItemResponse()
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id
            }).ToList(),
            FullName = employee.FullName,
            AppliedPromocodesCount = employee.AppliedPromocodesCount
        };

        return employeeModel;
    }

    /// <summary>
    /// Добавить нового сотрудника
    /// </summary>
    /// <returns>Id нового сотрудника</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateEmployeeAsync(EmployeeRequest employeeRequest)
    {
        var employee = new Employee()
        {
            LastName = employeeRequest.LastName,
            FirstName = employeeRequest.FirstName,
            Email = employeeRequest.Email,
            Roles = employeeRequest.Roles.Select(Converter.ConverterRole).ToList(),
            Id = Guid.NewGuid()
        };

        var gId = await _employeeRepository.CreateAsync(employee);
        return gId;
    }

    /// <summary>
    /// Обновить данные сотрудника
    /// </summary>
    /// <returns>Полную информацию о сотруднике</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, EmployeeRequest employeeRequest)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return NotFound($"Employee not found id={id}");

        var newDataEmployee = new Employee()
        {
            LastName = employeeRequest.LastName ?? employee.LastName,
            FirstName = employeeRequest.FirstName ?? employee.FirstName,
            Email = employeeRequest.Email ?? employee.Email,
            Roles = employeeRequest.Roles == null 
                    ? employee.Roles
                    : employeeRequest.Roles.Select(Converter.ConverterRole).ToList(),
            Id = employee.Id
        };

        var newEmployee = await _employeeRepository.UpdateAsync(newDataEmployee);

        var newEmployeeModel = new EmployeeResponse()
        {
            Id = newEmployee.Id,
            Email = newEmployee.Email,
            Roles = newEmployee.Roles.Select(x => new RoleItemResponse()
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id
            }).ToList(),
            FullName = newEmployee.FullName,
            AppliedPromocodesCount = newEmployee.AppliedPromocodesCount
        };

        return newEmployeeModel;
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    /// <returns>Успех операции</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteEmployeeAsync(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return NotFound($"Employee not found id={id}");

        var isSuccess = await _employeeRepository.DeleteAsync(id);
        return isSuccess;
    }
}