using System;

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
	}
}
