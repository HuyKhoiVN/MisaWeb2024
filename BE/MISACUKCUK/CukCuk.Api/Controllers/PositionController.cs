using CukCuk.Core.Entities;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CukCuk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : MISABaseController<Position>
    {
        public PositionController(IBaseService<Position> baseService, IBaseRepository<Position> baseRepository) : base(baseService, baseRepository)
        {
            
        }
    }
}
