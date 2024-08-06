using CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Interfaces.Infrastructure
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        int Delete(Guid employeeId);
        IEnumerable<Employee> GetPaging(int pageSize, int pageIndex);
        bool CheckDuplicateCode(string employeeCode, Guid? employeeId = null);
    }
}
