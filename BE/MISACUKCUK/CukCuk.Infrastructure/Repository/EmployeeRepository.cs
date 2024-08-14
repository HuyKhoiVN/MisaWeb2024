using CukCuk.Core.Entities;
using CukCuk.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using static Dapper.SqlMapper;

namespace CukCuk.Infrastructure.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {  
        /// <summary>
        /// Lấy employee theo phân trang
        /// </summary>
        /// <param name="pageSize">số phần tử/trang</param>
        /// <param name="pageIndex">số trang</param>
        /// <returns></returns>
        public IEnumerable<Employee> GetPaging(int pageSize, int pageIndex)
        {
            // offset: số phần tử bỏ qua
            var offset = (pageIndex - 1) * pageSize;
            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                // 1. Câu lệnh sql
                var sqlCommand = "SELECT * FROM Employee ORDER BY EmployeeCode LIMIT @Offset, @PageSize;";

                // 2. Truyền tham số vào dynamic Param
                var dyamicParams = new DynamicParameters();
                dyamicParams.Add("@Offset", offset);
                dyamicParams.Add("@Pagesize", pageSize);

                // 3. Thực hiện truy vấn
                var employees = _mySqlConnection.Query<Employee>(sql: sqlCommand, param: dyamicParams);

                return employees;
            }
        }

        /// <summary>
        /// Kiểm tra EmployeeCode đã tồn tại hay chưa, sử dụng employeeId để check phương thức POST/PUT
        /// </summary>
        /// <param name="employeeCode"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Tính tổng số Employee dựa trên đếm số empId
        /// </summary>
        /// <returns></returns>
        public int GetTotalEmployeeCount()
        {
            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                // 1. Câu lệnh SQL để đếm tổng số EmployeeId
                var sqlCommand = "SELECT COUNT(EmployeeId) FROM Employee;";

                // 2. Thực hiện truy vấn và trả về kết quả
                var totalEmployees = _mySqlConnection.ExecuteScalar<int>(sql: sqlCommand);

                return totalEmployees;
            }
        }
    }
}
