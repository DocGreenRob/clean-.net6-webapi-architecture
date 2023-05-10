using AutoMapper;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Dal.Entities;

namespace CGE.CleanCode.Service.Mappings
{
	public class EntityToDtoProfiles : Profile
	{
		public EntityToDtoProfiles()
		{
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<RolePermission, RolePermissionDto>().ReverseMap();
            CreateMap<Route, RouteDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
	}
}
