using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Notepad.PhysicalActivity.Common.Models.Patch;
using System.Collections.Generic;
using System.Linq;

namespace Notepad.PhysicalActivity.Api.Models.Validators.Patch
{
	public class PatchAthleteValidator : AbstractValidator<JsonPatchDocument<PatchAthlete>>
	{
		/// <summary>
		/// Because the JsonPatch Api is Open (add, copy, move, remove, replace, test), we have to add some rules so that we aren't getting unprocessable 
		/// requests (i.e., "add 'someCrazyProperty' to a Branch" or "remove the Branch 'Name'").
		/// </summary>
		internal static readonly Dictionary<string, string[]> SupportedOperations = new Dictionary<string, string[]>()
		{
				{"/DOB", new [] {"replace"} },
				{"/Email", new [] {"replace"} },
				{"/ImageUrl", new [] {"replace"} },
				{"/IsDeleted", new [] {"replace"} }
		};

		/// <summary>
		/// 
		/// </summary>
		public PatchAthleteValidator()
		{
			RuleLevelCascadeMode = CascadeMode.Stop;

			RuleFor(o => o.Operations)
				.Must((x, y) => BeSupportedOperation(y.AsQueryable()))
				.WithErrorCode(nameof(Resource.ERRUnsupportedOperation))
				.WithMessage(Resource.ERRUnsupportedOperation);
		}

		private bool BeSupportedOperation(IQueryable<Operation<PatchAthlete>> operation)
		{
			if (!operation
				.Select(op => op)
				.Any(patchOrder => SupportedOperations.Any(kvp => patchOrder.path.ToLower().Contains(kvp.Key.ToLower()) && kvp.Value.Any(d => d == patchOrder.op))))
			{
				return false;
			}
			return true;
		}
	}
}
