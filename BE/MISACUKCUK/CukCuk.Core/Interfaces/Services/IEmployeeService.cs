using CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Interfaces.Services
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Thêm mới dữ liệu
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        /// CreatedBy: Khoi 29/7
        int InsertService(Employee employee);

        /// <summary>
        /// Sửa nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        /// CreatedBy: Khoi 29/7
        int UpdateService(Employee employee, Guid employeeId);
    }
}
