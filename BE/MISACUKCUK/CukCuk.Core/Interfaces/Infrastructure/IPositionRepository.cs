using CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Interfaces.Infrastructure
{
    public interface IPositionRepository : IBaseRepository<Position>
    {
        Position GetById(Guid positionId);
        int Insert(Position position);
        int Update(Position position, Guid positionId);
    }
}
