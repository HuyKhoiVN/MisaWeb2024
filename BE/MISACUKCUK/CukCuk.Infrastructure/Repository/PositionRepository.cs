using CukCuk.Core.Entities;
using CukCuk.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Infrastructure.Repository
{
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public override int Delete(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public override int Insert(Position position)
        {
            throw new NotImplementedException();
        }

        public override int Update(Position position, Guid positionId)
        {
            throw new NotImplementedException();
        }
    }
}
