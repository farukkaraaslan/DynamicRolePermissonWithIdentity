using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using FluentValidation;

namespace Core.CrossCuttingConcerns.Validation;

public class ValidationTool
{
    public static void Validate(IValidator validator, object entity)
    {
        var context = new ValidationContext<object>(entity);

        var result = validator.Validate(context);
        if (!result.IsValid)
        {
       
            //var errorMessages = string.Join("\n", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            throw new ValidationException(result.Errors);
        }
    }
}