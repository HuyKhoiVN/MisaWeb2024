using CukCuk.Core.Entities;
using CukCuk.Core.Exceptions;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using CukCuk.Core.MISAResources;
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

        protected override void ValidateEmployee(Employee employee, Guid? employeeId = null)
        {
            //1.2 Check trùng mã
            var isDuplicate = _employeeRepository.CheckDuplicateCode(employee.EmployeeCode, employeeId);
            if (isDuplicate)
            {
                throw new EmployeeValidateException(ResourceVN.ValidateError_EmployeeCodeExits);
            }

            //1.3 Ngày sinh không lớn hơn ngày hiện tại
            if (employee.DateOfBirth > DateTime.Now)
            {
                throw new EmployeeValidateException(ResourceVN.ValidateError_DateOfBirthNotValid);
            }

            // 1.4 Email đúng định dạng
            if (!IsValidEmail(employee.Email))
                throw new EmployeeValidateException(ResourceVN.ValidateError_EmailNotValid);
        }

        //Kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
