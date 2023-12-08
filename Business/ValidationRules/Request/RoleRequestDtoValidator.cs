using Business.Dto.Role;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.Request;

public class RoleRequestDtoValidator :AbstractValidator<RoleRequestDto>
{
    public RoleRequestDtoValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Name).MinimumLength(3);
    }
}
