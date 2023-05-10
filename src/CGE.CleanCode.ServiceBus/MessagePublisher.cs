using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CGE.CleanCode.ServiceBus
{
	public class MessagePublisher : IMessagePublisher
	{
		private readonly string _servicebusConnectionString;
		//private IContractResolver _contractResolver;
		private IMessageSender _messageSender;
		private JsonSerializerSettings _settings;
		public MessagePublisher(string servicebusConnectionString)
		{
			_servicebusConnectionString = servicebusConnectionString;
			//_contractResolver = contractResolver;
			_settings = CustomSerializerSettings.GetSerializerSettings();
			//_settings.ContractResolver = _contractResolver;
		}

		public async void Close()
		{
			await _messageSender.CloseAsync().ConfigureAwait(false);
		}

		public void Initialize(string entityName)
		{
			_messageSender = new MessageSender(_servicebusConnectionString, entityName);
		}

		public void Initialize(string connectionName, string entityName)
		{
			_messageSender = new MessageSender(connectionName, entityName);
		}

		public async Task PublishMessage(object item, Dictionary<string, string> userProperties = null)
		{
			if (item == null)
			{
				throw new ArgumentException("Message is empty");
			}

			if (_messageSender == null)
			{
				throw new System.Exception("Messager Publisher Not Initialized.");
			}

			if (_messageSender.IsClosedOrClosing)
			{
				throw new System.Exception("Message Publisher Closed.");
			}

			if (!(item is string))
			{
				item = JsonConvert.SerializeObject(item, _settings);
			}

			var message = new Message(Encoding.UTF8.GetBytes(item.ToString()));

			if (userProperties != null)
			{
				foreach (var key in userProperties.Keys)
				{
					message.UserProperties[key] = userProperties[key];
				}
			}
			await _messageSender.SendAsync(message).ConfigureAwait(false);
		}
	}
}