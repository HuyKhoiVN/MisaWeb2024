using CukCuk.Core.Exceptions;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using CukCuk.Core.MISAAttribute;
using CukCuk.Core.MISAResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Services
{
    public class BaseService<MisaEntity> : IBaseService<MisaEntity>
    {
        IBaseRepository<MisaEntity> _baseRepository;

        public BaseService(IBaseRepository<MisaEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public int InsertService(MisaEntity entity)
        {
            // Xử lý validate
            // Validate chung
            ValidateData(entity);
            // Validate riêng
            ValidateEmployee(entity);
            // Thêm mới
            var res = _baseRepository.Insert(entity);
            return res;
        }

        public int UpdateService(MisaEntity entity, Guid entityId)
        {
            // Xử lý validate
            // Validate chung
            ValidateData(entity);
            // Validate riêng
            ValidateEmployee(entity, entityId);

            // Update
            var res = _baseRepository.Update(entity, entityId);
            return res;
        }

        private void ValidateData(MisaEntity entity)
        {
            var props = entity.GetType().GetProperties();
            // Check dữ liệu không được phép để trống
            // Lấy ra attribute được đánh dấu [NotEmpty]
            var propNotEmpty = props.Where(p => Attribute.IsDefined(p, typeof(NotEmpty)));

            foreach (var prop in propNotEmpty)
            {
                var propValue = prop.GetValue(entity);
                var propName = prop.Name;
                var nameDisplay = string.Empty;

                // Lấy ra tên của property
                var propertyNames = prop.GetCustomAttributes(typeof(PropertyName), true);
                if(propertyNames.Length > 0)
                {
                    nameDisplay = (propertyNames[0] as PropertyName).Name;
                }

                if(propValue == null || string.IsNullOrEmpty(propValue.ToString()))
                {
                    nameDisplay = (nameDisplay == string.Empty ? propName : nameDisplay);
                    throw new EmployeeValidateException($"{nameDisplay} " + ResourceVN.ValidateError_NotNull); 
                }
            }          
        }

        protected virtual void ValidateEmployee(MisaEntity entity, Guid? entityId = null)
        {

        }
    }
}
