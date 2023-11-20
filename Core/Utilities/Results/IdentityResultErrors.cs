using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results;

public class IdentityResultErrors : ErrorResult
{
    public IdentityResultErrors(IdentityResult result)
        : base("Identity operation failed.")
    {
        Errors = result.Errors.Select(e => new IdentityErrorModel
        {
            Code = e.Code,
            Description = e.Description
        }).ToList();
    }

    public List<IdentityErrorModel> Errors { get; set; }
}

public class IdentityErrorModel
{
    public string Code { get; set; }
    public string Description { get; set; }
}