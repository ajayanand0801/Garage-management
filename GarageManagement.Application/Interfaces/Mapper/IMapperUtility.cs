using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.Mapper
{
    public interface IMapperUtility
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }
}
