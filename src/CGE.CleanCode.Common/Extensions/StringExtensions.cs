using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Cge.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CGE.CleanCode.Common.Extensions
{
	public static class StringExtensions
	{
		public static string ReplaceSpacesWithThis(this string instance, string replaceValue)
		{
			if (string.IsNullOrEmpty(instance))
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if(string.IsNullOrEmpty(replaceValue))
			{
				throw new ArgumentException(nameof(replaceValue));
			}

			return instance.Replace(" ", replaceValue);
		}

		public static string RemoveWhitespace(this string instance)
		{
			if (string.IsNullOrEmpty(instance))
			{
				throw new ArgumentNullException(nameof(instance));
			}

			var reConstructedWord = string.Empty;

			foreach(char c in instance)
			{
				if(c != ' ')
				{
					reConstructedWord += c;
				}
			}

			return reConstructedWord;
		}
        
		public static async Task<string> GetKeyValultSecrets(string key,
                                                               IConfiguration configuration)
        {
            var keyVaultUrl = configuration["KeyVault:Vault"].ValidateArgNotNull("KeyVault:Vault");

            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            var secret = client.GetSecret(key).Value;

            if (secret == null)
            {
                throw new Exception("Invalid Key Valut Key");
            }

            return secret.Value;
        }
    }
}
