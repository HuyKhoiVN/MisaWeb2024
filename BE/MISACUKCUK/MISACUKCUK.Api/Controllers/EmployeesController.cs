using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using MISACUKCUK.Api.Model;
using System.Linq.Expressions;
using MISACUKCUK.Api.Resources;
using static Dapper.SqlMapper;

namespace MISACUKCUK.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        ///  lấy ds nhân viên
        /// </summary>
        /// <returns>
        ///   200 - DS nhân viên
        ///   204 - nếu không có dữ liệu
        /// </returns>
        /// CreatedBy: TH Khoi (20/07/2024)
        [HttpGet]
        public IActionResult Get()
        {
            try
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
                var employees = sqlConnection.Query<object>(sql: sqlCommand);

                //trả kết quả cho client
                return Ok(employees);
            }catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Lấy nhân viên theo Id
        /// </summary>
        /// <param name="employeeId">Id của nhân viên</param>
        /// <returns></returns>
        /// CreatedBy: TH Khoi (20/07/2024)
        [HttpGet("{employeeId}")]
        public IActionResult GetById(Guid employeeId)
        {
            try
            {
                //gọi hàm GetById
                var employee = GetEmployeeById(employeeId);

                //trả kết quả cho client
                //nếu tồn tại -> trả về emp, nếu không not found
                if (employee == null)
                {
                    return NotFound(ResourceVN.Info_EmployeeNotFound);
                }
                return Ok(employee);
            }catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Thêm mới nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>
        /// 201 - Thêm thành công
        /// 400 - Dữ liệu đầu vào không hợp lệ
        /// 500 - Có exception
        /// </returns>
        [HttpPost]
        public IActionResult Post([FromBody] Employee employee)
        {
            //Khai báo các thông tin cần thiết
            var connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
            var error = new ErrorService();

            //gọi hàm validate employee
            var errorData = ValidateEmployee(employee);

            //kiểm tra tính hợp lệ của PositionId và DepartmentId

            // kiểm tra sự tồn tại DepartmentId
            if (employee.DepartmentId.HasValue && !CheckDepartmentId(employee.DepartmentId.Value))
            {
                error.UserMsg = ResourceVN.Error_DepartmentIDNotValid;
                return BadRequest(error);
            }

            //kiểm tra sự tồn tại PositionId
            if (employee.PositionId.HasValue && !CheckPositionId(employee.PositionId.Value))
            {
                error.UserMsg = ResourceVN.Error_PositionIDNotValid;
                return BadRequest(error);
            }

            if (errorData.Count > 0)
            {
                error.UserMsg = ResourceVN.Error_InputValueNotValid;
                error.Data = errorData;
                return BadRequest(error);
            }

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
            var res = sqlConnection.Execute(sql:  sqlCommand, param: employee);
            //Bước 4: Trả thông tin về client
            if(res > 0)
            {
                return StatusCode(201, res);
            }else
            {
                return Ok(res);
            }
        }

        /// <summary>
        /// Sửa nhân viên
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="employee"></param>
        /// <returns>
        /// 200 - Sửa thành công
        /// 400 - Dữ liệu đầu vào không hợp lệ
        /// 404 - Không có nhân viên
        /// 500 - Có exception
        /// </returns>
        [HttpPut("{employeeId}")]
        public IActionResult Put(Guid employeeId, [FromBody] Employee employee)
        {
            // Kiểm tra sự tồn tại của nhân viên trước khi cập nhật
            var existingEmployee = GetEmployeeById(employeeId);
            if (existingEmployee == null)
            {
                return NotFound("Employee not found.");
            }

            //khai báo các thông tin cần thiết
            var connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
            var error = new ErrorService();

            //gọi hàm validate employee
            var errorData = ValidateEmployee(employee, employeeId);

            if (errorData.Count > 0)
            {
                error.UserMsg = ResourceVN.Error_InputValueNotValid;
                error.Data = errorData;
                return BadRequest(error);
            }

            // kiểm tra sự tồn tại DepartmentId
            if (employee.DepartmentId.HasValue && !CheckDepartmentId(employee.DepartmentId.Value))
            {
                error.UserMsg = "Department ID is invalid.";
                return BadRequest(error);
            }

            //kiểm tra sự tồn tại PositionId
            if (employee.PositionId.HasValue && !CheckPositionId(employee.PositionId.Value))
            {
                error.UserMsg = "Position ID is invalid.";
                return BadRequest(error);
            }


            //Khởi tạo kết nối đến db
            using (var sqlConnection = new MySqlConnection(connectionString))
            {
                sqlConnection.Open();

                // Bước 3: Cập nhật dữ liệu
                var sqlCommand = "UPDATE Employee SET EmployeeCode = @EmployeeCode, FullName = @FullName, DateOfBirth = @DateOfBirth, " +
                    "Gender = @Gender, Email = @Email, PhoneNumber = @PhoneNumber, IdentityNumber = @IdentityNumber, Address = @Address, " +
                    "IndentityDate = @IndentityDate, IdentityPlace = @IdentityPlace, LanelineNumber = @LanelineNumber, BankName = @BankName, " +
                    "BankBranch = @BankBranch, BankNumber = @BankNumber, PositionId = @PositionId, DepartmentId = @DepartmentId, " +
                    "ModifiedDate = @ModifiedDate, ModifiedBy = @ModifiedBy " +
                    "WHERE EmployeeId = @EmployeeId";

                // Thiết lập các tham số
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@EmployeeId", employeeId);
                dynamicParameters.Add("@EmployeeCode", employee.EmployeeCode);
                dynamicParameters.Add("@FullName", employee.FullName);
                dynamicParameters.Add("@DateOfBirth", employee.DateOfBirth);
                dynamicParameters.Add("@Gender", employee.Gender);
                dynamicParameters.Add("@Email", employee.Email);
                dynamicParameters.Add("@PhoneNumber", employee.PhoneNumber);
                dynamicParameters.Add("@IdentityNumber", employee.IdentityNumber);
                dynamicParameters.Add("@Address", employee.Address);
                dynamicParameters.Add("@IndentityDate", employee.IndentityDate);
                dynamicParameters.Add("@IdentityPlace", employee.IdentityPlace);
                dynamicParameters.Add("@LanelineNumber", employee.LanelineNumber);
                dynamicParameters.Add("@BankName", employee.BankName);
                dynamicParameters.Add("@BankBranch", employee.BankBranch);
                dynamicParameters.Add("@BankNumber", employee.BankNumber);
                dynamicParameters.Add("@PositionId", employee.PositionId);
                dynamicParameters.Add("@DepartmentId", employee.DepartmentId);
                dynamicParameters.Add("@ModifiedDate", DateTime.Now);
                dynamicParameters.Add("@ModifiedBy", employee.ModifiedBy); // Cập nhật người dùng thực hiện thay đổi

                // Thực hiện cập nhật
                var res = sqlConnection.Execute(sql: sqlCommand, param: dynamicParameters);

                // Bước 4: Trả thông tin về client
                if (res > 0)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(error);
                }
            }
        }

        /// <summary>
        /// Xoá nhân viên khỏi db
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>
        /// 200 - xoá thành công
        /// 400 - bad request
        /// </returns>
        [HttpDelete("{employeeId}")]
        public IActionResult Delete(Guid employeeId)
        {
            // Kiểm tra sự tồn tại của nhân viên trước khi xóa
            var existingEmployee = GetEmployeeById(employeeId);
            if (existingEmployee == null)
            {
                return NotFound(ResourceVN.Info_EmployeeNotFound);
            }

            // Khai báo thông tin cần thiết
            var connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
            var error = new ErrorService();

            // Khởi tạo kết nối đến db
            using (var sqlConnection = new MySqlConnection(connectionString))
            {
                sqlConnection.Open();

                // Bước 3: Xóa dữ liệu
                var sqlCommand = "DELETE FROM Employee WHERE EmployeeId = @EmployeeId";

                // Thiết lập tham số
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@EmployeeId", employeeId);

                try
                {
                    // Thực hiện xóa
                    var res = sqlConnection.Execute(sql: sqlCommand, param: dynamicParameters);

                    // Bước 4: Trả thông tin về client
                    if (res > 0)
                    {
                        return Ok(ResourceVN.Info_EmployeeDeleted);
                    }
                    else
                    {
                        return BadRequest(ResourceVN.Erorr_Exception);
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi và trả về lỗi server
                    return HandleException(ex);
                }
            }
        }


        /// <summary>
        /// Validate dữ liệu đầu vào
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>
        /// errorData > 0 - nếu không hợp lệ
        /// errorData = 0 - nếu hợp lệ
        /// </returns>
        private Dictionary<string, string> ValidateEmployee(Employee employee, Guid? employeeId = null)
        {
            var errorData = new Dictionary<string, string>();

            // 1.1 Thông tin mã nhân viên không được phép để trống
            if (string.IsNullOrEmpty(employee.EmployeeCode))
            {
                errorData.Add("EmployeeCode", ResourceVN.ValidateError_EmployeeCodeNotEmpty);
            }

            // 1.2 Thông tin họ tên không được trống
            if (string.IsNullOrEmpty(employee.FullName))
            {
                errorData.Add("FullName", ResourceVN.ValidateError_FullNameNotEmpty);
            }

            // 1.3 Email đúng định dạng
            if (!IsValidEmail(email: employee.Email))
            {
                errorData.Add("Email", ResourceVN.ValidateError_EmailNotValid);
            }

            // 1.4 Ngày sinh không lớn hơn ngày hiện tại
            if (employee.DateOfBirth > DateTime.Now)
            {
                errorData.Add("DateOfBirth", ResourceVN.ValidateError_DateOfBirthNotValid);
            }

            // 1.5 Check trùng mã nhân viên
            if (CheckEmployeeCode(employee.EmployeeCode, employeeId))
            {
                errorData.Add("EmployeeCode", ResourceVN.ValidateError_EmployeeCodeExits);
            }

            return errorData;
        }

        /// <summary>
        /// Kiểm tra mã nv có trùng không
        /// Lưu ý: Nếu sửa phải loại trừ chính mình ra
        /// </summary>
        /// <param name="employeeCode"></param>
        /// <returns>true - đã trùng; false - không trùng</returns>
        /// CreatedBy: TH Khoi (20/07/2024)
        private bool CheckEmployeeCode(string employeeCode, Guid? employeeId = null)
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

        /// <summary>
        /// Kiểm tra sự tồn tại của departmentId
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns>
        /// res > 0 nếu tồn tại
        /// res = 0 nếu không tồn tại
        /// </returns>
        private bool CheckDepartmentId(Guid departmentId)
        {
            var connectionString = "Host=8.222.228.150;Port=3306;Database=HAUI_2021604561_TrinhHuyKhoi;User Id=manhnv;Password=12345678";
            try
            {
                using (var sqlConnection = new MySqlConnection(connectionString))
                {
                    // Mở kết nối đến cơ sở dữ liệu
                    sqlConnection.Open();

                    // Khai báo câu lệnh
                    var sqlCheck = "SELECT COUNT(1) FROM Department WHERE DepartmentId = @DepartmentId";

                    // Khởi tạo DynamicParameters và thêm tham số
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@DepartmentId", departmentId);

                    // Thực hiện k tra và trả kết quả về client
                    var res = sqlConnection.ExecuteScalar<int>(sqlCheck, dynamicParameters);

                    // Trả về true nếu DepartmentId tồn tại, ngược lại trả về false
                    return res > 0;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ 
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của Position
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns>
        /// res > 0 nếu tồn tại
        /// res = 0 nếu không tồn tại
        /// </returns>
        private bool CheckPositionId(Guid positionId)
        {
            var connectionString = "Host=8.222.228.150;Port=3306;Database=HAUI_2021604561_TrinhHuyKhoi;User Id=manhnv;Password=12345678";
            try
            {
                using (var sqlConnection = new MySqlConnection(connectionString))
                {
                    // Mở kết nối đến cơ sở dữ liệu
                    sqlConnection.Open();

                    // Khai báo câu lệnh
                    var sqlCheck = "SELECT COUNT(1) FROM Position WHERE PositionId = @PositionId";

                    // Khởi tạo DynamicParameters và thêm tham số
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@PositionId", positionId);

                    // Thực hiện kiểm tra, trả kết quả về cho client
                    var res = sqlConnection.ExecuteScalar<int>(sqlCheck, dynamicParameters);

                    // Trả về true nếu PositionId tồn tại, ngược lại trả về false
                    return res > 0;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return false;
            }
        }

        //hàm getById tìm nhân viên theo mã hoặc kiểm tra sự tồn tại của nhân viên
        private Employee GetEmployeeById(Guid employeeId)
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

        //Bắt exption
        private IActionResult HandleException(Exception ex)
        {
            var error = new ErrorService();
            error.DevMsg = ex.Message;
            error.UserMsg = ResourceVN.Erorr_Exception;
            error.Data = ex.Data;
            return StatusCode(500, error);
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
