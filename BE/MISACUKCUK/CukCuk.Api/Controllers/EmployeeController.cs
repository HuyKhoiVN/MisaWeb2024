using CukCuk.Core.Entities;
using CukCuk.Core.Exceptions;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using CukCuk.Core.Services;
using CukCuk.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CukCuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        IEmployeeService _employeeService;
        IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository, IEmployeeService employeeService)
        {
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var employees = _employeeRepository.GetAll();
            return Ok(employees);
        }

        [HttpGet("{employeeId}")]
        public IActionResult GetById(Guid employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);
            return Ok(employee);
        }

        [HttpPost]
        public IActionResult Post(Employee employee)
        {
            try
            {
                //validate dữ liệu
                var res = _employeeService.InsertService(employee);
                return StatusCode(201, res);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = employee
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut]
        public IActionResult Put(Employee employee, Guid employeeId)
        {
            return Ok();
        }

        [HttpDelete("{employeeId}")]
        public IActionResult Delete(Guid employeeId)
        {
            return Ok();
        }
    }
}
