namespace CGE.CleanCode.Common
{
	public static class Globals
	{

		public static class CacheKeys
		{
			
			public static string Athlete { get { return "athlete"; } }
			public static string AthleteList { get { return "all_athletes"; } }

            public static string Role { get { return "role"; } }
            public static string RoleList { get { return "all_roles"; } }


            public static string RolePermission { get { return "rolePermission"; } }
            public static string RolePermissionList { get { return "all_rolePermission"; } }


            public static string Route { get { return "route"; } }
            public static string RouteList { get { return "all_routes"; } }

            public static string User { get { return "user"; } }
            public static string UserList { get { return "all_users"; } }
        }

		public static class ConfigurationKeys
		{
			public static string Database { get { return "MongoDb:Database"; } }
			public static string RedisCacheHostName { get { return "Redis:Azure:HostName"; } }
			public static string RedisCacheKey { get { return "Redis:Azure:Key"; } }
			public static string CacheReplaceSpaceString { get { return "Redis:CacheConfig:ReplaceSpaceString"; } }
			//public static string Database { get { return "MongoDb:Database"; } }
			//public static string Database { get { return "MongoDb:Database"; } }
			//public static string Database { get { return "MongoDb:Database"; } }
		}

		public static class Documents
		{
			public static string Role { get { return "Role"; } }
			public static string RolePermission { get { return "RolePermission"; } }
			public static string Route { get { return "Route"; } }
            public static string User { get { return "User"; } }

        }
	}
}
