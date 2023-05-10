using CGE.CleanCode.Common.Models.Dto.Interfaces;

namespace CGE.CleanCode.Service.Interfaces
{
	public interface ICacheConfigruationMangementService
	{
		string GetCustomKey<T>(IDto dto);
		string GetKeyById<T>(IDto dto);
		//string GetListKey<T>(IDto dto);
		string GetListKey<T1>();
	}
}