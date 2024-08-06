using CukCuk.Core.Entities;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CukCuk.Api.Controllers
{   
    public class DepartmentController : MISABaseController<Department>
    {
        public DepartmentController(IBaseService<Department> baseService, IBaseRepository<Department> baseRepository) : base(baseService, baseRepository)
        {
            
        }
    }
}
