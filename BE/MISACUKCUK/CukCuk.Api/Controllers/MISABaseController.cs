using CukCuk.Core.Exceptions;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using CukCuk.Core.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CukCuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MISABaseController<MISAEntity> : ControllerBase
    {
        #region Fields
        IBaseService<MISAEntity> _baseService;
        IBaseRepository<MISAEntity> _baseRepository;
        #endregion

        #region Contructor
        public MISABaseController(IBaseService<MISAEntity> baseService, IBaseRepository<MISAEntity> baseRepository)
        {
            _baseService = baseService;
            _baseRepository = baseRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <returns>
        /// 200 - có dữ liệu
        /// 400 - có lỗi nghiệp vụ
        /// 500 - có exception
        /// </returns>
        /// Creatd by: Khoi (01/08/24)
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var data = _baseRepository.GetAllEntities();
                return Ok(data);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = ex.Data
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ResourceVN.Erorr_Exception,
                    data = ex.InnerException
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Lấy entity theo id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpGet("{entityId}")]
        public IActionResult GetById(Guid entityId)
        {
            try
            {
                var data = _baseRepository.GetById(entityId);
                return Ok(data);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = ex.Data
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ResourceVN.Erorr_Exception,
                    data = ex.InnerException
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Thêm entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(MISAEntity entity)
        {
            try
            {
                var data = _baseService.InsertService(entity);
                return StatusCode(201, data);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = entity
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ResourceVN.Erorr_Exception,
                    data = ex.InnerException
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put(MISAEntity entity, Guid entityId)
        {
            try
            {
                var data = _baseService.UpdateService(entity, entityId);
                return Ok(data);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = ex.Data
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ResourceVN.Erorr_Exception,
                    data = ex.InnerException
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpDelete("{entityId}")]
        public IActionResult Delete(Guid entityId)
        {
            try
            {
                var data = _baseRepository.Delete(entityId);
                return Ok(data);
            }
            catch (EmployeeValidateException ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Message,
                    data = ex.Data
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    devMsg = ex.Message,
                    userMsg = ResourceVN.Erorr_Exception,
                    data = ex.InnerException
                };
                return StatusCode(500, response);
            }
        }

        #endregion
    }
}
