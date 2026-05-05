using AutoMapper;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShopEgypt.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Cart Profile
            //CreateMap<Cart, CartDto>()

            //.ForMember(dest => dest.Status,
            //    opt => opt.MapFrom(src => src.Status.ToString()));

            


            #endregion


            
        }
    }

}
