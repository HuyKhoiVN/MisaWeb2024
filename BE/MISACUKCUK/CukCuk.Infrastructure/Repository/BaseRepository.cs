using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.MISAAttribute;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace CukCuk.Infrastructure.Repository
{
    public class BaseRepository<MisaEntity> : IBaseRepository<MisaEntity>
    {
        protected readonly string _connectionString = "Host = 8.222.228.150; Port = 3306; Database = HAUI_2021604561_TrinhHuyKhoi;User Id = manhnv; Password = 12345678";
        protected MySqlConnection _mySqlConnection;

        /// <summary>
        /// Xoá đối tượng
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual int Delete(Guid entityId)
        {
            var className = typeof(MisaEntity).Name;
            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                // Câu lệnh sql truy vấn
                var sqlCommand = $"DELETE FROM {className} WHERE {className}Id = @{className}Id";

                // Khởi tạo Parameters và truyền tham số
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add($"@{className}Id", entityId);

                // Thực hiện xoá và trả kết quả
                var res = _mySqlConnection.Execute(sql: sqlCommand, param:  dynamicParameters);
                return res;
            }
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <typeparam name="MisaEntity">type của Object</typeparam>
        /// <returns>Danh sách Object</returns>
        /// Created by: Khoi (1/8/24)
        public IEnumerable<MisaEntity> GetAllEntities()
        {
            var className = typeof(MisaEntity).Name;
            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                // 2. Lấy dữ liệu
                // 2.1 Câu lệnh truy vấn lấy dữ liệu
                var sqlCommand = $"Select * FROM {className}";
                // 2.2 Thực hiện lấy dữ liệu
                // lấy ra toàn bộ thông tin -> sử dụng object
                // lấy ra thông tin khớp với đối tượng MisaEntity -> sử dụng MisaEntity
                var entities = _mySqlConnection.Query<MisaEntity>(sql: sqlCommand);

                return entities;
            }
        }

        public MisaEntity GetById(Guid entityId)
        {
            var className = typeof(MisaEntity).Name;

            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                // 2. Lấy dữ liệu
                // 2.1 Câu lệnh truy vấn lấy dữ liệu
                var sqlCommand = $"Select * FROM {className} WHERE {className}Id = @{className}Id";

                // Lưu ý: Nếu có tham số truyền cho câu lệnh truy vấn sql thì phải sử dụng DyamicParameter
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add($"@{className}Id", entityId);

                // 2.2 Thực hiện lấy dữ liệu
                // lấy ra toàn bộ thông tin -> sử dụng object
                // lấy ra thông tin khớp với đối tượng Employee -> sử dụng Emp
                var entity = _mySqlConnection.QueryFirstOrDefault<MisaEntity>(sql: sqlCommand, param: dynamicParameters);

                return entity;
            }
        }

        /*public virtual int Insert(MisaEntity entity)
        {
            // Build chuỗi câu sql thực hiện thêm mới dữ liệu:

            var className = typeof(MisaEntity).Name;
            var sqlColumnsName = new StringBuilder();
            var sqlColumnValue = new StringBuilder();
            var dynamicParam = new DynamicParameters();

            // 1. Duyệt tất cả property của đối tượng
            var props = typeof(MisaEntity).GetProperties();
            var delimiter = "";

            foreach (var prop in props)
            {
                // 2. Lấy ra tên property               
                var propName = prop.Name;
                // 3. Lấy ra giá trị của property
                var propValue = prop.GetValue(entity);

                // Kiểm tra property phải khoá chính không
                var primaryKey = Attribute.IsDefined(prop, typeof(PrimaryKey));

                // Kiểm tra thuộc tính NotMap
                var notMap= Attribute.IsDefined(prop, typeof(NotMap));

                // Thực hiện tạo mới giá trị cho khoá chính
                if (primaryKey == true || propName == $"{className}Id")
                {
                    if(prop.PropertyType == typeof(Guid))
                    {
                        propValue = Guid.NewGuid();
                    }
                }

                // Bỏ qua nếu property được đánh dấu notMap
                if(notMap == true)
                    continue;

                // Set giá trị mặc định cho CreatedDate và ModifiedDate
                if(propName == "CreatedDate" || propName == "ModifiedDate")
                {
                    if(prop.PropertyType == typeof(DateTime?))
                    {
                        propValue = DateTime.Now;
                    }
                }

                // Thêm @ vào trước parameter
                var paramName = $"@{propName}";

                sqlColumnsName.Append($"{delimiter}{propName}");
                sqlColumnValue.Append($"{delimiter}{paramName}");
                delimiter = ",";

                dynamicParam.Add(paramName, propValue);
            }
            
            // 4. Thực hiện build câu lệnh sql
            var sqlCommand = $"INSERT {className}({sqlColumnsName.ToString()}) VALUES ({sqlColumnValue.ToString()})";

            using(_mySqlConnection = new MySqlConnection(_connectionString))
            {
                var res = _mySqlConnection.Execute(sql: sqlCommand, param: dynamicParam);
                return res;
            }
        }*/

        public virtual int Update(MisaEntity entity, Guid entityId)
        {
            // Khai báo thông tin cần thiết
            var className = typeof(MisaEntity).Name;
            var sqlParam = new StringBuilder();
            var dynamicParam = new DynamicParameters();

            // Duyệt tất cả property của đối tượng
            var props = typeof(MisaEntity).GetProperties();
            var delimiter = "";

            foreach ( var prop in props )
            {
                // Lấy ra tên và giá trị của đối tượng
                var propName = prop.Name;
                var propValue = prop.GetValue(entity);
                var paramName = $"@{propName}";

                // Kiểm tra property phải khoá chính không
                var primaryKey = Attribute.IsDefined(prop, typeof(PrimaryKey));

                // Kiểm tra thuộc tính NotMap
                var notMap = Attribute.IsDefined(prop, typeof(NotMap));

                // Bỏ qua nếu property là NotMap hoặc primaryKey
                if (notMap == true || primaryKey == true)
                    continue;

                // Append property = @property vào sqlParam
                sqlParam.Append($"{delimiter}{propName} = {paramName}");

                // Set delimiter = "," từ property thứ 2
                delimiter = ",";

                // Add giá trị cho parameter
                dynamicParam.Add(paramName, propValue);
            }

            // Câu lệnh sql
            var sqlCommand = $"UPDATE {className} SET {sqlParam} WHERE {className}Id = @{className}Id";
            dynamicParam.Add($"@{className}Id", entityId);

            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                var res = _mySqlConnection.Execute(sql: sqlCommand, param: dynamicParam);
                return res;
            }
        }


        public int Insert(MisaEntity entity)
        {
            using (_mySqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommandText = "";
                _mySqlConnection.Open();

                // Tạo sqlCommand
                var sqlCommand = _mySqlConnection.CreateCommand();
                sqlCommand.CommandText = sqlCommandText;
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlCommandBuilder.DeriveParameters(sqlCommand);

                // Gán tham số cho parameter
                var dynamicParam = ParamProcess(sqlCommand, entity);

                // Thực hiện StoredProcedure
                var res = _mySqlConnection.Execute(sql: sqlCommandText, param: dynamicParam, commandType: System.Data.CommandType.StoredProcedure);
                return res;
            }

        }

        /// <summary>
        /// Xử lý các tham số đầu vào cho Parameter - Produce
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        private DynamicParameters ParamProcess(MySqlCommand sqlCommand, MisaEntity entity)
        {
            var dynamicParam = new DynamicParameters();
            foreach (MySqlParameter parameter in sqlCommand.Parameters)
            {
                // tên của tham số:
                var paramName = parameter.ParameterName;
                var propName = paramName.Replace("@m_", "");

                // Kiểm tra thuộc tính của entity
                var entityProperty = entity.GetType().GetProperty(propName);

                // Nếu thuộc tính tồn tại -> gán giá trị, ngược lại gán null
                if (entityProperty != null)
                {
                    var propValue = entityProperty.GetValue(entity);
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
    }
}
