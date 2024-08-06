using CukCuk.Core.Entities;
using CukCuk.Core.Exceptions;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using CukCuk.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        protected override void ValidateEmployee(Employee employee)
        {
            //1.2 Check trùng mã
            var isDuplicate = _employeeRepository.CheckDuplicateCode(employee.EmployeeCode);
            if (isDuplicate)
            {
                throw new EmployeeValidateException(ResourceVN.ValidateError_EmployeeCodeExits);
            }

            //1.3 Ngày sinh không lớn hơn ngày hiện tại
            if (employee.DateOfBirth > DateTime.Now)
            {
                throw new EmployeeValidateException("Ngày sinh không lớn hơn ngày hiện tại");
            }
        }
    }
}
