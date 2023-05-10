using System.Collections.Generic;
using System.Threading.Tasks;

namespace CGE.CleanCode.ServiceBus
{
	public interface IMessagePublisher
	{
		void Close();

		void Initialize(string entityName);
		void Initialize(string connectionName, string entityName);
		Task PublishMessage(object item, Dictionary<string, string> userProperties = null);
	}
}