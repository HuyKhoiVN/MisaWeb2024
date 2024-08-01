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
    public class EmployeeService : IEmployeeService
    {
        IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public int InsertService(Employee employee)
        {
            // 1.1 Thông tin mã nhân viên không được phép để trống
            if (string.IsNullOrEmpty(employee.EmployeeCode))
            {
                throw new EmployeeValidateException(ResourceVN.ValidateError_EmployeeCodeNotEmpty);
            }

            //1.2 Check trùng mã
            var isDuplicate = _employeeRepository.CheckDuplicateCode(employee.EmployeeCode);
            if (isDuplicate)
            {
                throw new EmployeeValidateException(ResourceVN.ValidateError_EmployeeCodeExits);
            }

            //Thực hiện thêm mới
            var res = _employeeRepository.Insert(employee);
            return res;
        }

        public int UpdateService(Employee employee, Guid employeeId)
        {
            return 0;
        }
    }
}
