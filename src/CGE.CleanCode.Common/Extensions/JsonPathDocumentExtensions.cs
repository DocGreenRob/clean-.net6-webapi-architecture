using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace CGE.CleanCode.Common.Extensions
{
	public static class JsonPathDocumentExtensions
	{
		public const string InvalidPatchDocumentCode = "InvalidPatchDocument";
		public const string InvalidPatchDocumentProperty = "JsonPatchDocument";
		private static readonly ConcurrentDictionary<Type, ConstructorInfo> _defaultConstructors = new ConcurrentDictionary<Type, ConstructorInfo>();
		private static readonly ConcurrentDictionary<Type, IValidator> _validators = new ConcurrentDictionary<Type, IValidator>();


		public static JsonPatchDocument<TOut> Map<TIn, TOut>(this JsonPatchDocument<TIn> instance)
			where TIn : class, new()
			where TOut : class, new()
		{
			return new JsonPatchDocument<TOut>(instance.Operations.Select(x => x.Map<TIn, TOut>()).ToList(), instance.ContractResolver);
		}

		public static Operation<TOut> Map<TIn, TOut>(this Operation<TIn> instance)
			where TIn : class, new()
			where TOut : class, new()
		{
			return new Operation<TOut>(instance.op, instance.path, instance.from, instance.value);
		}
	}
}
