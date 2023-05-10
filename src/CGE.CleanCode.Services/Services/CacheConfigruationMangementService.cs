using Cge.Core.Extensions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.IO;
using CGE.CleanCode.Common;
using CGE.CleanCode.Common.Extensions;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Common.Models.Dto.Interfaces;
using CGE.CleanCode.Dal.Entities;
using CGE.CleanCode.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Services
{
	public class CacheConfigruationMangementService<T> : ICacheConfigruationMangementService
	{
		private readonly string _cacheReplaceSpaceString;
		private readonly IConfiguration _configuration;

		public CacheConfigruationMangementService(IConfiguration configuration)
		{
			_configuration = configuration.ValidateArgNotNull(nameof(configuration));

			_configuration[Globals.ConfigurationKeys.CacheReplaceSpaceString].ValidateArgNotNull(Globals.ConfigurationKeys.CacheReplaceSpaceString);

			_cacheReplaceSpaceString = _configuration[Globals.ConfigurationKeys.CacheReplaceSpaceString];
		}

		public string GetCustomKey<T1>(IDto dto)
		{
			var a = dto.GetType();

			//if (a.FullName.Contains("AthleteDto"))
			//{
			//	var c = (AthleteDto)dto;

			//	return $"{Globals.CacheKeys.Athlete}_{c.Username}";
			//}
            if (a.FullName.Contains(nameof(RoleDto)))
            {
                var c = (RoleDto)dto;

                return $"{Globals.CacheKeys.Role}_{c.RoleName.ReplaceSpacesWithThis(_cacheReplaceSpaceString)}";
            }
            if (a.FullName.Contains(nameof(RolePermissionDto)))
            {
                var c = (RolePermissionDto)dto;

                return $"{Globals.CacheKeys.RolePermission}_{c.RoleId}_{c.RouteId}";
            }

            if (a.FullName.Contains(nameof(RouteDto)))
            {
                var c = (RouteDto)dto;

                return $"{Globals.CacheKeys.Route}_{c.RouteName.ReplaceSpacesWithThis(_cacheReplaceSpaceString)}";
            }

            if (a.FullName.Contains(nameof(UserDto)))
            {
                var c = (UserDto)dto;

                return $"{Globals.CacheKeys.User}_{c.Username.ReplaceSpacesWithThis(_cacheReplaceSpaceString)}_{c.Email}";
            }

            throw new System.NotImplementedException();
		}


		public string GetKeyById<T1>(IDto dto)
		{
			var a = dto.GetType();

			//if (a.FullName.Contains("AthleteDto"))
			//{
			//	var c = (AthleteDto)dto;

			//	return $"{Globals.CacheKeys.Athlete}_{c.Id}";
			//}
            if (a.FullName.Contains(nameof(RoleDto)))
            {
                var c = (RoleDto)dto;

                return $"{Globals.CacheKeys.Role}_{c.Id}";
            }
            if (a.FullName.Contains(nameof(RolePermissionDto)))
            {
                var c = (RolePermissionDto)dto;

                return $"{Globals.CacheKeys.RolePermission}_{c.Id}";
            }

            if (a.FullName.Contains(nameof(RouteDto)))
            {
                var c = (RouteDto)dto;

                return $"{Globals.CacheKeys.Route}_{c.Id}";
            }

            if (a.FullName.Contains(nameof(UserDto)))
            {
                var c = (UserDto)dto;

                return $"{Globals.CacheKeys.User}_{c.Id}";
            }

            throw new System.NotImplementedException();
		}

		//public string GetListKey<T1>(IDto dto)
		//{
		//	var a = dto.GetType();
		//	if (a.FullName.Contains("ActivityFeedDto"))
		//	{
		//		var c = (ActivityFeedDto)dto;
		//		return $"{Globals.CacheKeys.ActivityFeed}_{c.AthleteId}";
		//	}
		//	throw new System.NotImplementedException();
		//}

		public string GetListKey<T1>()
		{
            Type type = typeof(T1);

            switch (type.Name)
            {
                //case nameof(AthleteDto):
                //    return $"{Globals.CacheKeys.AthleteList}";
                case nameof(RoleDto):
                    return $"{Globals.CacheKeys.RoleList}";
                case nameof(RolePermissionDto):
                    return $"{Globals.CacheKeys.RolePermissionList}";
                case nameof(RouteDto):
                    return $"{Globals.CacheKeys.RouteList}";
                case nameof(UserDto):
                    return $"{Globals.CacheKeys.UserList}";

                default:
                    throw new NotSupportedException($"Type '{type.FullName}' is not supported.");
            }
        }
	}
}

