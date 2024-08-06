using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Interfaces.Services
{
    public interface IBaseService<MisaEntity>
    {
        int InsertService(MisaEntity entity);
        int UpdateService(MisaEntity entity, Guid entityId);
    }
}
