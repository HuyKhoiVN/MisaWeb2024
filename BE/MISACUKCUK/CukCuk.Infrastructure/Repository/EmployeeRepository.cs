using CukCuk.Core.Entities;
using CukCuk.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;

namespace CukCuk.Infrastructure.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {  
        public IEnumerable<Employee> GetPaging(int pageSize, int pageIndex)
        {
            throw new NotImplementedException();
        }
        public bool CheckDuplicateCode(string employeeCode, Guid? employeeId = null)
        {
            var sqlConnection = new MySqlConnection(_connectionString);

            var sqlCheck = "SELECT EmployeeCode FROM Employee WHERE EmployeeCode = @EmployeeCode";

            // nếu có ID, thêm điều kiện để loại trừ nhân viên hiện tại
            if (employeeId.HasValue)
            {
                sqlCheck += " AND EmployeeId != @EmployeeId";
            }

            //khởi tạo dyamicParameters và thêm tham số
            var dyamicParams = new DynamicParameters();
            dyamicParams.Add("@EmployeeCode", employeeCode);
            if (employeeId.HasValue)
            {
                dyamicParams.Add("@EmployeeId", employeeId.Value);
            }

            var res = sqlConnection.Query<string>(sqlCheck, param: dyamicParams);

            //có phần tử --> trả về true, nếu không có phần tử -> trả về false
            return res.Any();
        }
    }
}
