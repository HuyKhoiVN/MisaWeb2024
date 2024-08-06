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

       /* public override int Insert(Employee employee)
        {
            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                //Bước 3: Thêm mới dữ liệu
                var sqlCommand = "INSERT INTO Employee (EmployeeId, EmployeeCode, FullName, DateOfBirth, Gender, Email, " +
                    "PhoneNumber, IdentityNumber, Address, IndentityDate, IdentityPlace, LanelineNumber, BankName, " +
                    "BankBranch, BankNumber, PositionId, DepartmentId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) " +

                    "VALUES(@EmployeeId, @EmployeeCode, @FullName, @DateOfBirth, @Gender, @Email, @PhoneNumber, " +
                    "@IdentityNumber, @Address, @IndentityDate, IdentityPlace, @LanelineNumber, @BankName, @BankBranch, " +
                    "@BankNumber, @PositionId, @DepartmentId, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy) ";

                //Tạo mới employee
                employee.EmployeeId = Guid.NewGuid();
                var res = _mySqlConnection.Execute(sql: sqlCommand, param: employee);
                //Bước 4: Trả thông tin về client
                return res;
            }         
        }*/

        public override int Update(Employee employee, Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public override int Delete(Guid employeeId)
        {
            return 1;
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
