using CukCuk.Core.Entities;
using CukCuk.Core.Exceptions;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using CukCuk.Core.MISAResources;
using CukCuk.Core.Services;
using CukCuk.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CukCuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeController : MISABaseController<Employee>
    {
        IEmployeeService _employeeService;
        IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository, IEmployeeService employeeService) : base(employeeService, employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Lấy theo phân trang
        /// </summary>
        /// <param name="pageSize">Số trang</param>
        /// <param name="pageIndex">Số bản ghi/trang</param>
        /// <returns></returns>
        [HttpGet("getpaging")]
        public IActionResult GetPaging(int pageSize, int pageIndex)
        {
            try
            {
                var data = _employeeService.GetPaging(pageSize, pageIndex);
                return Ok(data);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = ex.Data
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ResourceVN.Erorr_Exception,
                    data = ex.InnerException
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Tổng số nhân viên
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalEmployeeCount")]
        public IActionResult GetTotalEmployeeCount()
        {
            try
            {
                var data = _employeeRepository.GetTotalEmployeeCount();
                return Ok(data);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = ex.Data
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ResourceVN.Erorr_Exception,
                    data = ex.InnerException
                };
                return StatusCode(500, response);
            }
        }
    }
}
