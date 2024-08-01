using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using MISACUKCUK.Api.Model;
using System.Linq.Expressions;
using MISACUKCUK.Api.Resources;
using static Dapper.SqlMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

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
            return GetDelEmployees("Get");
        }

        //nhận pageSize, pageNumber, employeeFilter, departmentId, positionId
        [HttpGet("getpage")]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            return GetDelEmployees("GetPage", pageNumber, pageSize);
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
            return SaveEmployee(employee, "Proc_InsertEmployee");
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

            return SaveEmployee(employee, "Proc_UpdateEmployee", employeeId);
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
            return GetDelEmployees("Delete", employeeId: employeeId);
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

            //1.6 kiểm tra tính hợp lệ của PositionId và DepartmentId

            // kiểm tra sự tồn tại DepartmentId
            if (employee.DepartmentId.HasValue && !CheckDepartmentId(employee.DepartmentId.Value))
            {
                errorData.Add("DepartmentId", "Department ID is invalid");
            }

            //kiểm tra sự tồn tại PositionId
            if (employee.PositionId.HasValue && !CheckPositionId(employee.PositionId.Value))
            {
                errorData.Add("PositionId", "PositionId is invalid");
            }

            return errorData;
        }

        /// <summary>
        /// Lấy danh sách nhân viên (toàn bộ hoặc lấy theo phân trang)
        /// </summary>
        /// <param name="action">tên hành động</param>
        /// <param name="pageNumber">số trang</param>
        /// <param name="pageSize">số bản ghi/trang</param>
        /// <returns></returns>
        private IActionResult GetDelEmployees(string action, int pageNumber = 1, int pageSize = 10, Guid? employeeId = null)
        {
            try
            {
                // Validate tham số đầu vào
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Page number and page size must be greater than 0.");
                }

                // Khai báo thông tin Database
                var connectionString = "Host=8.222.228.150;Port=3306;Database=HAUI_2021604561_TrinhHuyKhoi;User Id=manhnv;Password=12345678";

                // Stored procedure name
                var sqlCommandText = "Proc_GetPage_DelEmployee";

                // Khởi tạo kết nối với MariaDB
                using (var sqlConnection = new MySqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    // Tạo sqlCommand
                    var sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = sqlCommandText;
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    // Gán tham số cho parameter, bao gồm m_Action, m_Offset, m_PageSize, m_EmployeeId
                    var parameters = new DynamicParameters();
                    parameters.Add("@m_Action", action);

                    parameters.Add("@m_PageSize", pageSize);

                    //m_EmployeeId = EmployeeId, nếu không có giá trị -> null
                    parameters.Add("@m_EmployeeId", employeeId.HasValue ? employeeId : null);

                    // Gán tham số riêng với từng Action
                    if (action == "GetPage")
                    {
                        // Tính số bản ghi cần bỏ qua
                        var offset = (pageNumber - 1) * pageSize;

                        // Gán tham số cho offset
                        parameters.Add("@m_Offset", offset);
                    }
                    else if (action == "Get")
                    {
                        // Set m_Offset = 0
                        parameters.Add("@m_Offset", 0);
                    }
                    else if (action == "Delete")
                    {
                        if(!employeeId.HasValue)
                        {
                            return BadRequest("EmployeeId Not Found");
                        }
                        parameters.Add("@m_EmployeeId", employeeId);
                        parameters.Add("@m_Ofset", 0);
                        return Ok(ResourceVN.Info_EmployeeDeleted);
                    }
                    else
                    {
                        return BadRequest("Action not valid");
                    }

                    // Thực thi stored procedure
                    var employees = sqlConnection.Query<Employee>(sql: sqlCommandText, param: parameters, commandType: CommandType.StoredProcedure);

                    // Trả về json gồm tổng số nhân viên và danh sách nhân viên
                    return Ok(new
                    {
                        Employees = employees
                    });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Hàm thêm/sửa nhân viên, employee: đối tượng nv, sqlCommandText: StoreProduce, employeeId
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="sqlCommandText"></param>
        /// <param name="employeeId"></param>
        /// <returns>
        /// 400: Bad request
        /// 200: Sửa thành công, 201: Thêm thành công
        /// </returns>
        private IActionResult SaveEmployee(Employee employee, string sqlCommandText, Guid? employeeId = null)
        {
            // Khai báo các thông tin cần thiết
            var connectionString = "Host=8.222.228.150;Port=3306;Database=HAUI_2021604561_TrinhHuyKhoi;User Id=manhnv;Password=12345678";
            var error = new ErrorService();

            //Kiểm tra id của employee, nếu không -> tạo mới, có -> bằng giá trị cũ
            if (!employeeId.HasValue)
            {
                employee.EmployeeId = Guid.NewGuid();
            }
            else
            {
                employee.EmployeeId = employeeId.Value;
            }

            //validate Emp
            var errorData = ValidateEmployee(employee, employeeId);

            //Nếu có lỗi -> BadRequest, nếu không bỏ qua
            if (errorData.Count > 0)
            {
                error.UserMsg = ResourceVN.Error_InputValueNotValid;
                error.Data = errorData;
                return BadRequest(error);
            }

            // Khởi tạo kết nối đến db
            using (var sqlConnection = new MySqlConnection(connectionString))
            {
                sqlConnection.Open();

                // Tạo sqlCommand
                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = sqlCommandText;
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlCommandBuilder.DeriveParameters(sqlCommand);

                // Gán tham số cho parameter
                var dynamicParam = ParamProcess(sqlCommand, employee);

                // Thực hiện StoredProcedure
                var res = sqlConnection.Execute(sql: sqlCommandText, param: dynamicParam, commandType: System.Data.CommandType.StoredProcedure);

                // Trả thông tin về client
                if (res > 0)
                {
                    return employeeId.HasValue ? Ok(res) : StatusCode(201, res);
                }
                else
                {
                    error.UserMsg = employeeId.HasValue ? "Update failed." : "Insert failed.";
                    return BadRequest(error);
                }
            }
        }

        /// <summary>
        /// Xử lý, gán parameter
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="employee"></param>
        /// <returns>
        /// DyamicParameters đã được gán giá trị cho từng DymicParameter
        /// </returns>
        private DynamicParameters ParamProcess(MySqlCommand sqlCommand, Employee employee)
        {
            var dynamicParam = new DynamicParameters();
            foreach (MySqlParameter parameter in sqlCommand.Parameters)
            {
                // tên của tham số:
                var paramName = parameter.ParameterName;
                var propName = paramName.Replace("@m_", "");

                // Kiểm tra thuộc tính của employee
                var entityProperty = employee.GetType().GetProperty(propName);

                // Nếu thuộc tính tồn tại -> gán giá trị, ngược lại gán null
                if (entityProperty != null)
                {
                    var propValue = entityProperty.GetValue(employee);
                    // Thực hiện gán giá trị cho các Value
                    dynamicParam.Add(paramName, propValue);
                }
                else
                {
                    dynamicParam.Add(paramName, null);
                }
            }
            return dynamicParam;
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
