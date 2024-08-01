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
    public class EmployeeRepository : IEmployeeRepository
    {
        public bool CheckDuplicateCode(string employeeCode, Guid? employeeId = null)
        {
            var connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
            var sqlConnection = new MySqlConnection(connectionString);

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

        public int Delete(Employee employee)
        {
            return 1;
        }

        public IEnumerable<Employee> GetAll()
        {
            //Khai báo thông tin Database
            var connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
            //1. Khởi tạo kết nối với MariaDb:            
            var sqlConnection = new MySqlConnection(connectionString);

            //2. Lấy dữ liệu
            //2.1 Câu lệnh truy vấn lấy dữ liệu
            var sqlCommand = "Select * FROM Employee";
            //2.2 Thực hiện lấy dữ liệu
            //lấy ra toàn bộ thông tin -> sử dụng object
            //lấy ra thông tin khớp với đối tượng Employee -> sử dụng Emp
            var employees = sqlConnection.Query<Employee>(sql: sqlCommand);

            return employees;
        }

        public Employee GetById(Guid employeeId)
        {
            //Khai báo thông tin Database
            var connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
            //1. Khởi tạo kết nối với MariaDb:            
            var sqlConnection = new MySqlConnection(connectionString);

            //2. Lấy dữ liệu
            //2.1 Câu lệnh truy vấn lấy dữ liệu
            var sqlCommand = $"Select * FROM Employee WHERE EmployeeId = @EmployeeId";

            //Lưu ý: Nếu có tham số truyền cho câu lệnh truy vấn sql thì phải sử dụng DyamicParameter
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@EmployeeId", employeeId);


            //2.2 Thực hiện lấy dữ liệu
            //lấy ra toàn bộ thông tin -> sử dụng object
            //lấy ra thông tin khớp với đối tượng Employee -> sử dụng Emp
            var employee = sqlConnection.QueryFirstOrDefault<Employee>(sql: sqlCommand, param: dynamicParameters);

            return employee;
        }

        public IEnumerable<Employee> GetPaging(int pageSize, int pageIndex)
        {
            throw new NotImplementedException();
        }

        public int Insert(Employee employee)
        {
            //Khai báo các thông tin cần thiết
            var connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
            //Bước 2: Khởi tạo kết nối đến DB
            var sqlConnection = new MySqlConnection(connectionString);

            //Bước 3: Thêm mới dữ liệu
            var sqlCommand = "INSERT INTO Employee (EmployeeId, EmployeeCode, FullName, DateOfBirth, Gender, Email, " +
                "PhoneNumber, IdentityNumber, Address, IndentityDate, IdentityPlace, LanelineNumber, BankName, " +
                "BankBranch, BankNumber, PositionId, DepartmentId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) " +

                "VALUES(@EmployeeId, @EmployeeCode, @FullName, @DateOfBirth, @Gender, @Email, @PhoneNumber, " +
                "@IdentityNumber, @Address, @IndentityDate, IdentityPlace, @LanelineNumber, @BankName, @BankBranch, " +
                "@BankNumber, @PositionId, @DepartmentId, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy) ";

            //Tạo mới employee
            employee.EmployeeId = Guid.NewGuid();
            var res = sqlConnection.Execute(sql: sqlCommand, param: employee);
            //Bước 4: Trả thông tin về client
            return res;
        }

        public int Update(Employee employee, Guid employeeId)
        {
            throw new NotImplementedException();
        }
    }
}
