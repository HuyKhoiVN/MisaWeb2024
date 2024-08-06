using CukCuk.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Interfaces.Infrastructure
{
    public interface IBaseRepository<MisaEntity>
    {
        IEnumerable<MisaEntity> GetAllEntities();
        MisaEntity GetById(Guid entityId);
        int Insert(MisaEntity entity);
        int Update(MisaEntity entity, Guid entityId);
        int Delete(Guid entityId);
    }
}
