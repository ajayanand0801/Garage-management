using AutoMapper;
using GarageManagement.Application.Interfaces.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Mappings
{
    public class MapperUtility : IMapperUtility
    {
        private readonly IMapper _mapper;

        public MapperUtility(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination); // ✅ Fixed: map onto existing object
        }

    }

}
