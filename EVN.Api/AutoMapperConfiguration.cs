using AutoMapper;
using EVN.Api.Model;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public static class AutoMapperConfiguration
    {
        [Obsolete]
        public static void Configure()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Department, DepartmentTreeReponse>().ReverseMap();
                config.CreateMap<DepartmentRequest, Department>().ReverseMap();
            });
        }
    }

    public static class SupportAutoMapper
    {
        public static T Map<T>(this object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}