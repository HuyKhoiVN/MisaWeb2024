using CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Interfaces.Services
{
    public interface IEmployeeService : IBaseService<Employee>
    {
        public IEnumerable<Employee> GetPaging(int pageSize, int pageIndex);
    }
}
