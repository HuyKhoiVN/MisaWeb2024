using CukCuk.Core.Entities;
using CukCuk.Core.Exceptions;
using CukCuk.Core.Interfaces.Infrastructure;
using CukCuk.Core.Interfaces.Services;
using CukCuk.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Services
{
    public class PostionService : BaseService<Position>, IPositionService
    {
        IPositionRepository _positionRepository;

        public PostionService(IPositionRepository positionRepository):base(positionRepository)
        {
            _positionRepository = positionRepository;
        }    

        public int MultiUpdateService(List<Position> positions)
        {
            throw new NotImplementedException();
        }
    }
}
