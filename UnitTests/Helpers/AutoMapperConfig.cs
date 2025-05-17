using System;
using AutoMapper;
using Application.Mappings;

namespace UnitTests.Helpers
{
    public static class AutoMapperConfig
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(e =>
            {
                e.AddProfile<MappingProfile>();
            });

            return mapperConfig.CreateMapper();
        }
    }
}
