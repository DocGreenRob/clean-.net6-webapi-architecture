using FluentValidation;
using CGE.CleanCode.Common.Models.Dto;
using System;

namespace CGE.CleanCode.Api.Models.Validators
{
    public class RolePermissionValidator : AbstractValidator<RolePermissionDto>
    {
        public RolePermissionValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.RouteId)
                .NotEmpty()
                .WithErrorCode(nameof(Resource.ERRIsRequired))
                .WithMessage(Resource.ERRIsRequired)
                .NotNull()
                .WithErrorCode(nameof(Resource.ERRIsRequired))
                .WithMessage(Resource.ERRIsRequired)
                .MaximumLength(50)
                .WithErrorCode(nameof(Resource.ERRMaxLength50))
                .WithMessage(Resource.ERRMaxLength50);

            RuleFor(x => x.RoleId)
               .NotEmpty()
               .WithErrorCode(nameof(Resource.ERRIsRequired))
               .WithMessage(Resource.ERRIsRequired)
               .NotNull()
               .WithErrorCode(nameof(Resource.ERRIsRequired))
               .WithMessage(Resource.ERRIsRequired)
               .MaximumLength(50)
               .WithErrorCode(nameof(Resource.ERRMaxLength50))
               .WithMessage(Resource.ERRMaxLength50);

        }

        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default(DateTime));
        }

        private bool BeValid(string bodypart)
        {
            //foreach (var value in Enum.GetValues(typeof(Bodypart)))
            //{
            //    if (bodypart == value.ToString())
            //    {
            //        return true;
            //    }
            //}

            return false;
        }
    }
}
